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

namespace eDriven.Gui.Editor
{
    internal static class Help
    {
        public static string Main
        {
            get
            {
// ReSharper disable ConvertToConstant.Local
                var text =
// ReSharper restore ConvertToConstant.Local
                @"This is a main eDriven.Gui panel. It has to be present on screen for a full eDriven.Gui Designer functionality.

eDriven.Gui is rendering its content in the Game view, but only in Play mode. 

For seeing eDriven.Gui content, you should hit the Play button (see the image above). 

However, changes made in Play mode will be persisted: after stopping the Play mode, eDriven will save the changes (depending of the Auto save button setting). 

Use toggle buttons for setting up:

1. Watch changes: monitors both the Hierarchy view and the Inspector. In the case of changes related to eDriven.Gui during the Play mode, the dialog for saving changes is presented to the user after the Play mode is stopped.
2. Auto save: in the case of change, no dialog will be presented, but the changes would be applied automatically.
3. Inspect: allows selecting the component in the game view (while in Play mode). Draws an overlay on top of all the components.
4. Live styling: working in play mode, editor only. 
When turned ON:
A. styles are being re-processed upon each stylesheet change
B. media queries are being run upon each screen resize
Live styling is processor intensive so turn it OFF when not needed (or else the editor might not be so responsive).

When Inspect is on, the Game view becomes interactive:

1. Clicking the component in the game view selects the component in the hierarchy and hightlights it.
2. Double-clicking the component brings up the ""Add Event Handler"" dialog

Use the breadcrumbs to walk up the hierarchy.

Use tabs to switch between:

1. Events panel: used for managing events and event handlers for the selected component
2. Order panel: used for arranging the order of children if the selected component is Container or a subclass of Container
3. Layout panel: used for setting the layout of the component if the selected component is Container or a subclass of Container.

You can also switch between panels by clicking the component on screen:

1. Double click: switches to Events panel
2. Right click: switches to Order panel
3. Middle click: switches to Layout panel";

#if RELEASE
            text += @"

Use the Purchase button to purchase a non-watermarked version of eDriven.Gui via the Unity Asset Store.

By supporting eDriven.Gui you are helping the further development of the framework. Thank you!";
#endif
                return text;
            }
        }

        /*public static string EventDisplay
        {
            get
            {
                return
                    @"Select any container in the Hierarchy view and then click any component in this dialog.
The component will be created and added to hierarchy.
If holding the Control key, the created component will get selected.
Note: Stage could not be created as a child of another component.";
            }
        }*/

        public static string EventList
        {
            get
            {
                return
                    @"Select a single event type from the list.
Event type is an identifier of the event that will be listened to.

Use buttons on top to filter out target and bubbling events:
1. target events are events being dispatched by selected component exclusively
2. bubbling events are events being dispatched by component's children
Use search field to filter out events by name.
Note: bubbling events are available for containers and container subclasses only, because only them can have children

You could use the input mode. In input mode, the supplied event name should not be an empty string.

This dialog could also be popped up by double-clicking the game view.
Note that you'll have to enable the component double-clicking in the Events view in order to do this";
            }
        }

        public static string ScriptActions
        {
            get
            {
                return
                    @"Use toolbar on top to chose the script extension.
The choice of the extension will determine the type of scripts that will be used (UnityScript, C# or Boo).

Select one of the following actions:

1. Adds an existing event handler (from one of the scripts already attached to the selected game object).
2. Adds the existing script from disk to a selected game object and adds the event handler chosen from that script.
3. Modifies the existing script by adding the new event handler.
4. Creates new script with event handler.";
            }
        }

        public static string EventHandlerList
        {
            get
            {
                return
                    @"Select a single method from the list.
The event handler is a listener method that the event that will be mapped to.";
            }
        }

        public static string EventHandlerCreationSettings
        {
            get
            {
                return
                    @"Use this settings for finalizing the event handler creation and/or mapping.
Some of the options (like Event phases) could be changed later in Events view.";
            }
        }

        public static string Toolbox
        {
            get
            {
                return
                    @"Select any container in the Hierarchy view and choose the component to create.
The component will be created and added to the hierarchy.
If holding the Control key, the created component will get selected.
Note: Stage could not be created as a child of another component.

Toolbox could also be popped up by right-double-clicking the game view.";
            }
        }

        public static string HierarchyDebugWindow
        {
            get
            {
                return
                    @"Use this window for monitoring or fixing hierarchy issues.";
            }
        }

        public static string PersistenceDebugWindow
        {
            get
            {
                return
                    @"Use this window for monitoring or fixing persistence issues.
The persistence mechanism propagates play mode changes to edit mode and enables saving them with the scene.";
            }
        }

        public static string ReferenceWindow
        {
            get
            {
                return
                    @"You can use this window to get the information related to events, styles and skins available for each component type.";
            }
        }

        public static string CheckForUpdates
        {
            get
            {
                return
                    @"Enabling this option will make eDriven check for updates from eDriven server each time the Unity Editor is opened.

Note: the update check will run only if the eDriven.Gui window is present in the editor.";
            }
        }

        public static string StyleableComponentList
        {
            get
            {
                return
                    @"Select a single component type from the list.
Use search field to filter out components by class (type) name.";
            }
        }

        public static string ComponentStyleList
        {
            get
            {
                return
                    @"Clicke styles to move them between left and the right list.
Styles in the right list will be present in the style declaration.
Use search field as a filter.";
            }
        }

        public static string StyleDeclarationCreationSettings
        {
            get
            {
                return
                    @"Use this settings for finalizing the style declaration creation.
Note: for some of the styling modules the ommiting of the subject (component type) is disabled.";
            }
        }
    }
}
