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

using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class StyleCache
    {
        #region Singleton

        private static StyleCache _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleCache()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StyleCache();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        public static string EditorIconsPath = "eDriven/Editor/Icons/";
        public static string LogoPath = "eDriven/Editor/Logo/edriven_logo";

        public GUIStyle Label { get; private set; }
        public GUIStyle ControlButton { get; private set; }
        public GUIStyle DisplayRowButton { get; private set; }
        public GUIStyle Header { get; private set; }
        public GUIStyle Background { get; private set; }
        public GUIStyle Toggle { get; private set; }
        public GUIStyle Toggle2 { get; private set; }
        public GUIStyle CenteredLabel { get; private set; }
        public GUIStyle CenteredWhiteLabel { get; set; }
        public GUIStyle NormalLabel { get; private set; }
        public GUIStyle DragHandle { get; private set; }
        public GUIStyle ComponentHandle { get; private set; }
        public GUIStyle ContainerHandle { get; private set; }
        public GUIStyle GroupRowLabel { get; private set; }
        public GUIStyle EventFlow { get; private set; }
        public GUIStyle EventFlowDisabled { get; private set; }
        public GUIStyle ListRow { get; private set; }
        public GUIStyle GroupRow { get; private set; }
        public GUIStyle ImageOnlyButton { get; private set; }
        public GUIStyle ImageOnlyNoFrameButton { get; private set; }
        public GUIStyle SmallOpenButton { get; private set; }
        public GUIStyle ImageOnlyButtonWide { get; private set; }
        public GUIStyle Button2 { get; private set; }
        public GUIStyle Button { get; private set; }
        public GUIStyle BigButton { get; private set; }
        public GUIStyle BigLightButton { get; private set; }
        public GUIStyle TabButton { get; private set; }
        public GUIStyle TabBackground { get; private set; }
        public GUIStyle Breadcrumb { get; private set; }
        public GUIStyle BreadcrumbFirst { get; private set; }
        public GUIStyle GreenToggle { get; private set; }
        public GUIStyle GreenToggle2 { get; private set; }
        public GUIStyle Toolbar { get; private set; }
        public GUIStyle StatusToolbar { get; private set; }
        public GUIStyle Fieldset { get; private set; }
        public GUIStyle PanelChrome { get; private set; }
        public GUIStyle PanelChromeSquared { get; private set; }
        public GUIStyle PanelHeader { get; private set; }
        public GUIStyle PanelContent { get; private set; }
        public GUIStyle ScrollviewBackground { get; private set; }
        public GUIStyle InsetHeaderCollapsed { get; private set; }
        public GUIStyle InsetHeaderExpanded { get; private set; }
        public GUIStyle InsetChrome { get; private set; }
        public GUIStyle InsetContent { get; private set; }
        public GUIStyle Foldout { get; private set; }
        public GUIStyle AboutLabel { get; private set; }
        public GUIStyle AboutPanelContent { get; private set; }
        public GUIStyle AboutPanelContent2 { get; private set; }
        public GUIStyle PurchaseBox { get; private set; }
        public GUIStyle NormalBox { get; private set; }
        public GUIStyle Title { get; private set; }
        public GUIStyle Dot { get; private set; }
        public GUIStyle GridButton { get; private set; }
        public GUIStyle RichTextLabel { get; private set; }

        public static void Reset()
        {
            _instance = null;
        }

        private Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
        }

        /*private Color _textColorAccent;
        public Color TextColorAccent
        {
            get { return _textColorAccent; }
        }*/

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            var isDarkSkin = EditorSettings.UseDarkSkin;
            var darkSkinTextColor = ColorMixer.FromHex(0xb1b1b1).ToColor();
            var lightSkinTextColor = Color.black;
            _textColor = isDarkSkin ? darkSkinTextColor : lightSkinTextColor;
            //_textColorAccent = isDarkSkin ? RgbColor.FromHex(0xffff00).ToColor() : RgbColor.FromHex(0x0000ff).ToColor();

            var lightTextColor = isDarkSkin ? ColorMixer.FromHex(0xeeeeee).ToColor() : Color.white;

            string prefix = "Light/";
            if (isDarkSkin)
                prefix = "Dark/";

            //Debug.Log("Text color: " + textColor);

            /**
             * Note: EditorStyles.label introduces a BUG
             * NullReferenceException: Object reference not set to an instance of an object
             * UnityEditor.EditorStyles.get_label () (at C:/BuildAgent/work/d3d49558e4d408f4/Editor/Mono/GUI/EditorStyles.cs:11)
             * eDriven.Gui.Editor.Rendering.StyleCache.Initialize () (at
             * */

            GUIStyle labelBase = new GUIStyle(EditorStyles.label) //EditorStyles.largeLabel
            //GUIStyle labelBase = new GUIStyle(GUI.skin.label) //EditorStyles.largeLabel
            {
                normal = { textColor = _textColor },
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(3, 3, 2, 2),
                margin = new RectOffset(3, 3, 3, 3)
            };

            Label = new GUIStyle(labelBase);

            GUIStyle buttonBase = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_normal") },
                hover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_hover") },
                active = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_active") },
                onNormal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_on_normal") },
                onHover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_on_hover") },
                onActive = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_on_active") }
            };

            GUIStyle controlButtonBase = new GUIStyle(buttonBase)
                {
                    hover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "control_button_hover") },
                    active = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "control_button_active") },
                };

            ControlButton = new GUIStyle(controlButtonBase)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(10, 10, 2, 2), 
                    fixedHeight = 32,
                    margin = new RectOffset(3, 3, 3, 3),
                    clipping = TextClipping.Clip
                };

            DisplayRowButton = new GUIStyle(controlButtonBase)
            {
                normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "control_button_normal") },
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 2, 2),
                fixedHeight = 32,
                margin = new RectOffset(3, 3, 3, 3),
                clipping = TextClipping.Clip
            };

            Header = new GUIStyle(GUI.skin.label) { padding = new RectOffset(5, 5, 2, 2), alignment = TextAnchor.MiddleLeft };
            Background = new GUIStyle(GUI.skin.box) { padding = new RectOffset(10, 10, 2, 2), alignment = TextAnchor.MiddleCenter};
            
            Toggle = new GUIStyle(buttonBase)
                         {
                             padding = new RectOffset(10, 10, 2, 2), 
                             alignment = TextAnchor.MiddleCenter
                         };
            Toggle2 = new GUIStyle(buttonBase)
            {
                normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_normal2") },
                padding = new RectOffset(10, 10, 2, 2),
                alignment = TextAnchor.MiddleCenter
            };

            GridButton = new GUIStyle(Toggle)
            {
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 32,
                margin = new RectOffset(3, 3, 3, 3),
                clipping = TextClipping.Clip
            };

            CenteredLabel = new GUIStyle(labelBase)
                                 {
                                     padding = new RectOffset(10, 10, 10, 10), alignment = TextAnchor.MiddleCenter, imagePosition = ImagePosition.ImageAbove
                                 };

            CenteredWhiteLabel = new GUIStyle(labelBase)
            {
                padding = new RectOffset(10, 10, 10, 10),
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                normal = { textColor = Color.white }
            };

            NormalLabel = new GUIStyle(labelBase)
            {
                padding = new RectOffset(10, 10, 10, 10),
                alignment = TextAnchor.UpperLeft,
                imagePosition = ImagePosition.TextOnly,
                //wordWrap = true,
                normal = { textColor = _textColor },
                hover = { textColor = _textColor },
                active = { textColor = _textColor },
                focused = { textColor = _textColor }, // for EditorGUILayout.TextArea
            };

            RichTextLabel = new GUIStyle(NormalLabel)
            {
                richText = true
            };

            DragHandle = new GUIStyle(GUI.skin.label) { padding = new RectOffset(2, 2, 2, 2), alignment = TextAnchor.MiddleLeft};

            ListRow = new GUIStyle(GUI.skin.box)
                          {
                              normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "list_row") },
                              padding = new RectOffset(4, 4, 4, 4),
                              fixedHeight = 31
                          };
            GroupRow = new GUIStyle(GUI.skin.box)
            {
                normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "group_row") },
                padding = new RectOffset(8, 8, 4, 4),
                fixedHeight = 31
            };
            ComponentHandle = new GUIStyle(GUI.skin.label)
                                  {
                                      fixedHeight = 23,
                                      padding = new RectOffset(8, 8, 3, 3), 
                                      alignment = TextAnchor.MiddleLeft,
                                      normal = { textColor = _textColor },
                                      margin = new RectOffset(2, 2, 0, 0),
                                      border = new RectOffset(3, 3, 3, 3),
                                      wordWrap = false
                                  };
            ContainerHandle = new GUIStyle(GUI.skin.label)
                                  {
                                      fixedHeight = 23,
                                      padding = new RectOffset(8, 8, 2, 2), 
                                      alignment = TextAnchor.MiddleLeft,
                                      normal = { textColor = _textColor },
                                      fontStyle = FontStyle.Italic,
                                      //normal = { textColor = Color.white },
                                      margin = new RectOffset(2, 2, 0, 0),
                                      wordWrap = false
                                  };
            GroupRowLabel = new GUIStyle(labelBase)
            {
                normal = { textColor = darkSkinTextColor },
                fixedHeight = 23,
                //padding = new RectOffset(8, 8, 2, 2),
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                //normal = { textColor = Color.white },
                margin = new RectOffset(2, 2, 0, 0),
                wordWrap = false
            };
            
            EventFlow = new GUIStyle(GUI.skin.box)
                            {
                                normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "event_flow") },
                                fixedWidth = 50, fixedHeight = 16
                            };

            EventFlowDisabled = new GUIStyle(GUI.skin.box)
                                    {
                                        normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "event_flow_disabled") },
                                        fixedWidth = 50,
                                        fixedHeight = 16
                                    };

            ImageOnlyButton = new GUIStyle(buttonBase)
                                  {
                                      fixedWidth = 22,
                                      fixedHeight = 22,
                                      border = new RectOffset(15, 15, 22, 0),
                                      padding = new RectOffset(2, 2, 3, 2),
                                      margin = new RectOffset(2, 6, 0, 0)
                                  };
            ImageOnlyNoFrameButton = new GUIStyle(ImageOnlyButton)
                                         {
                                             normal = { background = null },
                                             margin = new RectOffset(3, 3, 3, 3)
                                         };
            SmallOpenButton = new GUIStyle(ImageOnlyButton)
            {
                normal = { background = null },
                margin = new RectOffset(-2, -2, -2, -2)
            };
            ImageOnlyButtonWide = new GUIStyle(ImageOnlyButton)
                                      {
                                          fixedWidth = 0, // no fixed width
                                          padding = new RectOffset(5, 5, 3, 2),
                                      };

            Button = new GUIStyle(buttonBase)
                     {
                         border = new RectOffset(3, 3, 3, 3),
                         padding = new RectOffset(6, 11, 5, 4)
                     }; 
            Button2 = new GUIStyle(buttonBase)
                     {
                         normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_normal2") },
                         border = new RectOffset(3, 3, 3, 3),
                         padding = new RectOffset(6, 11, 5, 4)
                     };
                
            BigButton = new GUIStyle(buttonBase)
                        {
                            //normal = { textColor = textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_normal2") },
                            border = new RectOffset(3, 3, 3, 3),
                            padding = new RectOffset(8, 8, 4, 3)
                        };

            BigLightButton = new GUIStyle(buttonBase)
            {
                normal = { textColor = lightSkinTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "Light/button_normal2") },
                hover = { textColor = lightSkinTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "Light/button_hover") },
                active = { textColor = lightSkinTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "Light/button_active") },
                onNormal = { textColor = lightSkinTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "Light/button_on_normal") },
                onHover = { textColor = lightSkinTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "Light/button_on_hover") },
                onActive = { textColor = lightSkinTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "Light/button_on_active") },
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(8, 8, 4, 3)
            };

            TabButton = new GUIStyle(buttonBase)
            {
                border = new RectOffset(3, 11, 11, 3),
                padding = new RectOffset(10, 14, 5, 4),
                margin = new RectOffset(0, 4, 0, 0)
            };

            TabButton = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "tab_button_normal") },
                hover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "tab_button_hover") },
                active = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "tab_button_active") },
                onNormal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "tab_button_on_normal") },
                onHover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "tab_button_on_hover") },
                onActive = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "tab_button_on_active") },
                border = new RectOffset(3, 11, 11, 3),
                padding = new RectOffset(10, 14, 5, 4),
                margin = new RectOffset(0, 4, 0, 0)
            };
            
            TabBackground = new GUIStyle(GUI.skin.button)
                                {
                                    normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + (isDarkSkin ? prefix + "tab_background" : prefix + "tab_background")) },
                                    border = new RectOffset(3, 3, 3, 3),
                                    padding = new RectOffset(0, 0, 0, 0),
                                    margin = new RectOffset(0, 0, 0, 0)
                                };

            Breadcrumb = new GUIStyle(GUI.skin.button)
                             {
                                 normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_normal") },
                                 hover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_hover") },
                                 active = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_active") },
                                 onNormal = { textColor = lightTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_on_normal") },
                                 onHover = { textColor = lightTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_on_hover") },
                                 onActive = { textColor = lightTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_on_normal") },
                                 fixedHeight = 22,
                                 border = new RectOffset(15, 15, 22, 0),
                                 padding = new RectOffset(15, 15, 1, 1),
                                 margin = new RectOffset(-12, -12, 0, 0),
                                 //fontStyle = FontStyle.Bold
                             };

            BreadcrumbFirst = new GUIStyle(GUI.skin.button)
                                  {
                                      normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_first_normal") },
                                      hover = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_first_hover") },
                                      active = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_first_active") },
                                      onNormal = { textColor = lightTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_first_on_normal") },
                                      onHover = { textColor = lightTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_first_on_hover") },
                                      onActive = { textColor = lightTextColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "breadcrumb_first_on_normal") },
                                      fixedHeight = 22,
                                      border = new RectOffset(15, 15, 22, 0),
                                      padding = new RectOffset(6, 15, 1, 1),
                                      margin = new RectOffset(4, -12, 0, 0),
                                      //fontStyle = FontStyle.Bold
                                  };

            GreenToggle = new GUIStyle(Button)
            {
                onNormal = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_green_on_normal") },
                onHover = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_green_on_hover") },
                onActive = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_green_on_active") }
            };

            GreenToggle2 = new GUIStyle(Button)
            {
                normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_normal2") },
                onNormal = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_green_on_normal") },
                onHover = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_green_on_hover") },
                onActive = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "button_green_on_active") }
            };

            Toolbar = isDarkSkin ? 
                          new GUIStyle(GUI.skin.button)
                          {
                              normal = { /*textColor = textColor, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "toolbar") },
                              border = new RectOffset(3, 3, 3, 3),
                              padding = new RectOffset(5, 5, 5, 5),
                              margin = new RectOffset(0, 0, 0, 0)
                          }: 
                          new GUIStyle(GUI.skin.button)
                          {
                              normal = { /*textColor = textColor, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "toolbar") },
                              border = new RectOffset(3, 3, 3, 3),
                              padding = new RectOffset(5, 5, 5, 5),
                              margin = new RectOffset(0, 0, 0, 0)
                          };

            StatusToolbar = new GUIStyle(GUI.skin.button)
            {
                normal = { /*textColor = textColor, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "white") },
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(0, 0, 0, 0)
            };

            Fieldset = new GUIStyle(GUI.skin.box)
                           {
                               normal = { textColor = _textColor, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "fieldset") },
                               border = new RectOffset(5, 5, 5, 5),
                               padding = new RectOffset(5, 5, 5, 5),
                               margin = new RectOffset(4, 4, 4, 0)
                           };

            /* ========== Panel ========== */
            PanelChrome = new GUIStyle(GUI.skin.box)
                              {
                                  normal = { /*textColor = Color.black, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "panel_chrome") },
                                  border = new RectOffset(5, 5, 5, 5),
                                  padding = new RectOffset(5, 5, 5, 5),
                                  margin = new RectOffset(4, 4, 4, 0)
                              };
            PanelChromeSquared = new GUIStyle(PanelChrome)
            {
                normal = { /*textColor = Color.black, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "panel_chrome_squared") }
            };

            PanelHeader = new GUIStyle(GUI.skin.label)
                              {
                                  alignment = TextAnchor.MiddleLeft,
                                  padding = new RectOffset(3, 3, 3, 3),
                                  margin = new RectOffset(3, 3, 3, 3),
                                  normal = { textColor = lightTextColor },
                                  fontStyle = FontStyle.Bold
                              };
            PanelContent = new GUIStyle(GUI.skin.box)
                               {
                                   normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "panel_content") },
                                   border = new RectOffset(5, 5, 5, 5),
                                   padding = new RectOffset(5, 5, 5, 5),
                                   margin = new RectOffset(4, 4, 4, 0)
                               };

            ScrollviewBackground = new GUIStyle(GUI.skin.box)
            {
                normal = { /*textColor = Color.black, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "panel_content") },
                border = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };

            /* ========== Inset ========== */
            InsetChrome = new GUIStyle(GUI.skin.box)
            {
                normal = { /*textColor = Color.black, */background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + prefix + "inset_chrome_normal") },
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(4, 4, 4, 4),
                margin = new RectOffset(4, -12, 0, 0)
            };
            InsetHeaderCollapsed = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(0, 2, 2, 2),
                margin = new RectOffset(2, 2, 2, 2),
                normal = { textColor = Color.black }
            };
            InsetHeaderExpanded = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(0, 2, 2, 2),
                margin = new RectOffset(2, 2, 2, 2),
                normal = { textColor = Color.black }
            };
            InsetContent = new GUIStyle(GUI.skin.box)
            {
                normal = { textColor = Color.black, background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "panel_content") },
                border = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(4, 4, 4, 0)
            };

            Foldout = new GUIStyle(GUI.skin.label)
                          {
                              alignment = TextAnchor.MiddleLeft,
                              padding = new RectOffset(0, 0, 0, 0),
                              margin = new RectOffset(3, 3, 3, 3)
                          };
            AboutLabel = new GUIStyle(GUI.skin.label)
                             {
                                 alignment = TextAnchor.MiddleLeft,
                                 padding = new RectOffset(3, 3, 2, 2),
                                 margin = new RectOffset(3, 3, 3, 3),
                                 normal = { textColor = lightTextColor },
                                 fontStyle = FontStyle.Bold
                             };
            AboutPanelContent = new GUIStyle(GUI.skin.box)
                                    {
                                        normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "about_panel_content") },
                                        border = new RectOffset(5, 5, 5, 5),
                                        padding = new RectOffset(5, 5, 5, 5),
                                        margin = new RectOffset(4, 4, 4, 0)
                                    };
            AboutPanelContent2 = new GUIStyle(GUI.skin.box)
                                     {
                                         normal = { background = EditorResourceLoader<Texture2D>.Load(EditorIconsPath + "about_panel_content2") },
                                         border = new RectOffset(5, 5, 5, 5),
                                         padding = new RectOffset(5, 5, 5, 5),
                                         margin = new RectOffset(4, 4, 4, 0)
                                     };
            PurchaseBox = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(10, 10, 2, 2),
                margin = new RectOffset(3, 3, 3, 3),
                normal = { textColor = lightTextColor, background = null },
                //fontStyle = FontStyle.Bold
            };
            NormalBox = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(3, 3, 2, 2),
                margin = new RectOffset(3, 3, 3, 3),
                normal = { background = null },
            };

            Title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(3, 3, 2, 2),
                margin = new RectOffset(3, 3, 15, 15),
                fontStyle = FontStyle.Bold
            };

            Dot = new GUIStyle(GUI.skin.label)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
        }
    }
}