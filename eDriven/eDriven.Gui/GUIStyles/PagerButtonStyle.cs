using eDriven.Gui.Geom;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class PagerButtonStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting ButtonStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private PagerButtonStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "PagerButtonStyle";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Font = FontMapper.GetDefault().Font,
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(6, 6, 6, 6)
                         };

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(ColorMixer.FromHex(0xC0C0C0).ToColor()),
                                             new Stroke(Weight) { Color = ColorMixer.FromHex(0x404040).ToColor() }
                );
            _style.HoverTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            _style.HoverGraphics = new Rect(w, w,
                                            new Fill(new Color(1f, 1f, 1f, 1f)),
                                            new Stroke(Weight) { Color = Color.grey }
                );
            _style.ActiveTextColor = new Color(0.1f, 0.1f, 0.1f, 1);
            _style.ActiveGraphics = new Rect(w, w,
                                             new Fill(ColorMixer.FromHex(0x808080).ToColor()),
                                             new Stroke(Weight) { Color = ColorMixer.FromHex(0xD3D3D3).ToColor() }
                );

            _style.OnNormalTextColor = Color.white;
            _style.OnNormalGraphics = new Rect(w, w,
                                               new Fill(ColorMixer.FromHex(0x808080).ToColor()),
                                               new Stroke(Weight) { Color = ColorMixer.FromHex(0xD3D3D3).ToColor() }
                 );

            _style.FixedHeight = 25;
            _style.FixedWidth = 25;

            _style.Commit();
        }
    }
}