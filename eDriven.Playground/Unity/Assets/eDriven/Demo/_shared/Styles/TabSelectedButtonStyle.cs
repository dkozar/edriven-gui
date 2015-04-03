using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect = eDriven.Gui.Graphics.Rect;

namespace eDriven.Playground.eDrivenSite
{
    public class TabSelectedButtonStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting ImageOnlyButtonStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private TabSelectedButtonStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "TabSelectedButtonStyle";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Font = FontMapper.GetDefault().Font,
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(6, 6, 3, 3)
                         };

            const int w = (Weight + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.black;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(ColorMixer.FromHex(0x3FFFAF).ToColor()),
                                             new Stroke(Weight) { Color = Color.grey }
                );
            //_style.HoverTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            //_style.HoverGraphics = new Rect(w, w,
            //                                new Fill(new Color(1f, 1f, 1f, 1f)),
            //                                new Stroke(Color.grey, Weight)
            //    );
            //_style.ActiveTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            //_style.ActiveGraphics = new Rect(w, w,
            //                                 new Fill(RgbColor.FromHex(0x808080).ToColor()),
            //                                 new Stroke(Color.black, Weight)
            //    );

            //_style.OnNormalTextColor = Color.white;
            //_style.OnNormalGraphics = new Rect(w, w,
            //                                   new Fill(RgbColor.FromHex(0x808080).ToColor()),
            //                                   new Stroke(RgbColor.FromHex(0xD3D3D3).ToColor(), Weight)
            //     );

            //_style.ImagePosition = ImagePosition.ImageAbove;

            _style.ContentOffset = new Vector2(-3, 0);

            _style.Commit();
        }
    }
}