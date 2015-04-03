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

#if RELEASE

using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.Check
{
    internal class LockButtonStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting LockButtonStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private LockButtonStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "LockButtonStyle";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Font = FontMapper.GetDefault().Font,
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(15, 15, 8, 8)
                         };

            const int w = (Weight + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            Color c = ColorMixer.FromHex(0xFFFFFF).ToColor();
            c.a = 0.5f;

            //_style.FontSize = 8;
            _style.NormalTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(c),
                                             new Stroke(Weight) { Color = ColorMixer.FromHex(0x404040).ToColor() }
                );
            _style.HoverTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            _style.HoverGraphics = new Rect(w, w,
                                            new Fill(new Color(1f, 1f, 1f, 1f)),
                                            new Stroke(Weight) { Color = Color.grey }
                );
            //_style.ActiveTextColor = Color.white;
            _style.ActiveTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            _style.ActiveGraphics = new Rect(w, w,
                                             new Fill(new Color(0.75f, 0.75f, 0.75f, 1)),
                                             new Stroke(Weight) { Color = ColorMixer.FromHex(0x404040).ToColor() }
                );
            
            _style.OnNormalTextColor = Color.white;
            _style.OnNormalGraphics = new Rect(w, w,
                                               new Fill(ColorMixer.FromHex(0x808080).ToColor()),
                                               new Stroke(Weight) { Color = ColorMixer.FromHex(0xD3D3D3).ToColor() }
                );

            _style.Commit();

            //_style.FixedHeight = 20;
            //_style.FixedWidth = 100;
        }
    }
}

#endif