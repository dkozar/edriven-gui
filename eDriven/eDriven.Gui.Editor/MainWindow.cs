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

using System.Collections.Generic;
using System.Reflection;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Display;
using eDriven.Gui.Editor.Persistence;
using eDriven.Gui.Editor.Prerequisites;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [Obfuscation(Exclude = true)]
    internal sealed class MainWindow : EDrivenEditorWindowBase
    {
        #region Init

        [MenuItem("Window/eDriven.Gui/Main")] // eDriven.Gui Designer
// ReSharper disable UnusedMember.Local
        static void Init()
// ReSharper restore UnusedMember.Local
        {
            /**
             * Instantiate window
             * */
            var window = GetWindow(typeof(MainWindow), DipSwitches.IsUtilityWindow, DipSwitches.MainWindowName);
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(DipSwitches.MinWindowWidth, DipSwitches.MinWindowHeight);
            window.wantsMouseMove = true;
            window.Show();

            IconSetter.SetIcon(window);

            _showLogo = true;
        }

        #endregion

        #region Static

#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public new static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

// ReSharper disable InconsistentNaming
        public const int ORDER_DISPLAY = 0;
        public const int LAYOUT_DISPLAY = 1;
        public const int EVENTS_DISPLAY = 2;
// ReSharper restore InconsistentNaming

        #endregion

        #region Members

        /// <summary>
        /// Overal window bounds
        /// </summary>
        private Rect _bounds;
    
        /// <summary>
        /// A flag indicated that Unity finished instantiating components when play mode started
        /// This flag is being set to false on OnEnable() and is being set to true on OnHierarchyChange()
        /// </summary>
        private bool _initialHierarchyChangeDone;

        private static readonly PanelRenderer PanelRenderer = new PanelRenderer
        {
            Collapsible = false,
            Tools = new List<ToolDescriptor>(new[]
            {
                new ToolDescriptor("help", TextureCache.Instance.Help, "Help"),
                new ToolDescriptor("options", TextureCache.Instance.Options, "Options"),
                new ToolDescriptor("info", TextureCache.Instance.Information, "About")
            })
        };

        private static bool _showLogo;
        private Vector2 _logoScrollPosition;
        private bool _shouldProcessSelectionChange;
        private bool _shouldProcessHierarchyChange;
        #endregion
        
        #region Slots
        
        /// <summary>
        /// Fires when the play mode stopped and changes were already submitted to edit-mode objects
        /// </summary>
        /// <param name="parameters"></param>
        private void ChangesAppliedSlot(object[] parameters)
        {
            ProcessSelectionChange();
        }

        private static void HierarchyWindowButtonClickedSlot(object[] parameters)
        {
            string msg = (string) parameters[1];
            if (msg == "event")
                MainTabBar.Instance.TabIndex = 2;
        }

        // ReSharper disable UnusedMember.Local
        //[Obfuscation(Exclude = true)]
        void SelectionChangeSlot(params object[] parameters)
        // ReSharper restore UnusedMember.Local
        {
            if (_showLogo)
            {
                _showLogo = false;
            }

            BreadcrumbsMain.Instance.RefreshPath();

            ProcessSelectionChange();
        }

        /// <summary>
        /// Note: for this handler to fire, Main window has to be (always) visible!
        /// </summary>
        void HierarchyChangsProcessedSlot(params object[] parameters)
        {
            _initialHierarchyChangeDone = true;
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("MainWindow->HierarchyChangsProcessedSlot");
            }
#endif
            //        Debug.Log(@"######### OnHierarchyChange. 
            //EditorSettings.ReadyToProcessHierarchyChanges: " + EditorSettings.ReadyToProcessHierarchyChanges);

            if (_showLogo)
            {
                _showLogo = false;
            }

            if (!EditorSettings.WatchChanges) // not watching for changes, return
                return;

            /*/**
             * 3. If second level loaded, go no further - do not process any changes
             * #1#
            if (_levelLoaded)
            {
                return;
            }*/

            /**
             * Update displays
             * (children displays after the new child addition or removal etc.)
             * */
            // process selection change
            ProcessSelectionChange();

            // process hierarchy change
            ProcessHierarchyChange();
        }

        #endregion

        #region ###### Native Unity handlers ######

        /*void Awake()
        {
            Debug.Log("MainWindow->Awake");
            EditorState.Instance.OnEnableSignal.Connect(OnEnableSlot);
            EditorState.Instance.OnDisableSignal.Connect(OnDisableSlot);
        }*/

// ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            IconSetter.SetIcon(this);
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("*** OnEnable ***");
            }
#endif
            // process the post script creation addition (runs after the compile)
            //CreatedScriptAdditionProcessor.Instance.Process();
            PostCompileScriptProcessor.Instance.Process();

            /**
             * Initialize the PlayModeStateChangeEmitter instance
             * We are subscribing to it's ChangesAppliedSignal to react when changes applied
             * so we could then push changes to views (events, children and layout view)
             * */
            PlayModeStateChangeEmitter.Instance.ChangesAppliedSignal.Connect(ChangesAppliedSlot); // a single slot connects only once, i.e. no need to vorry about the duplication

            HierarchyChangeProcessor.Instance.ChangesProcessedSignal.Connect(HierarchyChangsProcessedSlot);

            /**
             * Subscribe to ButtonClickedSignal of the HierarchyViewDecorator
             * */
            HierarchyViewDecorator.Instance.ButtonClickedSignal.Connect(HierarchyWindowButtonClickedSlot);

            /**
             * Check prerequisites for displaying play mode overlay
             * */
            if (_initialHierarchyChangeDone) {
                if (EditorSettings.InspectorEnabled)
                    OverlayPrerequisitesChecker.Check();
            }
        
            //SerializationChangesUpdater.Instance.Process();

            /**
             * 3. Set the flag to false
             * Will be set to true from within OnHierarchyChange
            * */
            _initialHierarchyChangeDone = false;

            /**
             * 4. 
             * */
            DesignModeStrategy.Start();

            /**
             * 5. Initialize the toolbox
             * */
            ToolboxInitializer.Initialize();

            /**
             * Selection change
             * */
            EditorState.Instance.SelectionChangeSignal.Connect(SelectionChangeSlot);

            /**
             * 6. Subscribes to game-view component double-clicks
             * */
            //DesignerOverlay.DoubleClickSignal.Connect(DoubleClickSlot);

            /**
             * Update bounds
             * */
            HandleBounds(_bounds);
        }

// ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("*** OnDisable ***");
            }
#endif
            // stop design mode strategy
            DesignModeStrategy.Stop();
        }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnLostFocus()
// ReSharper restore UnusedMember.Local
        {
            EventDisplay.Instance.OnLostFocus();
            OrderDisplay.Instance.OnLostFocus();
            LayoutDisplay.Instance.OnLostFocus();
        }

// ReSharper disable UnusedMember.Local
        /// <summary>
        /// Renders the controls allowed for instantiation in relation to selected parent
        /// </summary>
        [Obfuscation(Exclude = true)]
        void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            // Note: properties have to be commited prior to Render
            // or else, we get the "ArgumentException: Getting control 2's position in a group with only 2 controls when doing Repaint" error
            if (_propertiesInvalidated)
                CommitProperties();

            EditorWindowContentWrapper.Start();

            if (null == PanelRenderer.ChromeStyle)
                PanelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;

            PanelRenderer.RenderStart(GuiContentCache.Instance.MainPanelTitle, true);

            if (PanelRenderer.ClickedTools.Count > 0)
            {
                if (PanelRenderer.ClickedTools.Contains("info"))
                {
                    PanelRenderer.ClickedTools.Remove("info");
                    AboutDialog.Instance.ShowUtility(); //ShowAuxWindow();
                }
                if (PanelRenderer.ClickedTools.Contains("options"))
                {
                    PanelRenderer.ClickedTools.Remove("options");
                    OptionsDialog.Instance.ShowUtility();
                }
            }

            #region Handling bounds

// ReSharper disable CompareOfFloatsByEqualityOperator
            if (_bounds.width != position.width || _bounds.height != position.height)
