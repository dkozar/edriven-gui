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
using System.Reflection;
using System.Text;
using eDriven.Gui.Components;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(ComponentAdapter))]
    [CanEditMultipleObjects]
    [Serializable]
    [Obfuscation(Exclude = true)]
    public class ComponentEditor : ComponentEditorBase
    {
        #region Context menu commands

        // Note: context menu works when right-clicking the component name label

// Add a menu item called "Adapter Command" to a ComponentAdapter's context menu.
        /*[MenuItem("CONTEXT/ComponentAdapter/Locate Style Mapper")]
        static void LocateStyleMapperCommand(MenuCommand command)
        {
            //ComponentAdapter adapter = (ComponentAdapter)command.context;
            //Debug.Log("ComponentAdapter: " + adapter);
            //_shouldLocateMapper = true;
        }*/

        #endregion

        #region Constants

// ReSharper disable InconsistentNaming
        //internal const string PADDING = "padding";
        //internal const string MARGINS = "margins";
        internal const string GUID = "guid";
        internal const string INSTANCE_ID = "instance_id";
        internal const string COLORS = "colors";
        internal const string SIZING = "sizing";
        internal const string CONSTRAINS = "constrains";
// ReSharper restore InconsistentNaming

        #endregion
        
        #region Properties

        public const int GlobalMaxWidth = 2000;
        public const int GlobalMaxHeight = 2000;

        //public SerializedProperty Guid;
        public SerializedProperty Id;
        public SerializedProperty Depth;
        public SerializedProperty FactoryMode;
        public SerializedProperty Tooltip;
        public SerializedProperty SkinClass;
        public SerializedProperty StyleName;
        public SerializedProperty X;
        public SerializedProperty Y;
        public SerializedProperty Width;
        public SerializedProperty Height;
        public SerializedProperty MinWidth;
        public SerializedProperty MinHeight;
        public SerializedProperty MaxWidth;
        public SerializedProperty MaxHeight;
        public SerializedProperty UsePercentWidth;
        public SerializedProperty UsePercentHeight;
        public SerializedProperty UseX;
        public SerializedProperty UseY;
        public SerializedProperty UseWidth;
        public SerializedProperty UseHeight;
        public SerializedProperty UseLeft;
        public SerializedProperty UseRight;
        public SerializedProperty UseTop;
        public SerializedProperty UseBottom;
        public SerializedProperty Left;
        public SerializedProperty Right;
        public SerializedProperty Top;
        public SerializedProperty Bottom;
        public SerializedProperty UseHorizontalCenter;
        public SerializedProperty UseVerticalCenter;
        public SerializedProperty HorizontalCenter;
        public SerializedProperty VerticalCenter;
        /*public SerializedProperty SyncPadding;
        public SerializedProperty PaddingLeft;
        public SerializedProperty PaddingRight;
        public SerializedProperty PaddingTop;
        public SerializedProperty PaddingBottom;*/

        /*public SerializedProperty SyncMargins;
        public SerializedProperty MarginLeft;
        public SerializedProperty MarginRight;
        public SerializedProperty MarginTop;
        public SerializedProperty MarginBottom;*/

        public SerializedProperty Visible;
        public SerializedProperty IncludeInLayout;
        public SerializedProperty Enabled;
        public SerializedProperty MouseEnabled;
        public SerializedProperty MouseChildren;
        public SerializedProperty FocusEnabled;
        public SerializedProperty HighlightOnFocus;

        public SerializedProperty UseColor;
        public SerializedProperty Color;
        public SerializedProperty ContentColor;
        public SerializedProperty BackgroundColor;
        public SerializedProperty Alpha;

        public SerializedProperty NavigatorDescriptor;

        public SerializedProperty Rotation;
        public SerializedProperty RotationPivot;

        public SerializedProperty Scale;

        #endregion

        #region Members

        //private SerializedProperty _showMargins;
    
        /*private int _prevPaddingLeft;
        private int _prevPaddingRight;
        private int _prevPaddingTop;
        private int _prevPaddingBottom;*/

        /*private int _prevMarginLeft;
        private int _prevMarginRight;
        private int _prevMarginTop;
        private int _prevMarginBottom;*/

        private int _prevDepth;

#if DEBUG
        private readonly InsetRenderer _debugRenderer = new InsetRenderer();
#endif

        private readonly PanelRenderer _sizingPanelRenderer = new PanelRenderer();
        private readonly PanelRenderer _constrainsPanelRenderer = new PanelRenderer();
        //private readonly PanelRenderer _paddingPanelRenderer = new PanelRenderer();

        private readonly InsetRenderer _insetColorRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetXRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetYRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetExplicitWidthRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetExplicitHeightRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetLeftRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetRightRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetTopRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetBottomRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetHorizontalCenterRenderer = new InsetRenderer();
        private readonly InsetRenderer _insetVerticalCenterRenderer = new InsetRenderer();
        
        private List<string> _availableSkinClasses = new List<string>();
        private int _skinIndex;
        private int _skinTemp;

        /// <summary>
        /// A flag indicating that the style mapper specified with ID (via the textfield) hasn't been found in the scene
        /// </summary>
        private bool _cannotFind;

        #endregion

        protected override void Initialize()
        {
            EditorState.SerializedObject = serializedObject;
        
            Id = serializedObject.FindProperty("Id");
            Depth = serializedObject.FindProperty("Depth");
            FactoryMode = serializedObject.FindProperty("FactoryMode");

            Tooltip = serializedObject.FindProperty("Tooltip");
            SkinClass = serializedObject.FindProperty("SkinClass");
            StyleName = serializedObject.FindProperty("StyleName");
            X = serializedObject.FindProperty("X");
            Y = serializedObject.FindProperty("Y");
            Width = serializedObject.FindProperty("Width");
            Height = serializedObject.FindProperty("Height");
            MinWidth = serializedObject.FindProperty("MinWidth");
            MinHeight = serializedObject.FindProperty("MinHeight");
            MaxWidth = serializedObject.FindProperty("MaxWidth");
            MaxHeight = serializedObject.FindProperty("MaxHeight");
            UsePercentWidth = serializedObject.FindProperty("UsePercentWidth");
            UsePercentHeight = serializedObject.FindProperty("UsePercentHeight");
            UseX = serializedObject.FindProperty("UseX");
            UseY = serializedObject.FindProperty("UseY");
            UseWidth = serializedObject.FindProperty("UseWidth");
            UseHeight = serializedObject.FindProperty("UseHeight");

            UseLeft = serializedObject.FindProperty("UseLeft");
            UseRight = serializedObject.FindProperty("UseRight");
            UseTop = serializedObject.FindProperty("UseTop");
            UseBottom = serializedObject.FindProperty("UseBottom");
            Left = serializedObject.FindProperty("Left");
            Right = serializedObject.FindProperty("Right");
            Top = serializedObject.FindProperty("Top");
            Bottom = serializedObject.FindProperty("Bottom");
            UseHorizontalCenter = serializedObject.FindProperty("UseHorizontalCenter");
            UseVerticalCenter = serializedObject.FindProperty("UseVerticalCenter");
            HorizontalCenter = serializedObject.FindProperty("HorizontalCenter");
            VerticalCenter = serializedObject.FindProperty("VerticalCenter");

            /*SyncPadding = serializedObject.FindProperty("SyncPadding");
            PaddingLeft = serializedObject.FindProperty("PaddingLeft");
            PaddingRight = serializedObject.FindProperty("PaddingRight");
            PaddingTop = serializedObject.FindProperty("PaddingTop");
            PaddingBottom = serializedObject.FindProperty("PaddingBottom");*/

            /*SyncMargins = serializedObject.FindProperty("SyncMargins");
            MarginLeft = serializedObject.FindProperty("MarginLeft");
            MarginRight = serializedObject.FindProperty("MarginRight");
            MarginTop = serializedObject.FindProperty("MarginTop");
            MarginBottom = serializedObject.FindProperty("MarginBottom");*/

            UseColor = serializedObject.FindProperty("UseColor");
            Color = serializedObject.FindProperty("Color");
            ContentColor = serializedObject.FindProperty("ContentColor");
            BackgroundColor = serializedObject.FindProperty("BackgroundColor");
            Alpha = serializedObject.FindProperty("Alpha");

            //_showMargins = serializedObject.FindProperty("ShowMargins");

            Visible = serializedObject.FindProperty("Visible");
            IncludeInLayout = serializedObject.FindProperty("IncludeInLayout");
            Enabled = serializedObject.FindProperty("Enabled");
            MouseEnabled = serializedObject.FindProperty("MouseEnabled");
            MouseChildren = serializedObject.FindProperty("MouseChildren");
            FocusEnabled = serializedObject.FindProperty("FocusEnabled");
            HighlightOnFocus = serializedObject.FindProperty("HighlightOnFocus");
            NavigatorDescriptor = serializedObject.FindProperty("NavigatorDescriptor");
            Rotation = serializedObject.FindProperty("Rotation");
            RotationPivot = serializedObject.FindProperty("RotationPivot");
            Scale = serializedObject.FindProperty("Scale");

            /*_prevPaddingLeft = PaddingLeft.intValue;
            _prevPaddingRight = PaddingRight.intValue;
            _prevPaddingTop = PaddingTop.intValue;
            _prevPaddingBottom = PaddingBottom.intValue;*/

            /*_prevMarginLeft = MarginLeft.intValue;
            _prevMarginRight = MarginRight.intValue;
            _prevMarginTop = MarginTop.intValue;
            _prevMarginBottom = MarginBottom.intValue;*/

            // load skins now
            LoadAvailableSkins(AdapterAnalysis.ComponentAdapter);

            var skinClass = AdapterAnalysis.SkinClass;
            //if (null != AdapterAnalysis.SkinnableComponentAdapter)
            if (!string.IsNullOrEmpty(skinClass))
            {
                // skins
                FindSelectedSkinClassIndex(AdapterAnalysis.ComponentAdapter, skinClass);

                //// style mappers
                //FindSelectedStyleMapperIndex(AdapterAnalysis.ComponentAdapter, AdapterAnalysis.ComponentAdapter.StyleName);
            }
        }

        /// <summary>
        /// Applies the changed properties to component, using the Apply method of component descriptor
        /// </summary>
        //protected void Apply()
        //{
        //    if (null != target)
        //    {
        //        //Debug.Log("Apply: " + this);
        //        var t = (IComponentAdapter)target;
        //        //Debug.Log("t: " + t);
        //        //Debug.Log("t.Component: " + t.Component);
        //        if (null != t.Component)
        //            t.Apply(t.Component);
        //    }

        //    _isDirty = false;
        //}
        
        protected override void RenderMainOptions()
        {
#if DEBUG
            EditorSettings.ShowDebugOptionsInInspector = _debugRenderer.RenderStart("Debug", EditorSettings.ShowDebugOptionsInInspector);
            if (EditorSettings.ShowDebugOptionsInInspector)
            {
                if (!IsHidden(INSTANCE_ID))
                {
                    if (null != target)
                    {
                        EditorSettings.ShowUniqueIdInInspector = EditorGUILayout.Foldout(EditorSettings.ShowUniqueIdInInspector, "InstanceID");
                        EditorGUI.indentLevel = 1;
                        if (EditorSettings.ShowUniqueIdInInspector)
                        {
                            GUILayout.BeginHorizontal();

                            ComponentAdapter adapter = target as ComponentAdapter;
                            string transformId = (null == adapter) ? "-" : adapter.transform.GetInstanceID().ToString();

                            EditorGUILayout.HelpBox("Transform: " + transformId, MessageType.None);

                            EditorGUILayout.HelpBox("Adapter: " + target.GetInstanceID(), MessageType.None);

                            GUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel = 0;
                    }
                }

                //if (!IsHidden(GUID))
                //{
                //    EditorSettings.ShowGuidInInspector = EditorGUILayout.Foldout(EditorSettings.ShowGuidInInspector, "Guid");
                //    EditorGUI.indentLevel = 1;
                //    if (EditorSettings.ShowGuidInInspector)
                //    {
                //        EditorGUILayout.HelpBox(Guid.stringValue, MessageType.None);
                //    }
                //    EditorGUI.indentLevel = 0;
                //}

                _debugRenderer.RenderEnd();
            }
#endif
            
            FactoryMode.boolValue = GUILayout.Toggle(FactoryMode.boolValue,
                                                     new GUIContent("Factory Mode", FactoryMode.boolValue ? TextureCache.Instance.FactoryEnabled : TextureCache.Instance.FactoryDisabled),
                                                     StyleCache.Instance.GreenToggle2, GUILayout.Height(30));

            //if (GUILayout.Button("Print component descriptor GUID", StyleCache.Instance.Button, GUILayout.Height(30)))
            //{
            //    Debug.Log(serializedObject.FindProperty("Guid").stringValue);
            //}

            Id.stringValue = EditorGUILayout.TextField("Id", Id.stringValue, GUILayout.ExpandWidth(true));

            _prevDepth = Depth.intValue;
            Depth.intValue = EditorGUILayout.IntSlider("Depth", Depth.intValue, -1000, 1000); // EditorGUILayout.IntField("Depth", Depth.intValue);
            if (Depth.intValue != _prevDepth)
            {
                EditorState.Instance.Depth = Depth.intValue;
            }

            if (FactoryMode.boolValue && string.IsNullOrEmpty(Id.stringValue))
                EditorGUILayout.HelpBox(@"Component most probably needs an identifier when working in factory mode.", MessageType.Warning, true);

            Visible.boolValue = EditorGUILayout.Toggle("Visible", Visible.boolValue);

            IncludeInLayout.boolValue = EditorGUILayout.Toggle("Include in layout", IncludeInLayout.boolValue);
            Enabled.boolValue = EditorGUILayout.Toggle("Enabled", Enabled.boolValue);
            MouseEnabled.boolValue = EditorGUILayout.Toggle("Mouse enabled", MouseEnabled.boolValue);

            MouseChildren.boolValue = EditorGUILayout.Toggle("Mouse children", MouseChildren.boolValue);
            FocusEnabled.boolValue = EditorGUILayout.Toggle("Focus enabled", FocusEnabled.boolValue);
            HighlightOnFocus.boolValue = EditorGUILayout.Toggle("Highlight on focus", HighlightOnFocus.boolValue);

            RenderSkinClassBlock();

            RenderStyleNameBlock();

            Tooltip.stringValue = EditorGUILayout.TextField("Tooltip", Tooltip.stringValue);
            NavigatorDescriptor.stringValue = EditorGUILayout.TextField("Navigator descriptor", NavigatorDescriptor.stringValue);

            Rotation.floatValue = EditorGUILayout.Slider("Rotation", Rotation.floatValue, -360, 360);

            var oldEnabled = GUI.enabled;
            GUI.enabled = Rotation.floatValue != 0;

            var newPivot = EditorGUILayout.Vector2Field("Rotation pivot", RotationPivot.vector2Value);
            if (RotationPivot.vector2Value != newPivot)
            {
                RotationPivot.vector2Value = new Vector2(
                    Mathf.Clamp(newPivot.x, 0f, 1f),
                    Mathf.Clamp(newPivot.y, 0f, 1f)
                );
            }

            GUI.enabled = oldEnabled;

            Scale.vector2Value = EditorGUILayout.Vector2Field("Scale", Scale.vector2Value);

            if (!IsHidden(COLORS))
            {
                UseColor.boolValue = _insetColorRenderer.RenderStart("Colors", UseColor.boolValue);
                if (UseColor.boolValue)
                {
                    Color.colorValue = EditorGUILayout.ColorField("Color", Color.colorValue);
                    ContentColor.colorValue = EditorGUILayout.ColorField("ContentColor", ContentColor.colorValue);
                    BackgroundColor.colorValue = EditorGUILayout.ColorField("BackgroundColor", BackgroundColor.colorValue);

                    _insetColorRenderer.RenderEnd();
                }
            }

            Alpha.floatValue = EditorGUILayout.Slider("Alpha", Alpha.floatValue, 0, 1);

            //RenderStyleMapperBlock();
        }

        protected override void RenderExtendedOptions()
        {
            if (!IsHidden(SIZING))
            {
                EditorSettings.ComponentDescriptorSizingExpanded = _sizingPanelRenderer.RenderStart(GuiContentCache.Instance.PanelPositionAndSizingTitle, EditorSettings.ComponentDescriptorSizingExpanded);

                if (EditorSettings.ComponentDescriptorSizingExpanded)
                {
                    if (!AdapterAnalysis.ParentHasAbsoluteLayout)
                        EditorGUILayout.HelpBox("Note: Positioning (X, Y) has no effect unless the parent container has absolute layout", MessageType.Info);

                    UseX.boolValue = _insetXRenderer.RenderStart("X", UseX.boolValue);

                    if (UseX.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        //GUILayout.Label("X", GUILayout.MinWidth(80));
                        X.intValue = EditorGUILayout.IntSlider(string.Empty, X.intValue, 0, 2000);
                        EditorGUILayout.EndHorizontal();
                        _insetXRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    UseY.boolValue = _insetYRenderer.RenderStart("Y", UseY.boolValue);
                    if (UseY.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        //GUILayout.Label("Y", GUILayout.MinWidth(80));
                        Y.intValue = EditorGUILayout.IntSlider(string.Empty, Y.intValue, 0, 2000);
                        EditorGUILayout.EndHorizontal();
                        _insetYRenderer.RenderEnd();
                    }
                        
                    //EditorGUILayout.EndToggleGroup();

                    EditorGUILayout.Space();

                    EditorGUI.indentLevel = 0;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Min Width", GUILayout.MinWidth(80));
                    MinWidth.intValue = EditorGUILayout.IntSlider(string.Empty, MinWidth.intValue, 0, GlobalMaxWidth);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Max Width", GUILayout.MinWidth(80));
                    MaxWidth.intValue = EditorGUILayout.IntSlider(string.Empty, MaxWidth.intValue, 0, GlobalMaxWidth);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    UseWidth.boolValue = _insetExplicitWidthRenderer.RenderStart("Explicit Width", UseWidth.boolValue);
                    //UseWidth.boolValue = GUILayout.Toggle(UseWidth.boolValue,
                    //                                 new GUIContent("Explicit Width", "Explicit Width"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));

                    //UseWidth.boolValue = EditorGUILayout.BeginToggleGroup("Explicit Width", UseWidth.boolValue);
                    if (UseWidth.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Use % of the parent", GUILayout.ExpandWidth(false));
                        UsePercentWidth.boolValue = EditorGUILayout.Toggle(string.Empty, UsePercentWidth.boolValue, GUILayout.MinWidth(80));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(UsePercentWidth.boolValue ? "Percent Width" : "Width", GUILayout.MinWidth(80));
                        Width.intValue = EditorGUILayout.IntSlider(string.Empty, Width.intValue, 0, UsePercentWidth.boolValue ? 100 : GlobalMaxWidth);
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();

                        _insetYRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Min Height", GUILayout.MinWidth(80));
                    MinHeight.intValue = EditorGUILayout.IntSlider(string.Empty, MinHeight.intValue, 0, GlobalMaxHeight);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Max Height", GUILayout.MinWidth(80));
                    MaxHeight.intValue = EditorGUILayout.IntSlider(string.Empty, MaxHeight.intValue, 0, GlobalMaxHeight);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    UseHeight.boolValue = _insetExplicitHeightRenderer.RenderStart("Explicit Height", UseHeight.boolValue);
                    //UseHeight.boolValue = GUILayout.Toggle(UseHeight.boolValue,
                    //                                 new GUIContent("Explicit Height", "Explicit Height"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    //UseHeight.boolValue = EditorGUILayout.BeginToggleGroup("Explicit Height", UseHeight.boolValue);
                    if (UseHeight.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Use % of the parent", GUILayout.ExpandWidth(false));
                        UsePercentHeight.boolValue = EditorGUILayout.Toggle(string.Empty, UsePercentHeight.boolValue);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(UsePercentHeight.boolValue ? "Percent Height" : "Height", GUILayout.MinWidth(80));
                        Height.intValue = EditorGUILayout.IntSlider(string.Empty, Height.intValue, 0, UsePercentHeight.boolValue ? 100 : GlobalMaxHeight);
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();

                        _insetExplicitHeightRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    //EditorGUILayout.Space();
                    GUILayout.Space(1);

                    _sizingPanelRenderer.RenderEnd();
                }
                EditorGUI.indentLevel = 0;
            }

            if (!IsHidden(CONSTRAINS))
            {
                EditorSettings.ComponentDescriptorConstrainsExpanded = _constrainsPanelRenderer.RenderStart(GuiContentCache.Instance.PanelConstrainsTitle, EditorSettings.ComponentDescriptorConstrainsExpanded);

                //_showSizing.boolValue = EditorGUILayout.Foldout(_showSizing.boolValue, "Position and sizing");
                if (EditorSettings.ComponentDescriptorConstrainsExpanded)
                {

                    //Debug.Log("_containerHasAbsoluteLayout: " + _containerHasAbsoluteLayout);
                    if (!AdapterAnalysis.ParentHasAbsoluteLayout)
                        EditorGUILayout.HelpBox("Note: Constrains have no effect unless the parent container has absolute layout", MessageType.Info);

                    UseLeft.boolValue = _insetLeftRenderer.RenderStart("Left", UseLeft.boolValue);
                    //UseLeft.boolValue = GUILayout.Toggle(UseLeft.boolValue,
                    //                                 new GUIContent("Left", "Constrain to left"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    
                    //UseLeft.boolValue = EditorGUILayout.BeginToggleGroup("Left", UseLeft.boolValue);
                    if (UseLeft.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        //EditorGUI.indentLevel = 1;
                        Left.intValue = EditorGUILayout.IntSlider(Left.intValue, -GlobalMaxWidth / 2, GlobalMaxWidth / 2);
                        //EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();

                        _insetLeftRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    UseRight.boolValue = _insetRightRenderer.RenderStart("Right", UseRight.boolValue);
                    //UseRight.boolValue = GUILayout.Toggle(UseRight.boolValue,
                    //                                 new GUIContent("Right", "Constrain to right"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    //UseRight.boolValue = EditorGUILayout.BeginToggleGroup("Right", UseRight.boolValue);
                    if (UseRight.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        //EditorGUI.indentLevel = 1;
                        Right.intValue = EditorGUILayout.IntSlider(Right.intValue, -GlobalMaxWidth / 2, GlobalMaxWidth / 2);
                        //EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();

                        _insetRightRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    UseTop.boolValue = _insetTopRenderer.RenderStart("Top", UseTop.boolValue);
                    //UseTop.boolValue = GUILayout.Toggle(UseTop.boolValue,
                    //                                 new GUIContent("Top", "Constrain to top"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    //UseTop.boolValue = EditorGUILayout.BeginToggleGroup("Top", UseTop.boolValue);
                    if (UseTop.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        //EditorGUI.indentLevel = 1;
                        Top.intValue = EditorGUILayout.IntSlider(Top.intValue, -GlobalMaxHeight / 2, GlobalMaxHeight / 2);
                        //EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();
                        _insetTopRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    UseBottom.boolValue = _insetBottomRenderer.RenderStart("Bottom", UseBottom.boolValue);
                    //UseBottom.boolValue = GUILayout.Toggle(UseBottom.boolValue,
                    //                                 new GUIContent("Bottom", "Constrain to bottom"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    //UseBottom.boolValue = EditorGUILayout.BeginToggleGroup("Bottom", UseBottom.boolValue);
                    if (UseBottom.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        //EditorGUI.indentLevel = 1;
                        Bottom.intValue = EditorGUILayout.IntSlider(Bottom.intValue, -GlobalMaxHeight / 2, GlobalMaxHeight / 2);
                        //EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();
                        _insetBottomRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    UseHorizontalCenter.boolValue = _insetHorizontalCenterRenderer.RenderStart("Horizontal center", UseHorizontalCenter.boolValue);
                    //UseHorizontalCenter.boolValue = GUILayout.Toggle(UseHorizontalCenter.boolValue,
                    //                                 new GUIContent("Horizontal center", "Horizontal center"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    //UseHorizontalCenter.boolValue = EditorGUILayout.BeginToggleGroup("Horizontal center", UseHorizontalCenter.boolValue);
                    if (UseHorizontalCenter.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        //EditorGUI.indentLevel = 1;
                        HorizontalCenter.intValue = EditorGUILayout.IntSlider(HorizontalCenter.intValue, -GlobalMaxWidth / 2, GlobalMaxWidth / 2);
                        //EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();
                        _insetHorizontalCenterRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();

                    UseVerticalCenter.boolValue = _insetVerticalCenterRenderer.RenderStart("Vertical center", UseVerticalCenter.boolValue);
                    //UseVerticalCenter.boolValue = GUILayout.Toggle(UseVerticalCenter.boolValue,
                    //                                 new GUIContent("Vertical center", "Vertical center"),
                    //                                 StyleCache.Instance.GreenToggle, GUILayout.Height(24));
                    //UseVerticalCenter.boolValue = EditorGUILayout.BeginToggleGroup("Vertical center", UseVerticalCenter.boolValue);
                    if (UseVerticalCenter.boolValue)
                    {
                        //EditorGUILayout.BeginVertical(StyleCache.Instance.InsetHeaderCollapsed);

                        //EditorGUI.indentLevel = 1;
                        VerticalCenter.intValue = EditorGUILayout.IntSlider(VerticalCenter.intValue, -GlobalMaxHeight / 2, GlobalMaxHeight / 2);
                        //EditorGUI.indentLevel = 0;

                        //EditorGUILayout.EndVertical();

                        _insetVerticalCenterRenderer.RenderEnd();
                    }
                    //EditorGUILayout.EndToggleGroup();
                    GUILayout.Space(1);

                    _constrainsPanelRenderer.RenderEnd();
                }
                EditorGUI.indentLevel = 0;
            }

            /*if (!IsHidden(PADDING))
            {
                EditorSettings.ComponentDescriptorPaddingExpanded = _paddingPanelRenderer.RenderStart(GuiContentCache.Instance.PanelPaddingTitle, EditorSettings.ComponentDescriptorPaddingExpanded);

                if (EditorSettings.ComponentDescriptorPaddingExpanded && AdapterAnalysis.HasAbsoluteLayout)
                    EditorGUILayout.HelpBox("Note: Padding has no effect if the container has absolute layout", MessageType.Info);

                if (EditorSettings.ComponentDescriptorPaddingExpanded)
                {
                    SynchronizedBlock.Render(SyncPadding, PaddingLeft, PaddingRight, PaddingTop, PaddingBottom, ref _prevPaddingLeft, ref _prevPaddingRight, ref _prevPaddingTop, ref _prevPaddingBottom);
                    EditorGUILayout.Space();
                    _paddingPanelRenderer.RenderEnd();
                }
            }*/

            /*if (!IsHidden(MARGINS))
            {
                _showMargins.boolValue = EditorGUILayout.Foldout(_showMargins.boolValue, "Margins");
                if (_showMargins.boolValue)
                {
                    SynchronizedBlock.Render(SyncMargins, MarginLeft, MarginRight, MarginTop, MarginBottom, ref _prevMarginLeft, ref _prevMarginRight, ref _prevMarginTop, ref _prevMarginBottom);
                    EditorGUILayout.Space();
                }
            }*/
        }

        #region Helper

        private void FindSelectedSkinClassIndex(ComponentAdapter adapter, string value)
        {
            //Dictionary<string, Type> skins = new Dictionary<string, Type>();

            //Debug.Log("FindSelectedStyleMapperIndex: " + value);
            //var skin = adapter.SkinClass;
            //ReflectionUtil.GetSkinClasses(adapter.ComponentType, ref skins);

            LoadAvailableSkins(adapter);

            _skinTemp = 0;
            foreach (string skin in _availableSkinClasses)
            {
                if (skin == value)
                {
                    break;
                }
                _skinTemp++;
            }
            _skinIndex = _skinTemp;
            //Debug.Log("_skinIndex: " + _skinIndex);
            _cannotFind = !string.IsNullOrEmpty(value) && !_availableSkinClasses.Contains(value);
            //Debug.Log("_cannotFind: " + _cannotFind);
        }

        private void LoadAvailableSkins(ComponentAdapter adapter)
        {
            var skins = EditorReflector.GetSkins(adapter.ComponentType).ToList();

#if DEBUG
            if (true)
            {
                StringBuilder sb = new StringBuilder();
                if (skins.Count == 0)
                {
                    sb.AppendLine("No available skins.");
                }
                else
                {
                    /*foreach (KeyValuePair<string, Type> pair in skins)
                    {
                        sb.AppendLine(string.Format("    {0} -> {1}", pair.Key, pair.Value));
                    }*/
                    foreach (var skin in skins)
                    {
                        sb.AppendLine(string.Format("    -> {0}", skin));
                    }
                }

                /*Debug.Log(string.Format(@"====== Skins ======
{0}", sb));*/
            }
#endif
            _availableSkinClasses = new List<string>();
            //if (_couldNotLocateMapper)
            //    list.Add("=== Not found ===");
            //list.Add("= Default =");
            foreach (Type skinClass in skins)
            {
                _availableSkinClasses.Add(skinClass.FullName);
            }
        }

        private void RenderSkinClassBlock()
        {
            string value = null == SkinClass ? string.Empty : SkinClass.stringValue;
            //Debug.Log(string.Format("value1: {0}", value));

            if (_cannotFind)
            {
                EditorGUILayout.HelpBox(string.Format(@"Couldn't locate skin class [{0}]. Using the default class instead.", value), MessageType.Warning, true);
                //value = string.Empty;
            }

            EditorGUILayout.BeginHorizontal();

            string[] skins = _availableSkinClasses.ToArray();

            var oldEnabled = GUI.enabled;
            GUI.enabled = AdapterAnalysis.ComponentAdapter.ComponentType.IsSubclassOf(typeof(SkinnableComponent));

            _skinTemp = EditorGUILayout.Popup(
                "Skin class",
                _skinIndex,
                skins);

            GUI.enabled = oldEnabled;

            if (_skinTemp != _skinIndex)
            {
                _cannotFind = false;
                _skinIndex = _skinTemp;

                value = _availableSkinClasses[_skinIndex];
                //Debug.Log(string.Format("Change: [index: {0}, value: {1}]", _skinIndex, value));
            }

            /*GUILayout.Space(4);

            GUILayout.Label(string.Empty, GUILayout.ExpandWidth(false), GUILayout.Width(40));*/

            EditorGUILayout.EndHorizontal();

            //Debug.Log("value: " + value);
            if (null != SkinClass)
                SkinClass.stringValue = value;
        }

        private void RenderStyleNameBlock()
        {
            string value = StyleName.stringValue;

            value = EditorGUILayout.TextField("Style name", value);

            //Debug.Log("value: " + value);
            StyleName.stringValue = value;
        }

        /*private void LocateStyleMapper(IComponentFactory adapter, int index)
        {
            // TODO: Locate stylesheets effecting this CSS class

            ////Debug.Log("Locating");
            //var componentType = adapter.ComponentType;
            //var mappers = Gui.GetAvailableStyleMappers(componentType);

            //StyleMapperBase mapper;
            //bool defaultSelected = IsDefaultSelected(index);

            //if (defaultSelected)
            //{
            //    // look for default
            //    mapper = mappers.Find(delegate(StyleMapperBase m)
            //    {
            //        return m.Default;
            //    });
            //}
            //else
            //{
            //    // look for named mapper
            //    string mapperId = _availableMappers[_mapperIndex];
            //    mapper = mappers.Find(delegate(StyleMapperBase m)
            //    {
            //        return m.Id == mapperId;
            //    });
            //}

            //if (null != mapper)
            //{
            //    //Debug.Log("Pinging mapper: " + mapper);
            //    Selection.objects = new Object[] { mapper.gameObject };
            //    EditorGUIUtility.PingObject(mapper);
            //}
            //else
            //{
            //    Debug.LogWarning("Cannot locate style mapper");
            //}
        }*/

        #endregion
    }
}