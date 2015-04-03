#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using eDriven.Core.Signals;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Designer.Rendering;
using eDriven.Gui.Editor.Building;
using eDriven.Gui.Editor.Hierarchy;
using eDriven.Gui.Editor.Persistence;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Processing
{
    /// <summary>
    /// This is the very important class for tracking hierarchy changes
    /// We decided to propagate all the changes throught the hierarchy: nothing is
    /// being processed directly, but we manipulate hierarchy in order of adding/removing children etc.
    /// This way we are always sure that we process all the changes the same way
    /// We listen for hierarchy changes and do the appropriate action
    /// There is a difference in processing hierarchy changes in edit mode and play mode:
    /// 
    /// 1. Edit mode
    /// When in edit mode, we should process each hierarchy change (each delta) in order of refreshing group packs
    /// Group packs are constructs taking care of parent-child relationship between adapters:
    /// When we add/remove or move the adapter, we have to take the appropriate action on its container
    /// 
    /// 2. Play mode
    /// In play mode, we need to track 2 kinds of hierarchy changes:
    /// 
    /// a) The first one is the same one as in the edit mode. We need to track each delta in order to process
    /// the parent adapter children (and also for processing actual "component" children - making "live" 
    /// visual changes on the screen)
    /// 
    /// b) The second delta we need to track is delta between the original hierarchy (hierarchy snapshot made
    /// the moment the play mode is started) and final hierarchy (the moment the play mode is stopped).
    /// We use this delta to apply changes to the hierarchy on play mode stop.
    /// This is a very robust mechanism because it doesn't rely on tracking multiple "small" deltas.
    /// </summary>
    internal class HierarchyChangeProcessor
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static HierarchyChangeProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private HierarchyChangeProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static HierarchyChangeProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating HierarchyChangeProcessor instance"));
#endif
                    _instance = new HierarchyChangeProcessor();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        #region Signals

        public readonly Signal ChangesProcessedSignal = new Signal();

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            EditorState.Instance.HierarchyChangeSignal.Connect(HierarchyChangeSlot);
            PlayModeStateChangeEmitter.Instance.LevelLoadedSignal.Connect(LevelLoadedSlot);
        }

        /// <summary>
        /// The snapshot of the original hierarchy
        /// </summary>
        private Node _originalHierarchy = new Node(null);

        /// <summary>
        /// Previous snapshot
        /// </summary>
        private Node _oldHierarchy = new Node(null);

        /// <summary>
        /// Current snapshot
        /// </summary>
        private Node _newHierarchy;

        /// <summary>
        /// A flag indicated that Unity finished instantiating components when play mode started
        /// This flag is being set to false on OnEnable() and is being set to true on OnHierarchyChange()
        /// </summary>
        //private bool _initialHierarchyChangeDone;

        /// <summary>
        /// The flag indicating that the new level is loaded so we should cache the Level 1 values and stop monitoring for changes
        /// </summary>
        private static bool _levelLoaded;

        private Delta _stepDelta;
        public Delta StepDelta
        {
            get { return _stepDelta; }
        }

        private Delta _fullDelta;
        public Delta FullDelta
        {
            get { return _fullDelta; }
        }

        /// <summary>
        /// Called the moment the play mode starts
        /// Used for serializing the starting state
        /// </summary>
        /// <returns></returns>
        public void InitialSnapshot()
        {
            //LogUtil.PrintCurrentMethod();
            /**
             * 1. Take the hierarchy snapshot
             * */
            _originalHierarchy = HierarchyBuilder.Instance.BuildHierarchy();

            //Debug.Log(_originalHierarchy.DescribeHierarchy());
        }

        public void HierarchyChangeSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("HierarchyChangeProcessor->HierarchyChangeSlot");
            }
