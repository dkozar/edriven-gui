using eDriven.Gui.Geom;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ButtonBarButtonStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting ToggleStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private ButtonBarButtonStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            //Debug.Log("ToggleStyle initializer");

            _instance.name = "ButtonBarButtonStyle";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Font = FontMapper.GetDefault().Font,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(10, 10, 0, 0)
                         };

            const int w = (Weight + 1)*2; // calculating the minimum width to stretch

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 30;
            _style.NormalTextColor = Color.white;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Color.grey),
                                             new Stroke(Weight) { Color = Color.grey }
                );

            _style.HoverTextColor = Color.white;
            _style.HoverGraphics = new Rect(w, w,
                                            new Fill(new Color(0.9f, 0.9f, 0.9f, 1f)),
                                            new Stroke(Weight) { Color = Color.grey }
                );

            _style.ActiveTextColor = Color.white;
            _style.ActiveGraphics = new Rect(w, w,
                                             new Fill(new Color(0.9f, 0.9f, 0.9f, 1f)),
                                             new Stroke(Weight) { Color = Color.grey }
                );
            _style.FocusedTextColor = Color.white;
            _style.FocusedGraphics = new Rect(w, w,
                                              new Fill(new Color(0.9f, 0.9f, 0.9f, 1f)),
                                              new Stroke(Weight) { Color = Color.grey }
                );

            _style.OnNormalTextColor = Color.gray;
            _style.OnNormalGraphics = new Rect(w, w,
                                               new Fill(new Color(1f, 1f, 1f, 1f)),
                                               new Stroke(Weight) { Color = Color.grey }
                );
            _style.OnHoverTextColor = Color.gray;
            _style.OnHoverGraphics = new Rect(w, w,
                                              new Fill(new Color(1f, 1f, 1f, 1f)),
                                              new Stroke(Weight) { Color = Color.grey }
                );
            _style.OnActiveTextColor = Color.gray;
            _style.OnActiveGraphics = new Rect(w, w,
                                               new Fill(new Color(1f, 1f, 1f, 1f)),
                                               new Stroke(Weight) { Color = Color.grey }
                );
            _style.OnFocusedTextColor = Color.gray;
            _style.OnFocusedGraphics = new Rect(w, w,
                                                new Fill(new Color(1f, 1f, 1f, 1f)),
                                                new Stroke(Weight) { Color = Color.grey }
                );

            _style.Commit();
        }
    }
}