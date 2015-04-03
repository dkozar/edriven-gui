/*using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ListItemStyle
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

        private ListItemStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            //Debug.Log("RectToggleSkin initializer");

            _instance.name = "ListItemStyle";

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
            _style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Color.white)
                                             //new Stroke(Color.grey, Weight)
                );

            _style.HoverTextColor = Color.grey;
            _style.HoverGraphics = new Rect(w, w,
                                            new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())//new Fill(new Color(1f, 0.85f, 0f, 1f))
                                            //new Stroke(Color.grey, Weight)
                );

            _style.ActiveTextColor = Color.grey;
            _style.ActiveGraphics = new Rect(w, w,
                                             new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())//new Fill(new Color(1f, 0.75f, 0f, 1f))
                                             //new Stroke(Color.black, Weight)
                );
            //_style.FocusedTextColor = Color.white;
            //_style.FocusedGraphics = new Rect(w, w,
            //                                  new Fill(new Color(0.9f, 0.9f, 0.9f, 1f)),
            //                                  new Stroke(new Color(0.5f, 0.5f, 0.5f, 1f), Weight)
            //    );

            _style.OnNormalTextColor = Color.black;
            _style.OnNormalGraphics = new Rect(w, w,
                                               new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())
                                               //new Stroke(Color.black, Weight)
                );
            _style.OnHoverTextColor = Color.black;
            _style.OnHoverGraphics = new Rect(w, w,
                                              new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())
                                               //new Stroke(Color.black, Weight)
                );
            _style.OnActiveTextColor = Color.black;
            _style.OnActiveGraphics = new Rect(w, w,
                                               new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())
                                               //new Stroke(Color.black, Weight)
                );
            //_style.OnFocusedTextColor = Color.white;
            //_style.OnFocusedGraphics = new Rect(w, w,
            //                                    new Fill(new Color(0.9f, 0.9f, 0.9f, 1f)),
            //                                   new Stroke(Color.black, Weight)
            //    );

            //_style.FixedWidth = 400;
            //_style.FixedHeight = 40;

            _style.Commit();
        }
    }
}*/