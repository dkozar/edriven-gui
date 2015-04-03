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

using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class TextureCache
    {
        #region Singleton

        private static TextureCache _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private TextureCache()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static TextureCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TextureCache();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        public static string EditorIconsPath = "eDriven/Editor/Icons/";
        public static string LogoSmallPath = "eDriven/Editor/Logo/logo_small";
        public static string LogoPath = "eDriven/Editor/Logo/logo";
        public static string LogoPath2 = "eDriven/Editor/Logo/logo2";
        
        public Texture MainPanelTitleIcon { get; private set; }
        public Texture DebugPanelTitleIcon { get; private set; }
        public Texture HelpPanelTitleIcon { get; private set; }
        public Texture LogoSmall { get; private set; }
        public Texture Logo { get; private set; }
        public Texture Logo2 { get; private set; }
        public Texture AutoSave { get; private set; }
        public Texture WatchChanges { get; private set; }
        public Texture PlayModeGameViewInfo { get; private set; }

#if RELEASE
        public Texture Purchase { get; private set; }
#endif

        public Texture WindowTitleIcon { get; private set; }
        public Texture Save { get; private set; }
        public Texture Refresh { get; private set; }
        public Texture AutoUpdateOn { get; private set; }
        public Texture AutoUpdateOff { get; private set; }
        public Texture Inspect { get; private set; }
        public Texture InspectComponent { get; private set; }
        //public Texture OptionsS { get; private set; }
        public Texture Options { get; private set; }
        public Texture FixHierarchy { get; private set; }
        public Texture OrderSelected { get; private set; }
        public Texture OrderNormal { get; private set; }
        public Texture Depth { get; private set; }
        public Texture Add { get; private set; }
        public Texture Remove { get; private set; }
        public Texture Edit { get; private set; }
        public Texture Lock { get; private set; }
        public Texture EditScript { get; private set; }
        public Texture RemoveAll { get; private set; }
        public Texture RemoveAllHandlers { get; private set; }
        public Texture Select { get; private set; }
        //public Texture DescriptorLayout { get; private set; }
        public Texture Layout { get; private set; }
        public Texture LayoutNormal { get; private set; }
        public Texture LayoutAbsoluteOn { get; private set; }
        public Texture LayoutAbsoluteOff { get; private set; }
        public Texture LayoutHorizontalOn { get; private set; }
        public Texture LayoutHorizontalOff { get; private set; }
        public Texture LayoutVerticalOn { get; private set; }
        public Texture LayoutVerticalOff { get; private set; }
        public Texture LayoutTileOn { get; private set; }
        public Texture LayoutTileOff { get; private set; }
        public Texture FullLayout { get; private set; }
        public Texture DragHandle { get; private set; }
        public Texture Exclamation { get; private set; }
        public Texture Play { get; private set; }
        public Texture Pause { get; private set; }
        public Texture Event { get; private set; }
        public Texture Events { get; private set; }
        public Texture EventsNormal { get; private set; }
        public Texture EventHandlerAddMapping { get; private set; }
        public Texture EventHandlerAddButton { get; private set; }
        public Texture EventHandlerCreateButton { get; private set; }
        public Texture EventHandlerScriptAddButton { get; private set; }
        public Texture EventHandlerScriptCreateButton { get; private set; }
        public Texture EventsEnabled { get; private set; }
        public Texture EventsDisabled { get; private set; }
        public Texture Cancel { get; private set; }
        public Texture EventPhaseCapture { get; private set; }
        public Texture EventPhaseTarget { get; private set; }
        public Texture EventPhaseBubbling { get; private set; }
        public Texture EventPhaseCaptureOn { get; private set; }
        public Texture EventPhaseTargetOn { get; private set; }
        public Texture EventPhaseBubblingOn { get; private set; }
        public Texture EventPhases { get; private set; }
        public Texture EventPhasesOn { get; private set; }
        public Texture EventFlowEnabled { get; private set; }
        public Texture EventFlowDisabled { get; private set; }
        public Texture EventHandler { get; private set; }
        public Texture EventHandlerScript { get; private set; }
        public Texture EventDoubleClick { get; private set; }
        public Texture EventDoubleClickSelected { get; private set; }
        public Texture FactoryEnabled { get; private set; }
        public Texture FactoryDisabled { get; private set; }
        public Texture Scrolling { get; private set; }
        public Texture Help { get; private set; }
        public Texture ListOff { get; private set; }
        public Texture ListOn { get; private set; }
        public Texture Information { get; private set; }
        public Texture Properties { get; private set; }
        public Texture StyleSheet { get; private set; }
        public Texture StyleSheetAdd { get; private set; }
        public Texture StyleDeclaration { get; private set; }
        public Texture StyleDeclarationNotApplied { get; private set; }
        public Texture StyleDeclarationAdd { get; private set; }
        public Texture Expand { get; private set; }
        public Texture Collapse { get; private set; }
        public Texture Sizing { get; private set; }
        public Texture Constrains { get; private set; }
        public Texture Padding { get; private set; }
        public Texture ToolsGroup { get; private set; }
        public Texture ControlBarGroup { get; private set; }
        public Texture DataProvider { get; private set; }
        public Texture Accept { get; private set; }
        public Texture InsetCollapsed { get; private set; }
        public Texture InsetExpanded { get; private set; }
        public Texture SymbolComponentAdapter { get; private set; }
        public Texture BulletGui { get; private set; }
        public Texture BulletGuiInspector { get; private set; }
        public Texture BulletFontMapper { get; private set; }
        public Texture BulletStyleSheet { get; private set; }
        public Texture BulletAudioPlayerMapper { get; private set; }
        public Texture ScriptJavascript { get; private set; }
        public Texture ScriptCSharp { get; private set; }
        public Texture ScriptBoo { get; private set; }
        public Texture ScriptEventHandlerUsed { get; private set; }
        //public Texture Locate { get; private set; }
        public Texture LocateStyleMapper { get; private set; }
        public Texture WizardPrevious { get; private set; }
        public Texture WizardNext { get; private set; }
        public Texture WizardFinish { get; private set; }
        public Texture Search { get; private set; }
        public Texture InputMode { get; private set; }
        public Texture InputModeOn { get; private set; }
        public Texture Error { get; private set; }
        public Texture GroupRowArrow { get; private set; }
        public Texture LightBulbOff { get; private set; }
        public Texture LightBulbOn { get; private set; }
        public Texture On { get; private set; }
        public Texture Off { get; private set; }
        public Texture CheckForUpdates { get; private set; }
        public Texture CheckForUpdatesLink { get; private set; }

        public Texture RemoveGhostObjects { get; private set; }

        public Texture UnityComponent { get; private set; }
        //public Texture UnityComponentS { get; private set; }
        public Texture Component { get; private set; }
        //public Texture ComponentS { get; private set; }
        public Texture SkinnableComponent { get; private set; }
        //public Texture SkinnableComponentS { get; private set; }
        public Texture Skin { get; private set; }
        //public Texture SkinS { get; private set; }
        
        public Texture Style { get; private set; }
        public Texture Property { get; private set; }
        public Texture MediaQuery { get; private set; }

        public Texture MediaQueryPass { get; private set; }
        public Texture MediaQueryFail { get; private set; }

        public Texture MoveUp { get; private set; }
        public Texture MoveDown { get; private set; }
        public Texture Copy { get; private set; }
        public Texture AbandonChanges { get; private set; }
        public Texture Back { get; private set; }

        public Texture ComponentOn { get; private set; }
        public Texture ComponentOff { get; private set; }
        public Texture SkinnableComponentOn { get; private set; }
        public Texture SkinnableComponentOff { get; private set; }
        public Texture SkinOn { get; private set; }
        public Texture SkinOff { get; private set; }
        public Texture SkinPartOn { get; private set; }
        public Texture SkinPartOff { get; private set; }
        public Texture SkinStateOn { get; private set; }
        public Texture SkinStateOff { get; private set; }
        public Texture SignalOn { get; private set; }
        public Texture SignalOff { get; private set; }

        public Texture EventOn { get; private set; }
        public Texture EventOff { get; private set; }
        public Texture MulticastDelegateOn { get; private set; }
        public Texture MulticastDelegateOff { get; private set; }
        public Texture StyleOn { get; private set; }
        public Texture StyleOff { get; private set; }
        
        public Texture Reference { get; private set; }

        public Texture GuiEditor { get; private set; }
        public Texture GuiInspectorEditor { get; private set; }
        public Texture FontMapperEditor { get; private set; }
        public Texture AudioPlayerEditor { get; private set; }

        public Texture WriteToLog { get; private set; }
        
        //        public Texture EventTarget { get; private set; }
//        public Texture EventBubbling { get; private set; }

        public static void Reset()
        {
            _instance = null;
        }

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            var isDarkSkin = EditorSettings.UseDarkSkin;
            string prefix = "Light/";
            if (isDarkSkin)
                prefix = "Dark/";

            LogoSmall = (Texture) Resources.Load(LogoSmallPath);
            Logo = (Texture)Resources.Load(LogoPath);
            Logo2 = (Texture)Resources.Load(LogoPath2);
            AutoSave = (Texture)Resources.Load(EditorIconsPath + "disk_multiple");
            
#if RELEASE
            Purchase = (Texture)Resources.Load(EditorIconsPath + "purchase");
#endif
            WindowTitleIcon = (Texture)Resources.Load(EditorIconsPath + "edriven_gui_bw");
            MainPanelTitleIcon = (Texture)Resources.Load(EditorIconsPath + "edriven_gui_panel_title_icon");
            DebugPanelTitleIcon = (Texture)Resources.Load(EditorIconsPath + "bug");
            HelpPanelTitleIcon = (Texture)Resources.Load(EditorIconsPath + "help");
            Save = (Texture)Resources.Load(EditorIconsPath + "disk");
            Refresh = (Texture)Resources.Load(EditorIconsPath + "refresh");
            AutoUpdateOn = (Texture)Resources.Load(EditorIconsPath + "auto_update_on");
            AutoUpdateOff = (Texture)Resources.Load(EditorIconsPath + "auto_update_off");
            Inspect = (Texture)Resources.Load(EditorIconsPath + "magnifier");
            Options = (Texture)Resources.Load(EditorIconsPath + "options");
            //OptionsS = (Texture)Resources.Load(EditorIconsPath + "options_s");
            FixHierarchy = (Texture)Resources.Load(EditorIconsPath + "fix_hierarchy"); 
            InspectComponent = (Texture)Resources.Load(EditorIconsPath + "node-magnifier");
            OrderSelected = (Texture)Resources.Load(EditorIconsPath + "order");
            OrderNormal = (Texture)Resources.Load(EditorIconsPath + "order_deselected");
            Depth = (Texture)Resources.Load(EditorIconsPath + "shape_move_forwards");
            Add = (Texture)Resources.Load(EditorIconsPath + "add");
            Remove = (Texture)Resources.Load(EditorIconsPath + "delete");
            Edit = (Texture)Resources.Load(EditorIconsPath + "edit");
            Lock = (Texture)Resources.Load(EditorIconsPath + "lock");
            EditScript = (Texture)Resources.Load(EditorIconsPath + "language_csharp");
            RemoveAll = (Texture)Resources.Load(EditorIconsPath + "table_delete");
            RemoveAllHandlers = (Texture)Resources.Load(EditorIconsPath + "lightning_delete");
            Select = (Texture)Resources.Load(EditorIconsPath + "shape_square_select");
            //DescriptorLayout = (Texture)Resources.Load(EditorIconsPath + "layout_descriptor");
            Layout = (Texture)Resources.Load(EditorIconsPath + "layout");
            LayoutNormal = (Texture)Resources.Load(EditorIconsPath + "layout_deselected");
            LayoutAbsoluteOn = (Texture)Resources.Load(EditorIconsPath + "layout_absolute");
            LayoutAbsoluteOff = (Texture)Resources.Load(EditorIconsPath + "layout_absolute_off");
            LayoutHorizontalOn = (Texture)Resources.Load(EditorIconsPath + "layout_horizontal");
            LayoutHorizontalOff = (Texture)Resources.Load(EditorIconsPath + "layout_horizontal_off");
            LayoutVerticalOn = (Texture)Resources.Load(EditorIconsPath + "layout_vertical");
            LayoutVerticalOff = (Texture)Resources.Load(EditorIconsPath + "layout_vertical_off");
            LayoutTileOn = (Texture)Resources.Load(EditorIconsPath + "layout_tile");
            LayoutTileOff = (Texture)Resources.Load(EditorIconsPath + "layout_tile_off");
            FullLayout = (Texture)Resources.Load(EditorIconsPath + "layout_full");
            DragHandle = (Texture)Resources.Load(EditorIconsPath + "drag_handle");
            Exclamation = (Texture)Resources.Load(EditorIconsPath + "exclamation");
            Play = (Texture)Resources.Load(EditorIconsPath + "control_play");
            Pause = (Texture)Resources.Load(EditorIconsPath + "control_pause");
            Event = (Texture)Resources.Load(EditorIconsPath + "event");
            Events = (Texture)Resources.Load(EditorIconsPath + "events");
            EventsNormal = (Texture)Resources.Load(EditorIconsPath + "events_deselected");
            EventHandlerAddMapping = (Texture)Resources.Load(EditorIconsPath + "lightning_add");
            EventHandlerAddButton = (Texture)Resources.Load(EditorIconsPath + "event_handler");
            EventHandlerCreateButton = (Texture) Resources.Load(EditorIconsPath + "event_handler_create");
            EventHandlerScriptAddButton = (Texture)Resources.Load(EditorIconsPath + "event_handler_script");
            EventHandlerScriptCreateButton = (Texture)Resources.Load(EditorIconsPath + "event_handler_script_create");
            EventsEnabled = (Texture)Resources.Load(EditorIconsPath + "event_flow_multiple");
            EventsDisabled = (Texture)Resources.Load(EditorIconsPath + "event_flow_multiple_disabled");
            Cancel = (Texture)Resources.Load(EditorIconsPath + "cancel");
            EventPhaseCapture = (Texture)Resources.Load(EditorIconsPath + "event_phase_capture");
            EventPhaseTarget = (Texture)Resources.Load(EditorIconsPath + "event_phase_target");
            EventPhaseBubbling = (Texture)Resources.Load(EditorIconsPath + "event_phase_bubbling");
            EventPhaseCaptureOn = (Texture)Resources.Load(EditorIconsPath + "event_phase_capture_on");
            EventPhaseTargetOn = (Texture)Resources.Load(EditorIconsPath + "event_phase_target_on");
            EventPhaseBubblingOn = (Texture)Resources.Load(EditorIconsPath + "event_phase_bubbling_on");
            EventPhases = (Texture)Resources.Load(EditorIconsPath + "event_phases");
            EventPhasesOn = (Texture)Resources.Load(EditorIconsPath + "event_phases_on");
            EventFlowEnabled = (Texture)Resources.Load(EditorIconsPath + "event_flow");
            EventFlowDisabled = (Texture)Resources.Load(EditorIconsPath + "event_flow_disabled");
            EventHandler = (Texture)Resources.Load(EditorIconsPath + "event_handler");
            EventHandlerScript = (Texture)Resources.Load(EditorIconsPath + "script");
            EventDoubleClick = (Texture)Resources.Load(EditorIconsPath + "event_double_click");
            EventDoubleClickSelected = (Texture)Resources.Load(EditorIconsPath + "event_double_click_selected");
            FactoryEnabled = (Texture)Resources.Load(EditorIconsPath + "factory");
            FactoryDisabled = (Texture)Resources.Load(EditorIconsPath + "factory_disabled");
            Scrolling = (Texture)Resources.Load(EditorIconsPath + "scrolling");
            Help = (Texture)Resources.Load(EditorIconsPath + "question");
            ListOff = (Texture)Resources.Load(EditorIconsPath + "star_grey");
            ListOn = (Texture)Resources.Load(EditorIconsPath + "star");
            Information = (Texture)Resources.Load(EditorIconsPath + "information");
            Properties = (Texture)Resources.Load(EditorIconsPath + "properties");
            StyleSheet = (Texture)Resources.Load(EditorIconsPath + "stylesheet");
            StyleSheetAdd = (Texture)Resources.Load(EditorIconsPath + "stylesheet_add");
            StyleDeclaration = (Texture)Resources.Load(EditorIconsPath + "style_declaration");
            StyleDeclarationNotApplied = (Texture)Resources.Load(EditorIconsPath + "style_declaration_not_applied");
            StyleDeclarationAdd = (Texture)Resources.Load(EditorIconsPath + "style_declaration_add");
            Expand = (Texture)Resources.Load(EditorIconsPath + "section_expand");
            Collapse = (Texture)Resources.Load(EditorIconsPath + "section_collapse");
            Sizing = (Texture)Resources.Load(EditorIconsPath + "sizing");
            Constrains = (Texture)Resources.Load(EditorIconsPath + "constrains");
            Padding = (Texture)Resources.Load(EditorIconsPath + "padding");
            ToolsGroup = (Texture)Resources.Load(EditorIconsPath + "group_tools");
            ControlBarGroup = (Texture)Resources.Load(EditorIconsPath + "group_buttons");
            DataProvider = (Texture)Resources.Load(EditorIconsPath + "data_provider");
            Accept = (Texture)Resources.Load(EditorIconsPath + "accept");
            InsetCollapsed = (Texture)Resources.Load(EditorIconsPath + "insed_collapsed");
            InsetExpanded = (Texture)Resources.Load(EditorIconsPath + "inset_expanded");
            SymbolComponentAdapter = (Texture)Resources.Load(EditorIconsPath + "bullet_blue");
            BulletGui = (Texture)Resources.Load(EditorIconsPath + "bullet_green");
            BulletGuiInspector = (Texture)Resources.Load(EditorIconsPath + "bullet_gui_inspector"); 
            BulletFontMapper = (Texture)Resources.Load(EditorIconsPath + "bullet_font");
            BulletStyleSheet = (Texture)Resources.Load(EditorIconsPath + "bullet_style");
            BulletAudioPlayerMapper = (Texture)Resources.Load(EditorIconsPath + "bullet_audio");
            ScriptJavascript = (Texture)Resources.Load(EditorIconsPath + "language_javascript");
            ScriptCSharp = (Texture)Resources.Load(EditorIconsPath + "language_csharp");
            ScriptBoo = (Texture)Resources.Load(EditorIconsPath + "language_boo");
            ScriptEventHandlerUsed = (Texture)Resources.Load(EditorIconsPath + "event");
            //Locate = (Texture)Resources.Load(EditorIconsPath + "magnifier");
            //LocateStyleMapper = (Texture)Resources.Load(EditorIconsPath + "bullet_red");
            WatchChanges = (Texture)Resources.Load(EditorIconsPath + "watch_changes");
            WizardPrevious = (Texture)Resources.Load(EditorIconsPath + "wizard_previous");
            WizardNext = (Texture)Resources.Load(EditorIconsPath + "wizard_next");
            WizardFinish = (Texture)Resources.Load(EditorIconsPath + "wizard_finish");
            Search = (Texture)Resources.Load(EditorIconsPath + "magnifier");
            InputMode = (Texture)Resources.Load(EditorIconsPath + "input_mode");
            InputModeOn = (Texture)Resources.Load(EditorIconsPath + "input_mode_on");
            Error = (Texture)Resources.Load(EditorIconsPath + "error");
            PlayModeGameViewInfo = (Texture)Resources.Load(EditorIconsPath + prefix +  "play_mode_game_view_info");
            GroupRowArrow = (Texture)Resources.Load(EditorIconsPath + "group_arrow_down");
            LightBulbOff = (Texture)Resources.Load(EditorIconsPath + "lightbulb_off");
            LightBulbOn = (Texture)Resources.Load(EditorIconsPath + "lightbulb");
            On = (Texture)Resources.Load(EditorIconsPath + "on");
            Off = (Texture)Resources.Load(EditorIconsPath + "off");
            CheckForUpdates = (Texture)Resources.Load(EditorIconsPath + "check_for_updates");
            CheckForUpdatesLink = (Texture)Resources.Load(EditorIconsPath + "check_for_updates_link");

            RemoveGhostObjects = (Texture)Resources.Load(EditorIconsPath + "ghost");

            UnityComponent = (Texture)Resources.Load(EditorIconsPath + "unity_component");
            //UnityComponentS = (Texture)Resources.Load(EditorIconsPath + "unity_component_s");
            Component = (Texture)Resources.Load(EditorIconsPath + "component");
            //ComponentS = (Texture)Resources.Load(EditorIconsPath + "component_s");
            SkinnableComponent = (Texture)Resources.Load(EditorIconsPath + "skinnable_component");
            //SkinnableComponentS = (Texture)Resources.Load(EditorIconsPath + "skinnable_component_s");
            Skin = (Texture)Resources.Load(EditorIconsPath + "skin");
            //SkinS = (Texture)Resources.Load(EditorIconsPath + "skin_s");
            
            Style = (Texture)Resources.Load(EditorIconsPath + "style");
            Property = (Texture)Resources.Load(EditorIconsPath + "property");
            MediaQuery = (Texture)Resources.Load(EditorIconsPath + "media_query");

            MediaQueryPass = (Texture)Resources.Load(EditorIconsPath + "media_query_pass");
            MediaQueryFail = (Texture)Resources.Load(EditorIconsPath + "media_query_fail");

            MoveUp = (Texture)Resources.Load(EditorIconsPath + "arrow_move_up");
            MoveDown = (Texture)Resources.Load(EditorIconsPath + "arrow_move_down");
            Copy = (Texture)Resources.Load(EditorIconsPath + "copy");
            AbandonChanges = (Texture)Resources.Load(EditorIconsPath + "cancel");
            Back = (Texture)Resources.Load(EditorIconsPath + "back");

            ComponentOn = (Texture)Resources.Load(EditorIconsPath + "component_on");
            ComponentOff = (Texture)Resources.Load(EditorIconsPath + "component_off");
            SkinnableComponentOn = (Texture)Resources.Load(EditorIconsPath + "skinnable_component_on");
            SkinnableComponentOff = (Texture)Resources.Load(EditorIconsPath + "skinnable_component_off");
            SkinOn = (Texture)Resources.Load(EditorIconsPath + "skin_on");
            SkinOff = (Texture)Resources.Load(EditorIconsPath + "skin_off");
            EventOn = (Texture)Resources.Load(EditorIconsPath + "event_on");
            EventOff = (Texture)Resources.Load(EditorIconsPath + "event_off");
            MulticastDelegateOn = (Texture)Resources.Load(EditorIconsPath + "multicast_delegate_on");
            MulticastDelegateOff = (Texture)Resources.Load(EditorIconsPath + "multicast_delegate_off");
            StyleOn = (Texture)Resources.Load(EditorIconsPath + "style_on");
            StyleOff = (Texture)Resources.Load(EditorIconsPath + "style_off");
            SkinPartOn = (Texture)Resources.Load(EditorIconsPath + "skin_part_on");
            SkinPartOff = (Texture)Resources.Load(EditorIconsPath + "skin_part_off");
            SkinStateOn = (Texture)Resources.Load(EditorIconsPath + "states_on");
            SkinStateOff = (Texture)Resources.Load(EditorIconsPath + "states_off");
            SignalOn = (Texture)Resources.Load(EditorIconsPath + "signal_on");
            SignalOff = (Texture)Resources.Load(EditorIconsPath + "signal_off");

            Reference = (Texture)Resources.Load(EditorIconsPath + "reference");
            GuiEditor = (Texture)Resources.Load(EditorIconsPath + "gui_editor");
            GuiInspectorEditor = (Texture)Resources.Load(EditorIconsPath + "gui_inspector");
            FontMapperEditor = (Texture)Resources.Load(EditorIconsPath + "font");
            AudioPlayerEditor = (Texture)Resources.Load(EditorIconsPath + "audio");

            WriteToLog = (Texture)Resources.Load(EditorIconsPath + "write_to_log");

//            EventTarget = (Texture)Resources.Load(EditorIconsPath + "event_with_target");
//            EventBubbling = (Texture)Resources.Load(EditorIconsPath + "event_with_bubbling");
        }
    }
}