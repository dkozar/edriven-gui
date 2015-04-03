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

using System;
using System.Collections.Generic;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Persistence;
using eDriven.Gui.Editor.Processing;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class Toolbox
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static Toolbox _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private Toolbox()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Toolbox Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating Toolbox instance"));
#endif
                    _instance = new Toolbox();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {
            
        }

        private bool _oldEnabled;
        private bool _showControlsHelp;
        private Vector2 _scrollPosition;
        private readonly Type _stageDescriptorType = typeof(StageAdapter);
        private readonly Type _componentDescriptorType = typeof(ComponentAdapter);
        
        public GUIContent StageContent;

        private int _selectedIndex = -1;

        public float Width;

        private readonly List<GroupDesc> _groups = new List<GroupDesc>();
        internal List<GroupDesc> Groups
        {
            get { return _groups; }
        }

        private readonly Dictionary<string, bool> _previousGroupState = new Dictionary<string, bool>();

        internal void Add(ComponentTypeDesc desc)
        {
            var group = Groups.Find(delegate(GroupDesc groupDesc) { return groupDesc.Name == desc.Group; });
            if (null == group)
            {
                group = new GroupDesc(desc.Group);
                Groups.Add(group);
            }
            group.Add(desc);
        }

        internal void Clear()
        {
            foreach (GroupDesc desc in _groups)
            {
                _previousGroupState[desc.Name] = desc.Expanded;
                desc.Clear();
            }

            _groups.Clear();
        }

        internal void Process()
        {
            _groups.Sort(NameComparison);
            foreach (GroupDesc desc in _groups)
            {
                desc.Process();
                if (_previousGroupState.ContainsKey(desc.Name))
                {
                    desc.Expanded = _previousGroupState[desc.Name];
                    _previousGroupState.Remove(desc.Name);
                }
            }
        }

        private static readonly Comparison<GroupDesc> NameComparison = delegate(GroupDesc c1, GroupDesc c2)
                                                                           {
                                                                               return String.Compare(c1.Name, c2.Name, StringComparison.Ordinal);
                                                                           };

        internal void Render()
        {
            //GUI.backgroundColor = RgbColor.FromHex(0x335fd8).ToColor();

            //GUI.backgroundColor = Color.white;

            //EditorGUILayout.BeginVertical(StyleCache.Instance.PanelContent, GUILayout.ExpandWidth(true));
            // panel content

            if (ToolboxDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.Toolbox, MessageType.Info, true);
            }

            /**
             * Breadcrumbs
             * */
            if (null != Selection.activeGameObject)
            {
                BreadcrumbsMain.Instance.Render();
                GUILayout.Space(5);
            }

            if (EditorSettings.ShowAddChildOptions)
            {
                EditorGUILayout.BeginVertical(StyleCache.Instance.Fieldset, GUILayout.ExpandWidth(true));
                GUILayout.Label(GuiContentCache.Instance.OptionsFieldsetTitle, StyleCache.Instance.Label,
                                GUILayout.ExpandWidth(false));

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                EditorSettings.SelectUponCreation = GUILayout.Toggle(EditorSettings.SelectUponCreation,
                                                                     GuiContentCache.Instance.SelectContent,
                                                                     StyleCache.Instance.Button,
                                                                     GUILayout.ExpandWidth(false));
                EditorSettings.ExpandWidthUponCreation = GUILayout.Toggle(EditorSettings.ExpandWidthUponCreation,
                                                                          GuiContentCache.Instance.
                                                                              ExpandWidthContent,
                                                                          StyleCache.Instance.Button,
                                                                          GUILayout.ExpandWidth(false));
                EditorSettings.ExpandHeightUponCreation = GUILayout.Toggle(EditorSettings.ExpandHeightUponCreation,
                                                                           GuiContentCache.Instance.
                                                                               ExpandHeightContent,
                                                                           StyleCache.Instance.Button,
                                                                           GUILayout.ExpandWidth(false));
                EditorSettings.FactoryModeUponCreation = GUILayout.Toggle(EditorSettings.FactoryModeUponCreation,
                                                                          GuiContentCache.Instance.FactoryModeContent,
                                                                          StyleCache.Instance.Button,
                                                                          GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                
                EditorSettings.ApplyLastUsedSkinUponCreation = GUILayout.Toggle(EditorSettings.ApplyLastUsedSkinUponCreation,
                                                                          GuiContentCache.Instance.ApplyLastUsedSkinUponCreationContent,
                                                                          StyleCache.Instance.Button,
                                                                          GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                GUILayout.Space(5);
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            /**
             * 1. Stage
             * */
            _oldEnabled = GUI.enabled;
            GUI.enabled = ToolboxInitializer.ControlCouldBeInstantiated(Selection.activeTransform,
                                                                        _stageDescriptorType);

            if (GUILayout.Button(StageContent, StyleCache.Instance.ControlButton, GUILayout.ExpandWidth(false)))
            {
                /**
                 * 1. Initialize prerequisites for this scene
                 * */
                if (!SceneInitializer.Init())
                    return;

                //Debug.Log("*** INITIALIZED ***");

                /**
                 * 2. Create stage
                 * */
                CreateChild(new ComponentTypeDesc("Stage", typeof(StageAdapter)),
                            Event.current.control || EditorSettings.SelectUponCreation);
            }
            GUI.enabled = _oldEnabled;

            EditorGUILayout.Space();

            /**
             * 2. Other controls
             * */
            foreach (var group in Groups)
            {
                group.Expanded = EditorPrefs.GetBool("eDrivenGuiEditorControlGroup_" + group.Name);

                bool newExpanded = EditorGUILayout.Foldout(group.Expanded, group.Name);
                if (newExpanded != group.Expanded)
                {
                    EditorPrefs.SetBool("eDrivenGuiEditorControlGroup_" + group.Name, newExpanded);
                    group.Expanded = newExpanded;
                }

                if (group.Expanded)
                {
                    _oldEnabled = GUI.enabled;
                    GUI.enabled = ToolboxInitializer.ControlCouldBeInstantiated(Selection.activeTransform,
                                                                                _componentDescriptorType);

                    var columnCount = Math.Max((int)Mathf.Floor((Screen.width - 40) / DipSwitches.ControlButtonMinWidth), 1);
                    //Debug.Log("Screen.width: " + Screen.width);
                    //Debug.Log("columnCount: " + columnCount);
                    // against dividing by zero
                    _selectedIndex = GUILayout.SelectionGrid(_selectedIndex, group.Contents, columnCount,
                                                             StyleCache.Instance.ControlButton,
                                                             GUILayout.Width(columnCount * DipSwitches.ControlButtonMinWidth));
                    if (_selectedIndex > -1)
                    {
                        //Debug.Log("Click: " + _selectedIndex);
                        CreateChild(group.Components[_selectedIndex],
                                    Event.current.control || EditorSettings.SelectUponCreation);

                        //OrderDisplay.Instance.Refresh();

                        _selectedIndex = -1;
                    }

                    GUI.enabled = _oldEnabled;
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(1);
        }

        /// <summary>
        /// Creates a child of the selected component descriptor
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="selectCreatedControl">Should the newly created control be selected, or the selection should be as it is (container)</param>
        private static void CreateChild(ComponentTypeDesc desc, bool selectCreatedControl)
        {
            //Debug.Log("CreateChild: " + desc);
            GameObject go = new GameObject(desc.Name);

            go.transform.parent = Selection.activeTransform;
            go.transform.localPosition = Vector3.zero;

            ComponentAdapter adapter = (ComponentAdapter)go.AddComponent(desc.Type);

            // in in play mode, start monitoring this component because we need to setup immediatelly
            if (EditorApplication.isPlaying)
                PersistenceManager.Instance.Watch(adapter);

            // setup default values
            SetupDefaults(Event.current.alt, adapter);
            
            /**
            * When not in play mode, we are adding the adapter directly to a child colection
            * (no play mode persistance)
            * */
            if (null != EditorState.Instance.GroupAdapter && !Application.isPlaying)
            {
                //Debug.Log("Adding child in edit mode: " + adapter);
                EditorState.Instance.GroupAdapter.AddChild(adapter, true);
                //ParentChildLinker.Instance.Link(EditorState.ContainerAdapter, adapter);
                //ParentChildLinker.Instance.Process(); // process immediately
            }

            // the following should definitelly be after setting the active object:
            if (selectCreatedControl) {
                Selection.activeGameObject = go;
            }

            // hierarchy change mechanism should do the rest
        }

        private static void SetupDefaults(bool alt, ComponentAdapter adapter)
        {
            if (alt || EditorSettings.ExpandWidthUponCreation)
            {
                adapter.UseWidth = true;
                adapter.UsePercentWidth = true;
                adapter.Width = 100;
            }

            if (alt || EditorSettings.ExpandHeightUponCreation)
            {
                adapter.UseHeight = true;
                adapter.UsePercentHeight = true;
                adapter.Height = 100;
            }

            if (EditorSettings.FactoryModeUponCreation)
            {
                adapter.FactoryMode = true;
            }

            var skinnableComponentAdapter = adapter as SkinnableComponentAdapter;
            if (skinnableComponentAdapter != null)
            {
                skinnableComponentAdapter.ApplySkin(EditorSettings.ApplyLastUsedSkinUponCreation);
            }
        }

        //public void ResetLastAddition()
        //{
        //    _isAdded = false;
        //    _createdTransform = null;
        //}
    }
}