// ReSharper restore CompareOfFloatsByEqualityOperator
            {
                HandleBounds(position);
                _bounds = position;
                //Toolbox.Instance.Width = position.width;
            }

            #endregion

            /**
             * 1. Help
             * */
            if (PanelRenderer.ClickedTools.Contains("help"))
            {
                // info image
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent(TextureCache.Instance.PlayModeGameViewInfo), GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(1);

                EditorGUILayout.HelpBox(Help.Main, MessageType.Info, true);
            }

            /**
             * 2. Toolbar with buttons
             * */
            Toolbar.Instance.Render();
            GUILayout.Space(5);
        
            /**
             * 3. Breadcrumbs
             * */
            if (null != Selection.activeGameObject)
            {
                BreadcrumbsMain.Instance.Render();
                GUILayout.Space(5);
            }

            /**
             * 4. Tabs
             * */
            MainTabBar.Instance.Render();

            /**
             * 5. Display
             * */
            EditorGUILayout.BeginVertical(StyleCache.Instance.TabBackground, GUILayout.ExpandHeight(true));

            if (_showLogo)
            {
                _logoScrollPosition = EditorGUILayout.BeginScrollView(_logoScrollPosition);
                GUILayout.Label(TextureCache.Instance.Logo, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
            else
            {
                switch (MainTabBar.Instance.TabIndex)
                {
                    case ORDER_DISPLAY:
                        OrderDisplay.Instance.Render();
                        break;
                    case LAYOUT_DISPLAY:
                        LayoutDisplay.Instance.Render();
                        break;
                    case EVENTS_DISPLAY:
                        EventDisplay.Instance.Render();
                        break;
                    
                }
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(2);

            PanelRenderer.RenderEnd();

            EditorWindowContentWrapper.End();

            if (GUI.changed && _showLogo)
            {
                _showLogo = false;
            }

            if (UpdateCheck.Instance.DataReady)
                UpdateCheck.Instance.ShowDialog();
        }
// ReSharper disable UnusedMember.Local
        protected override void UpdateHandler()
        // ReSharper restore UnusedMember.Local
        {
            switch (MainTabBar.Instance.TabIndex)
            {
                case ORDER_DISPLAY:
                    OrderDisplay.Instance.Update();
                    break;
                case LAYOUT_DISPLAY:
                    LayoutDisplay.Instance.Update();
                    break;
                case EVENTS_DISPLAY:
                    EventDisplay.Instance.Update();
                    break;
                
            }

            // fixes the choppines when dragging things in edit mode
            if (!Application.isPlaying)
                Repaint();
        }

        private static void HandleBounds(Rect rect)
        {
            Rectangle rectangle = Rectangle.FromRect(rect);
            Rect bounds = rectangle.Collapse(10, 10, 0, 13).ToRect();

            EventDisplay.Instance.Bounds = bounds;
            OrderDisplay.Instance.Bounds = bounds;
            LayoutDisplay.Instance.Bounds = bounds;
        }

        #endregion

        #region Commit properties

        private bool _propertiesInvalidated;

        private void InvalidateProperties()
        {
            _propertiesInvalidated = true;
        }

        private void CommitProperties()
        {
            _propertiesInvalidated = false;
            if (_shouldProcessSelectionChange)
            {
                _shouldProcessSelectionChange = false;
                EventDisplay.Instance.ProcessSelectionChange();
                OrderDisplay.Instance.ProcessSelectionChange();
                LayoutDisplay.Instance.ProcessSelectionChange();
            }

            if (_shouldProcessHierarchyChange)
            {
                _shouldProcessHierarchyChange = false;
                EventDisplay.Instance.ProcessHierarchyChange();
                OrderDisplay.Instance.ProcessHierarchyChange();
                LayoutDisplay.Instance.ProcessHierarchyChange();
            }
        }

        #endregion

        #region Helper
    
        /// <summary>
        /// Being run each time we need to update our displays
        /// </summary>
        private void ProcessSelectionChange()
        {
            _shouldProcessSelectionChange = true;
            InvalidateProperties();
        }

        private void ProcessHierarchyChange()
        {
            _shouldProcessHierarchyChange = true;
            InvalidateProperties();
        }
        
        #endregion
    }
}