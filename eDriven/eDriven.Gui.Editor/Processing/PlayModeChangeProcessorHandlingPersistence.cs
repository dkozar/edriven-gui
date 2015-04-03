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

using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Persistence;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Processing
{
    /// <summary>
    /// Takes care of showing/hiding the designer overlay
    /// </summary>
    internal class PlayModeChangeProcessorHandlingPersistence
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PlayModeChangeProcessorHandlingPersistence _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PlayModeChangeProcessorHandlingPersistence()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PlayModeChangeProcessorHandlingPersistence Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating PlayModeChangeProcessorHandlingPersistence instance"));
#endif
                    _instance = new PlayModeChangeProcessorHandlingPersistence();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            var p = PlayModeStateChangeEmitter.Instance;
            p.PlayModeStartedSignal.Connect(PlayModeStartedSlot);
            p.PlayModeStoppingSignal.Connect(PlayModeStoppingSlot);
            p.PlayModeStoppedSignal.Connect(PlayModeStoppedSlot);
            p.SceneChangeSignal.Connect(SceneChangeSlot);
            p.LevelLoadedSignal.Connect(LevelLoadedSlot);
        }

        #region Slots

        public void PlayModeStartedSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("PlayModeChangeProcessorHandlingPersistence->PlayModeStartedSlot");
            }
#endif
            _changesCached = false;

            HierarchyChangeProcessor.Instance.InitialSnapshot();
        }

        /// <summary>
        /// Fires when play mode stopping
        /// Used for caching the current values
        /// </summary>
        /// <param name="parameters"></param>
        private void PlayModeStoppingSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("PlayModeChangeProcessorHandlingPersistence->PlayModeStoppingSlot");
            }
#endif
            // do not process hierarchy changes until the persistance applies its own changes
            //EditorSettings.ReadyToProcessHierarchyChanges = 20;

            #region Delay stuff

            // note: we have to handle this here, not in the PlayModeStoppedSlot, because the Delta is being reset by then

            EditorSettings.ReadyToProcessHierarchyChanges = 1;

            //Debug.Log("=== Delta: " + PersistenceManager.Instance.Delta);

            // Important: if there are hierarchy changes to be applied, we should expect one more OnHierarchyChange call!
            if (PersistenceManager.Instance.Delta.HasChanges)
            {
                //Debug.Log("-------- DELTA HAS CHANGES --------");
                EditorSettings.ReadyToProcessHierarchyChanges += 1; // the total is 2
            }

            #endregion

            //Selection.activeObject = null; // TODO
            //EditorState.Instance.Adapter = null;
            //OrderDisplay.Instance.Clear();

            if (!_changesCached)
                CacheValues(false);
        }

        public void PlayModeStoppedSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("PlayModeChangeProcessorHandlingPersistence->PlayModeStoppedSlot");
            }
#endif
            if (EditorSettings.WatchChanges)
            {
                PersistenceManager.Instance.ApplyChanges();
                //HierarchyOrderScanner.Instance.ApplyChanges();
            }

            // process the font mapper prerequisite addition
            FontMapperAdditionProcessor.Instance.Process();

            // process the audio mapper prerequisite addition
            AudioMapperAdditionProcessor.Instance.Process();

            // process the script addition (play mode has been stopped to allow the addition)
            PostPlayModeStopScriptProcessor.Instance.Process();

            // do not process hierarchy changes until the persistance applies its own changes
            //EditorSettings.ReadyToProcessHierarchyChanges = 100;
        }

        /// <summary>
        /// Fires when the scene is about to change
        /// We have to cache all the changes from the 1st scene, just like in th e
        /// </summary>
        /// <param name="parameters"></param>
        private static void SceneChangeSlot(object[] parameters)
        {
            if (!_changesCached)
                CacheValues(true);
        }

        /// <summary>
        /// Fires when scene changes (another level loads)
        /// The persisted delta has to be reset
        /// This solves the persistance problem reported here: http://forum.unity3d.com/threads/148424-eDriven-Q-amp-A/page10?p=1198331&viewfull=1#post1198331
        /// Without this reset all the scene objects (from the 1st scene) were deleted when user programatically switches to the 2nd scene
        /// </summary>
        /// <see>http://forum.unity3d.com/threads/148424-eDriven-Q-amp-A/page10?p=1198331&viewfull=1#post1198331</see>
        /// <param name="parameters"></param>
        private static void LevelLoadedSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"*** Level loaded ***
Resetting the accumulated delta"));
            }
#endif
            if (!_levelLoaded)
            {
                Debug.LogWarning("Level loaded. Only the 1st scene changes will be persisted.");
                //PersistenceManager.Instance.Persist(true);
            }
            _levelLoaded = true;
        }

        #endregion

        /// <summary>
        /// The flag indicating that the new level is loaded so we should cache the Level 1 values and stop monitoring for changes
        /// </summary>
        private static bool _levelLoaded;

        /// <summary>
        /// The flag indicating that changes have already been cached, thus avoiding multiple call to Persist()
        /// </summary>
        private static bool _changesCached;

        private static void CacheValues(bool cachingOnSceneChange)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Persist. cachingOnSceneChange=" + cachingOnSceneChange);
            }
#endif
            if (!_levelLoaded)
            {
                //            Debug.Log(string.Format(@"##### ParentChildLinker #####
                //{0}", ParentChildLinker.Instance.DescribeChanges()));
                PersistenceManager.Instance.CacheValues(true, cachingOnSceneChange);
            }

            _changesCached = true;
        }
    }
}