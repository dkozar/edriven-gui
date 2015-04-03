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
using eDriven.Gui.Components;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Processing
{
    internal class EditorState
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static EditorState _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private EditorState()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EditorState Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating EditorState instance"));
#endif
                    _instance = new EditorState();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        public Signal DepthChangeSignal = new Signal();
        
        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            SelectionChangeSignal.Connect(SelectionChangeProcessor.Instance.SelectionChangeSlot);
            /*var p = PlayModeStateChangeEmitter.Instance; // init
            p.PlayModeStartedSignal.Connect(PlayModeStartedSlot);*/
        }

        private ComponentAdapter _adapter;
        public ComponentAdapter Adapter
        {
            get
            {
                return _adapter;
            }
            set
            {
                _adapter = value;
                if (null != _adapter)
                    _adapterId = _adapter.GetInstanceID();
                _groupAdapter = _adapter as GroupAdapter;
            }
        }

        private int _adapterId;
        public int AdapterId
        {
            get { return _adapterId; }
        }

        private GroupAdapter _groupAdapter;
        public GroupAdapter GroupAdapter
        {
            get
            {
                return _groupAdapter;
            }
        }

        public int Depth
        {
            set
            {
                //if (null != Adapter.ParentAdapter)
                //{
                //    Adapter.ParentAdapter.
                //}

                DepthChangeSignal.Emit(value);
            }
        }

        #region Serialized object

        private static SerializedObject _serializedObject;

        /// <summary>
        /// The serialized object being set by component editor<br/>
        /// This happens whenever the component is selected in the hierarchy
        /// </summary>
        internal static SerializedObject SerializedObject
        {
            get
            {
                return _serializedObject;
            }
            set
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("Setting SerializedObject to: " + value);
                }
#endif
                _serializedObject = value;
            }
        }

        #endregion

        #region Selected transform

        public readonly Signal SelectionChangeSignal = new Signal();

        private Transform _selectedTransform;

        public Transform SelectedTransform
        {
            get
            {
                return _selectedTransform;
            }
            set
            {
                if (value == _selectedTransform)
                    return;

                _selectedTransform = value;

                if (null != _selectedTransform) { 
                    
                    // 1. evaluate adapter
                    Adapter = GuiLookup.GetAdapter(_selectedTransform.gameObject);

                    // 2. when we touch the component, we want to remember its skin to be used in the next created component
                    if (null != Adapter && EditorSettings.ApplyLastUsedSkinUponCreation/* && typeof(SkinnableComponent).IsAssignableFrom(Adapter.ComponentType)*/)
                    {
                        var skinnableComponentAdapter = Adapter as SkinnableComponentAdapter;
                        string skinClass = null;
                        if (null != skinnableComponentAdapter)
                        {
                            skinClass = skinnableComponentAdapter.SkinClass;
                        }

                        var skinnableContainerAdapter = Adapter as SkinnableContainerAdapter;
                        if (null != skinnableContainerAdapter)
                        {
                            skinClass = skinnableContainerAdapter.SkinClass;
                        }

                        if (!string.IsNullOrEmpty(skinClass))
                            SkinnableComponentAdapter.SkinChanged(Adapter.GetType(), skinClass);
                    }

                    /* TODO: 
                     * This is the alternative way for monitoring ALL the Unity components
                     * Meditate on this */
                    /*if (EditorApplication.isPlaying && EditorSettings.WatchChanges)
                    {
                        var unityComponents = _selectedTransform.gameObject.GetComponents<UnityEngine.Component>();
                        foreach (UnityEngine.Component component in unityComponents)
                        {
                            PersistenceManager.Instance.Watch(component);
                        }
                    }*/
                }

                SelectionChangeSignal.Emit(_selectedTransform);
            }
        }

        public static bool ShouldFixHierarchyAfterStop { get; set; }

        #endregion

        #region Hierarchy change

        public readonly Signal HierarchyChangeSignal = new Signal();

        public void HierarchyChange()
        {
            //LogUtil.PrintCurrentMethod();
            HierarchyChangeSignal.Emit();
        }

        #endregion

        /*#region Hierarchy change

        public readonly Signal HierarchyProcessedSignal = new Signal();

        public void HierarchyProcessed()
        {
            //LogUtil.PrintCurrentMethod();
            HierarchyProcessedSignal.Emit();
        }

        #endregion*/

        #region Theme change

        public readonly Signal ThemeChangeSignal = new Signal();
        public void ThemeChanged()
        {
            ThemeChangeSignal.Emit();
        }

        #endregion

        #region OnEnable

        public readonly Signal OnEnableSignal = new Signal();

        public void OnEnable()
        {
            OnEnableSignal.Emit();
        }

        #endregion

        #region OnDisable

        public readonly Signal OnDisableSignal = new Signal();

        public void OnDisable()
        {
            OnDisableSignal.Emit();
        }

        #endregion

        public readonly Signal WatchChangesChangedSignal = new Signal();

        public void WatchChangesChanged()
        {
            WatchChangesChangedSignal.Emit();
        }
    }
}
