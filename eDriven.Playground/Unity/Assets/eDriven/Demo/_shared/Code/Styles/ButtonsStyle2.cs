using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace Assets.eDriven.Demo.Styles
{
    public class ButtonStyle2
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private ButtonStyle2()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 3;

        private static void Initialize()
        {
            _instance.name = "ButtonsStyle2";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Font = FontMapper.GetDefault().Font,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(10, 10, 14, 10)
                         };

            const int w = (Weight + 1)*2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            _style.Font = FontMapper.Get("arial").Font;
            _style.FontSize = 30;
            _style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(new Color(0.9f, 0.9f, 0.9f, 1f)),
                                             new Stroke(Weight) { Color = Color.grey });

            _style.HoverTextColor = Color.white;
            _style.HoverGraphics = new Rect(w, w,
                                            new Fill(Color.green),
                                            new Stroke(Weight) { Color = Color.grey }
                );

            _style.ActiveTextColor = Color.white;
            _style.ActiveGraphics = new Rect(w, w,
                                             new Fill(new Color(0.2f, 0.9f, 0.2f, 1f)),
                                             new Stroke(Weight) { Color = Color.white }
                );
            _style.FocusedTextColor = Color.white;
            _style.FocusedGraphics = new Rect(w, w,
                                              new Fill(Color.blue),
                                              new Stroke(Weight) { Color = Color.yellow }
                );

            _style.OnNormalTextColor = Color.white;
            _style.OnNormalGraphics = new Rect(w, w,
                                               new Fill(new Color(0.3f, 0.4f, 0.9f, 1f)),
                                               new Stroke(Weight)
                );
            _style.OnHoverTextColor = Color.white;
            _style.OnHoverGraphics = new Rect(w, w,
                                              new Fill(new Color(0.4f, 0.5f, 0.9f, 1f)),
                                              new Stroke(Weight)
                );
            _style.OnActiveTextColor = Color.white;
            _style.OnActiveGraphics = new Rect(w, w,
                                               new Fill(new Color(0.4f, 0.6f, 0.9f, 1f)),
                                               new Stroke(Weight)
                );
            _style.OnFocusedTextColor = Color.white;
            _style.OnFocusedGraphics = new Rect(w, w,
                                                new Fill(Color.blue),
                                                new Stroke(Weight)
                );

            _style.Commit();
        }
    }
}