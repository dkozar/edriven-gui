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

using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.Designer.Styles
{
    public class PlayModeOverlaySelectionLabelStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting FormItemLabelSkin instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private PlayModeOverlaySelectionLabelStyle()
        {
            // constructor is protected
        }

        #endregion

        //public static Color BackgroundColor = Color.white; //new Color(0, 0, 1f, 0.3f);
        //public static Color TextColor = Color.white;
        //public static Font Font;
        //public static int FontSize;

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        public static void Redraw()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //Debug.Log("PlayModeOverlayHoverLabelStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(6, 6, 3, 4),
                             Font = FontMapper.GetWithFallback("pixel").Font
                         };

            //_style.Font = FontMapper.GetDefault();
            //Debug.Log("Initialized * " + _style.Font);
            
            /*if (FontMapper.IsMapping("pixel"))
                _style.Font = FontMapper.Get("pixel");*/

            const int w = (Weight + 1)*2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = FontSize;
            _style.NormalTextColor = Color.white; // we'll tint it from outside
            _style.NormalGraphics = new Rect(w, w, new Fill(Color.white)); // we'll tint it from outside
            
            _style.Commit();
        }
    }
}