#endif
            
            if (!Application.isPlaying && !EditorSettings.WatchChanges)
                return; // not watching for changes in play mode, return

            /**
             * 1. Take the hierarchy snapshot
             * */
            _newHierarchy = HierarchyBuilder.Instance.BuildHierarchy();
            
            /*/**
             * 2. Return for the first time (if the flag is OFF)
             * (first hierarchy change happens the moment the project is loaded)
             * #1#
            if (!_initialHierarchyChangeDone)
            {
                _initialHierarchyChangeDone = true;
                _oldHierarchy = _newHierarchy;
                return;
            }*/

            /**
             * 3. If second level loaded, go no further - do not process any more changes
             * */
            if (_levelLoaded)
            {
                return;
            }

            /**
             * 4. Process changes
             * If the application is playing, we are processing changes
             * If not playing, we have to be sure that this is not inside a few frames after the Play mode is stopped
             * This is because the PersistenceManager only should submit changes after the play mode is stopped,
             * or else we might end in edit mode change processing (like we added new components by hand)
             * */
            if (Application.isPlaying || 
                (!Application.isPlaying && EditorSettings.ReadyToProcessHierarchyChanges == 0))
            {
                ProcessChanges(Application.isPlaying);
                /**
                 * 5. Process the selection the newly created component
                 * */
                SelectionChangeProcessor.Instance.SelectCreatedComponent();
            }

            /**
             * After the play mode stopped, and if the hierarchy has not changed yet (flag not set), set it:
             * */
            if (EditorSettings.ReadyToProcessHierarchyChanges > 0/* && !Application.isPlaying*/)
            {
                EditorSettings.ReadyToProcessHierarchyChanges -= 1;
            }
            
            /**
             * 7. Rescan hierarchy tree icons
             * Important when expanding a part of the tree
             * */
            HierarchyViewDecorator.Instance.ReScan();

            if (null == Selection.activeTransform && null != DesignerOverlay.Instance) // DesignerOverlay.Instance might be null
                DesignerOverlay.Instance.Deselect();
        }

        /// <summary>
        /// Fires when scene changes (another level loads)
        /// The persisted delta has to be reset
        /// This solves the persistance problem reported here: http://forum.unity3d.com/threads/148424-eDriven-Q-amp-A/page10?p=1198331&viewfull=1#post1198331
        /// Without this reset all the scene objects (from the 1st scene) were deleted when user programatically switches to the 2nd scene
        /// </summary>
        /// <see>http://forum.unity3d.com/threads/148424-eDriven-Q-amp-A/page10?p=1198331&viewfull=1#post1198331</see>
        /// <param name="parameters"></param>
        internal void LevelLoadedSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"*** Level loaded ***
Resetting the accumulated delta"));
            }
#endif
            /*if (!_levelLoaded)
            {
                Debug.LogWarning("Level loaded. Only the 1st scene changes will be persisted.");
                //PersistenceManager.Instance.Persist(true);
            }*/
            _levelLoaded = true;
        }

        /// <summary>
        /// Processes changes
        /// </summary>
        private void ProcessChanges(bool inPlayMode)
        {
            /* Calculating the single step change */
            _stepDelta = HierarchyComparer.Compare(
                _oldHierarchy,
                _newHierarchy
            );

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(_stepDelta);
            }
#endif
            //Debug.Log(delta);

            /**
             * 2. Handle additions
             * a) Instantiate component
             * b) Record transform addition
             * */

            if (inPlayMode)
            {
                /**
                 * 1. PLAY mode
                 * Leaving the processing to persistence logic
                 * */

                /**
                 * In play mode we need a full delta, so we will be able to re-apply it
                 * after the play mode is stopped
                 * Tracking full delta with each change, because we don't know when
                 * will be the last chance to track it before stopping the play mode
                 * */
                _fullDelta = HierarchyComparer.Compare(
                    _originalHierarchy,
                    _newHierarchy
                );

                /**
                 * IMPORTANT:
                 * This was the source of the quite performance killing recursive bug
                 * If this processing take place in play mode, OnHierarchyChange fires in each consequent frame, without ever stopping
                 * TODO: do a special logic for reordering items in play mode
                 * */
                AdditionProcessor.Process(_stepDelta.TopLevelAdditions, _stepDelta.Additions);
                RemovalProcessor.Process(_stepDelta.Removals);
                MovesProcessor.Process(_stepDelta.Moves);

                /**
                 * Accumulate delta
                 * */
                PersistenceManager.Instance.Delta = _fullDelta; //.Reset(); // TEMP -> create a direct Delta setter
                //PersistenceManager.Instance.Delta.Accumulate(_fullDelta);
            }
            else
            {
                /**
                * 2. EDIT mode
                * In edit mode, so handling the ordering information immediatelly
                * Note: the ordering information is all that has to be processed when adding a new component 
                * or a prefab in Edit mode - other things (parent-child relationship etc.) is handled by Unity
                * */
                EditModeAdditionsProcessor.Process(_stepDelta.TopLevelAdditions);
                EditModeRemovalProcessor.Process(_stepDelta.TopLevelRemovals);
                EditModeMovesProcessor.Process(_stepDelta.Moves);
            }

            _stepDelta.Reset();

            // save the current hierarchy
            _oldHierarchy = _newHierarchy;

            EditorState.Instance.Adapter = ComponentRegistry.Instance.Get(EditorState.Instance.AdapterId, true) as ComponentAdapter;

            ChangesProcessedSignal.Emit();
        }

        public void Reset()
        {
            if (null != _fullDelta)
                _fullDelta.Reset();
            if (null != _stepDelta)
                _stepDelta.Reset();

            _originalHierarchy = new Node(null);
            _oldHierarchy = new Node(null);
            _newHierarchy = null;
        }
    }
}