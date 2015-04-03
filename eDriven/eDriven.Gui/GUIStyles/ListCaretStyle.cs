/*using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ListCaretStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting RectToggleSkin instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private ListCaretStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            //Debug.Log("RectToggleSkin initializer");

            _instance.name = "ListCaretStyle";

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Font = FontMapper.GetDefault().Font,
                             Alignment = TextAnchor.MiddleLeft,
                             Padding = new RectOffset(10, 10, 6, 6)
                         };

            const int w = (Weight + 1)*2; // calculating the minimum width to stretch

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.black;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(new Color(0.9f, 0.9f, 0.9f, 1f))
                                             //new Stroke(Color.grey, Weight)
                );

            _style.OnNormalTextColor = Color.black;
            _style.OnNormalGraphics = new Rect(w, w,
                                               new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())
                                               //new Stroke(Color.black, Weight)
                );

            _style.Commit();
        }
    }
}*/