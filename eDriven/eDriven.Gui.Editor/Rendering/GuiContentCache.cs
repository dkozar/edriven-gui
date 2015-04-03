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
    internal class GuiContentCache
    {
        #region Singleton

        private static GuiContentCache _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private GuiContentCache()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GuiContentCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GuiContentCache();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        public static string EditorIconsPath = "eDriven/Editor/Icons/";

        public GUIContent InitScene;
        public GUIContent MainPanelTitle;
        public GUIContent HierarchyDebuggerPanelTitle;
        public GUIContent PersistenceDebuggerPanelTitle;
        public GUIContent HelpPanelTitle;
        public GUIContent AddControlPanelTitle;
        public GUIContent AboutPanelTitle;
        public GUIContent OptionsPanelTitle;
        public GUIContent PurchasePanelTitle;
        public GUIContent OptionsFieldsetTitle;
        public GUIContent AutoSave;
        public GUIContent MonitorChanges;
        public GUIContent LiveStyling;
        public GUIContent FixHierarchy;
        public GUIContent Inspect;
        public GUIContent UseDarkSkin;

        public GUIContent Logo1;
        public GUIContent Logo2;
        
#if RELEASE
        public GUIContent Purchase;
        public GUIContent PurchaseButton;
#endif
        //public GUIContent ComponentContent;
        public GUIContent AddChildControlContent;
        public GUIContent SelectContent;
        public GUIContent ExpandWidthContent;
        public GUIContent ExpandHeightContent;
        public GUIContent FactoryModeContent;
        public GUIContent ApplyLastUsedSkinUponCreationContent;
        public GUIContent NoHandlersAttached;
        public GUIContent NoEventsFound;
        public GUIContent NoStyleableClassesFound;
        public GUIContent NoStylePropertiesFound;
        public GUIContent NoStylePropertiesSelected;
        public GUIContent NoMediaQueriesFound;
        public GUIContent NoMediaQueriesSelected;
        public GUIContent NoUpdatesFound;
        public GUIContent NoEventHandlersFound;
        public GUIContent NoComponentsFound;
        public GUIContent AddMappingFieldsetContent;
        public GUIContent NoSelectionContent;
        public GUIContent PlayModeOnly;
        public GUIContent NotEDrivenComponentContent;
        public GUIContent NotAContainerContent;
        public GUIContent NoComponentsInTheContainer;
        public GUIContent PanelPropertiesTitle;
        public GUIContent PanelPositionAndSizingTitle;
        public GUIContent PanelConstrainsTitle;
        public GUIContent PanelPaddingTitle;
        public GUIContent PanelToolsTitle;
        public GUIContent DialogButtonGroupTitle;
        public GUIContent AddEventHandler;
        public GUIContent AddEventHandlerButton;
        public GUIContent EventHandlerAddButton;
        public GUIContent EventHandlerCreateButton;
        public GUIContent EventHandlerScriptAddButton;
        public GUIContent EventHandlerScriptCreateButton;
        //public GUIContent Exclamation;
        public GUIContent PanelStringDataProviderTitle;
        public GUIContent PanelBindableStringListDataProviderTitle;
        public GUIContent PanelListItemDataProviderTitle;
        public GUIContent AddItem;
        public GUIContent AddItemWithText;
        public GUIContent RemoveItem;
        public GUIContent Apply;
        public GUIContent CreateEventHandler;
        public GUIContent AddButtonGroupElement;
        public GUIContent Dismiss;
        public GUIContent RemoveAllChildrenButton;

        public GUIContent WizardPrevious;
        public GUIContent WizardNext;
        public GUIContent WizardFinish;

        public GUIContent Search;

        public GUIContent ScriptJavascript;
        public GUIContent ScriptCSharp;
        public GUIContent ScriptBoo;
        public GUIContent Error;
        public GUIContent CloseButton;

        public GUIContent CheckForUpdatesButton;
        public GUIContent RemoveGhostObjectsButton;

        public GUIContent PanelStyleSheetTitle;
        public GUIContent Expand;
        public GUIContent Collapse;

        public GUIContent StyleDeclarationCreate;
        public GUIContent MoveUp;
        public GUIContent MoveDown;
        public GUIContent Duplicate;
        public GUIContent Delete;
        public GUIContent StyleSheetLocked;
        public GUIContent EditSelector;

        public GUIContent Back;
        public GUIContent Refresh;
        public GUIContent CopyToClipboard;
        public GUIContent AbandonChanges;
        public GUIContent WriteToLog;
        public GUIContent ReferencePanelTitle;

        public GUIContent GuiEditor;
        public GUIContent GuiInspectorEditor;
        public GUIContent FontMapperEditor;
        public GUIContent AudioPlayerEditor;
        public GUIContent AudioPlayerMapperEditor;

        public GUIContent DragHandle;
        public GUIContent AutoUpdateOn;
        public GUIContent AutoUpdateOff;
        
        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            MainPanelTitle = new GUIContent(" eDriven.Gui", TextureCache.Instance.MainPanelTitleIcon);
            HierarchyDebuggerPanelTitle = new GUIContent(" Hierarchy", TextureCache.Instance.DebugPanelTitleIcon);
            PersistenceDebuggerPanelTitle = new GUIContent(" Persistence", TextureCache.Instance.DebugPanelTitleIcon);
            HelpPanelTitle = new GUIContent(" Help", TextureCache.Instance.HelpPanelTitleIcon);
            InitScene = new GUIContent(" Init scene", TextureCache.Instance.Add);
            AddControlPanelTitle = new GUIContent(" Add child", TextureCache.Instance.Add);
            AboutPanelTitle = new GUIContent(" About", TextureCache.Instance.Information);
            OptionsPanelTitle = new GUIContent(" Options", TextureCache.Instance.Options);
            OptionsFieldsetTitle = new GUIContent(" Creation options", TextureCache.Instance.Options);
            AutoSave = new GUIContent(" Auto save", TextureCache.Instance.AutoSave, "Auto save after stopping the play mode");
            MonitorChanges = new GUIContent(" Watch changes", TextureCache.Instance.WatchChanges, "Watch and persist changes");
            LiveStyling = new GUIContent(" Live styling", TextureCache.Instance.MediaQuery, "Live styling (media queries are being run and property changes applied)");
            FixHierarchy = new GUIContent(" Fix hierarchy", TextureCache.Instance.FixHierarchy);
            Inspect = new GUIContent(" Inspect", TextureCache.Instance.Inspect);
            Logo1 = new GUIContent(TextureCache.Instance.Logo);
            Logo2 = new GUIContent(TextureCache.Instance.Logo2);
            
#if RELEASE
            Purchase = new GUIContent("Purchase", TextureCache.Instance.Purchase); 
            PurchasePanelTitle = new GUIContent(" Purchase", TextureCache.Instance.MainPanelTitleIcon);
            PurchaseButton = new GUIContent("Purchase", TextureCache.Instance.Purchase);
#endif
            //ComponentContent = new GUIContent("Component", TextureCache.Instance.InspectComponent);
            AddChildControlContent = new GUIContent(" Add child", TextureCache.Instance.Add);
            SelectContent = new GUIContent("Select");
            ExpandWidthContent = new GUIContent("Expand width");
            ExpandHeightContent = new GUIContent("Expand height");
            FactoryModeContent = new GUIContent("Factory mode");
            ApplyLastUsedSkinUponCreationContent = new GUIContent("Apply last used skin");

            NoHandlersAttached = new GUIContent(@"No event handlers found in any of the attached scripts.

Use the above buttons to add an existing script or create a new one.
Note: Scripts could not be added while in Play mode.

Scripts should have methods with a following signature:

public void ExampleHandler(eDriven.Gui.Events.Event e);", TextureCache.Instance.LogoSmall);

            NoEventsFound = new GUIContent(@"No events found.", TextureCache.Instance.LogoSmall);
            NoStyleableClassesFound = new GUIContent(@"No styleable classes found.", TextureCache.Instance.LogoSmall);
            NoStylePropertiesFound = new GUIContent(@"No style properties found.", TextureCache.Instance.LogoSmall);
            NoStylePropertiesSelected = new GUIContent(@"Nothing selected.", TextureCache.Instance.LogoSmall);
            NoMediaQueriesFound = new GUIContent(@"No media queries found.", TextureCache.Instance.LogoSmall);
            NoMediaQueriesSelected = new GUIContent(@"Nothing selected.", TextureCache.Instance.LogoSmall);
            NoUpdatesFound = new GUIContent(@"No updates found.", TextureCache.Instance.LogoSmall);
            NoEventHandlersFound = new GUIContent(@"No event handlers found.", TextureCache.Instance.LogoSmall);
            NoComponentsFound = new GUIContent(@"No components found.", TextureCache.Instance.LogoSmall);

            AddMappingFieldsetContent = new GUIContent(" Add handler", TextureCache.Instance.EventHandlerAddMapping);
            NoSelectionContent = new GUIContent("No selection.", TextureCache.Instance.LogoSmall);
            PlayModeOnly = new GUIContent("Play mode only.", TextureCache.Instance.LogoSmall); 
            NotEDrivenComponentContent = new GUIContent("Not eDriven.Gui component.", TextureCache.Instance.LogoSmall);
            NotAContainerContent = new GUIContent("Not a container.", TextureCache.Instance.LogoSmall);
            NoComponentsInTheContainer = new GUIContent(@"No components in the container.

Create new component or drag & drop the prefab.", TextureCache.Instance.LogoSmall);
            PanelPropertiesTitle = new GUIContent(" Properties", TextureCache.Instance.Properties);
            PanelPositionAndSizingTitle = new GUIContent(" Position and Sizing", TextureCache.Instance.Sizing);
            PanelConstrainsTitle = new GUIContent(" Constrains", TextureCache.Instance.Constrains); 
            PanelPaddingTitle = new GUIContent(" Padding", TextureCache.Instance.Padding);
            PanelToolsTitle = new GUIContent(" ToolsGroup", TextureCache.Instance.ToolsGroup);
            DialogButtonGroupTitle = new GUIContent(" Button group", TextureCache.Instance.ControlBarGroup);
            AddEventHandler = new GUIContent(" Add handler", TextureCache.Instance.Add);
            AddEventHandlerButton = new GUIContent(" Add handler", TextureCache.Instance.Add);
            EventHandlerAddButton = new GUIContent(" Map the existing event handler", TextureCache.Instance.EventHandlerAddButton);
            EventHandlerCreateButton = new GUIContent(" Create new handler", TextureCache.Instance.EventHandlerCreateButton);
            EventHandlerScriptAddButton = new GUIContent(" Add the existing script", TextureCache.Instance.EventHandlerScriptAddButton);
            EventHandlerScriptCreateButton = new GUIContent(" Create new script", TextureCache.Instance.EventHandlerScriptCreateButton);
            //Exclamation = new GUIContent(TextureCache.Instance.Exclamation);
            PanelStringDataProviderTitle = new GUIContent(" String data provider", TextureCache.Instance.DataProvider);
            PanelBindableStringListDataProviderTitle = new GUIContent(" Bindable string data provider", TextureCache.Instance.DataProvider);
            PanelListItemDataProviderTitle = new GUIContent(" List item data provider", TextureCache.Instance.DataProvider);
            AddItem = new GUIContent(TextureCache.Instance.Add);
            AddItemWithText = new GUIContent(" Add data item", TextureCache.Instance.Add);
            RemoveItem = new GUIContent(TextureCache.Instance.Remove);
            Apply = new GUIContent(" Apply", TextureCache.Instance.Accept);
            AddButtonGroupElement = new GUIContent(" Add button group element", TextureCache.Instance.Add);
            Dismiss = new GUIContent(" Dismiss", TextureCache.Instance.Cancel);
            CreateEventHandler = new GUIContent(" Add event handler", TextureCache.Instance.EventHandlerAddMapping);
            WizardPrevious = new GUIContent(TextureCache.Instance.WizardPrevious, "Previous");
            WizardNext = new GUIContent(TextureCache.Instance.WizardNext, "Next");
            WizardFinish = new GUIContent(TextureCache.Instance.WizardFinish, "Finish"); 
            RemoveAllChildrenButton = new GUIContent(" Remove all children", TextureCache.Instance.RemoveAll);
            Search = new GUIContent(" Search:", TextureCache.Instance.Search);
            ScriptJavascript = new GUIContent(" Javascript", TextureCache.Instance.ScriptJavascript);
            ScriptCSharp = new GUIContent(" CSharp", TextureCache.Instance.ScriptCSharp);
            ScriptBoo = new GUIContent(" Boo", TextureCache.Instance.ScriptBoo);
            Error = new GUIContent(TextureCache.Instance.Error);
            UseDarkSkin = new GUIContent("Use dark skin");
            CloseButton = new GUIContent(" Close", TextureCache.Instance.Cancel);

            CheckForUpdatesButton = new GUIContent(" Check for updates now!", TextureCache.Instance.CheckForUpdates);
            RemoveGhostObjectsButton = new GUIContent(" Remove ghost framework objects", TextureCache.Instance.RemoveGhostObjects);

            PanelStyleSheetTitle = new GUIContent(" Style sheet", TextureCache.Instance.StyleSheet);
            Expand = new GUIContent(TextureCache.Instance.Expand, "Expand all style declarations");
            Collapse = new GUIContent(TextureCache.Instance.Collapse, "Collapse all style declarations");

            StyleDeclarationCreate = new GUIContent(" New", TextureCache.Instance.Add, "Create new style declaration");

            MoveUp = new GUIContent(TextureCache.Instance.MoveUp, "Move up");
            MoveDown = new GUIContent(TextureCache.Instance.MoveDown, "Move down");
            Duplicate = new GUIContent(TextureCache.Instance.Copy, "Duplicate");
            Delete = new GUIContent(TextureCache.Instance.Remove, "Delete");
            StyleSheetLocked = new GUIContent(" Lock", TextureCache.Instance.Lock, "Lock the stylesheet structure");
            EditSelector = new GUIContent(TextureCache.Instance.Edit, "Edit selector");

            Back = new GUIContent(" Back", TextureCache.Instance.Back, "Back to component list");
            Refresh = new GUIContent(" Refresh", TextureCache.Instance.Refresh, "Refresh info");
            CopyToClipboard = new GUIContent(" Copy to clipboard", TextureCache.Instance.Copy, "Copy to clipboard");
            AbandonChanges = new GUIContent(" Abandon changes", TextureCache.Instance.AbandonChanges, "Abandon changes"); 
            WriteToLog = new GUIContent(" Write to log", TextureCache.Instance.WriteToLog, "Write to log");
            ReferencePanelTitle = new GUIContent(" Reference", TextureCache.Instance.Reference);

            GuiEditor = new GUIContent(" Gui", TextureCache.Instance.GuiEditor);
            GuiInspectorEditor = new GUIContent(" Gui Inspector", TextureCache.Instance.GuiInspectorEditor);
            FontMapperEditor = new GUIContent(" Font Mapper", TextureCache.Instance.FontMapperEditor);
            AudioPlayerEditor = new GUIContent(" Audio Player", TextureCache.Instance.AudioPlayerEditor);
            AudioPlayerMapperEditor = new GUIContent(" Audio Player Mapper", TextureCache.Instance.AudioPlayerEditor);

            DragHandle = new GUIContent(string.Empty, TextureCache.Instance.DragHandle);
            AutoUpdateOn = new GUIContent(" Update silently", TextureCache.Instance.AutoUpdateOn);
            AutoUpdateOff = new GUIContent(" Update silently", TextureCache.Instance.AutoUpdateOff);
        }
    }
}