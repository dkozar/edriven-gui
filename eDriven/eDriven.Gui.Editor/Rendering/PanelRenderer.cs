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
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    public class PanelRenderer
    {
        /// <summary>
        /// Collapsible/expandable?
        /// </summary>
        public bool Collapsible = true;

        /// <summary>
        /// Show collapse button?
        /// </summary>
        public bool ShowCollapseButton;

        /// <summary>
        /// The color of the chrome
        /// </summary>
        public Color ChromeColor = ColorMixer.FromHex(0x335fd8).ToColor();

        private List<ToolDescriptor> _tools = new List<ToolDescriptor>();
        public List<ToolDescriptor> Tools
        {
            get
            {
                return _tools;
            }
            set
            {
                _tools = value;
                _clickedTools.Clear();
                foreach (ToolDescriptor tool in _tools)
                {
                    if (tool.Selected)
                        _clickedTools.Add(tool.Id);
                }
            }
        }

        private readonly List<string> _clickedTools = new List<string>();
        public List<string> ClickedTools
        {
            get { return _clickedTools; }
        }

        private bool _expanded;

        /// <summary>
        /// Chrome style
        /// </summary>
        public GUIStyle ChromeStyle;

        /// <summary>
        /// Inner style
        /// </summary>
        public GUIStyle ContentStyle;

        public bool RenderStart(GUIContent content, bool expanded)
        {
            GUI.backgroundColor = ChromeColor;

            EditorGUILayout.BeginVertical(ChromeStyle ?? StyleCache.Instance.PanelChrome); // panel chrome

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();

            if (Collapsible)
            {
                _expanded = GUILayout.Toggle(expanded, content, StyleCache.Instance.PanelHeader, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
                if (expanded != _expanded && !ShowCollapseButton)
                {
                    expanded = _expanded;
                }

                if (ShowCollapseButton)
                    expanded = GUILayout.Toggle(expanded, expanded ? TextureCache.Instance.Remove : TextureCache.Instance.Add, StyleCache.Instance.ImageOnlyNoFrameButton, GUILayout.ExpandWidth(false));    
            }
            else
            {
                // not collapsible. Render label instead.
                GUILayout.Label(content, StyleCache.Instance.PanelHeader, GUILayout.ExpandWidth(true));
            }

            foreach (ToolDescriptor descriptor in Tools)
            {
                bool isClicked = _clickedTools.Contains(descriptor.Id);

                bool isClicked2 = false;
                if (string.IsNullOrEmpty(descriptor.Tooltip))
                {
                    isClicked2 = GUILayout.Toggle(isClicked, descriptor.Image,
                                                   StyleCache.Instance.ImageOnlyNoFrameButton,
                                                   GUILayout.ExpandWidth(false));
                }
                else
                {
                    isClicked2 = GUILayout.Toggle(isClicked, new GUIContent(descriptor.Image, descriptor.Tooltip),
                                                   StyleCache.Instance.ImageOnlyNoFrameButton,
                                                   GUILayout.ExpandWidth(false));
                }

                bool changed = isClicked2 != isClicked;

                if (changed)
                {
                    if (isClicked2)
                        _clickedTools.Add(descriptor.Id);
                    else
                        _clickedTools.Remove(descriptor.Id);
                }
            }
            
            EditorGUILayout.EndHorizontal();

            if (expanded)
                EditorGUILayout.BeginVertical(ContentStyle ?? StyleCache.Instance.PanelContent, GUILayout.ExpandWidth(true)); // panel content
            else    
                EditorGUILayout.EndVertical();

            return expanded;
        }

        public bool RenderStart(string title, bool expanded)
        {
            return RenderStart(new GUIContent(title), expanded);
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        public void RenderEnd()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            EditorGUILayout.EndVertical(); // panel content
            EditorGUILayout.EndVertical(); // panel chrome
        }
    }

    public class ToolDescriptor
    {
        public ToolDescriptor()
        {
        }

        public ToolDescriptor(string id, Texture content)
        {
            Id = id;
            Image = content;
        }

        public ToolDescriptor(string id, Texture content, bool selected)
        {
            Id = id;
            Image = content;
            Selected = selected;
        }

        public ToolDescriptor(string id, Texture content, string tooltip) :
            this(id, content)
        {
            Tooltip = tooltip;
        }

        public ToolDescriptor(string id, Texture content, bool selected, string tooltip) :
            this(id, content, selected)
        {
            Tooltip = tooltip;
        }

        public string Id;
        public Texture Image;
        public bool Selected;
        public string Tooltip;
    }
}