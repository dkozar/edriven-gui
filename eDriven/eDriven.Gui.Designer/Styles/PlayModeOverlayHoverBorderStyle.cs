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

using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.Designer.Styles
{
    public class PlayModeOverlayHoverBorderStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting PlayModeOverlayHoverBorderStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private PlayModeOverlayHoverBorderStyle()
        {
            // constructor is protected
        }

        #endregion

        public static int BorderWidth = 2;
        public static Color BorderColor = Color.white;

        private static ProgramaticStyle _style;
        
        //private const int Weight = 2;

        public static void Draw()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //Debug.Log("PlayModeOverlayHoverBorderStyle initializer");

            _style = new ProgramaticStyle {Style = _instance};
            /*_style.Font = Font ?? FontMapper.GetDefault();
            _style.Alignment = TextAnchor.MiddleLeft;
            _style.Padding = new RectOffset(10, 10, 0, 0);*/

            int w = (BorderWidth + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(BorderWidth + 1, BorderWidth + 1, BorderWidth + 1, BorderWidth + 1);

            //_style.FontSize = 30;
            //_style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(new Color(0.9f, 0.9f, 0.9f, 0)),//), // transparent fill!
                                             new Stroke(BorderWidth) { Color = BorderColor/*Color.red*/, }
                );

            _style.Commit();
        }
    }
}