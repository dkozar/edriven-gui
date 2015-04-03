using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ButtonSingleStateStyle
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

        private ButtonSingleStateStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "ButtonSingleStateStyle";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Font = FontMapper.GetDefault().Font,
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             ContentOffset = new Vector2(-1, -1) // fix bad Unity offset
                         };

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Color.white),
                                             new Stroke(Weight) { Color = ColorMixer.FromHex(0x404040).ToColor() });
            _style.Commit();
        }
    }
}