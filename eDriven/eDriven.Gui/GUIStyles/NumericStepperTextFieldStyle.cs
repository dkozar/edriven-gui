using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;
using Rect = eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    ///<summary>
    ///</summary>
    public class NumericStepperTextFieldStyle
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
                    _instance = new GUIStyle { name = "NumericStepperTextFieldStyle" };
                    Initialize();
                }
                return _instance;
            }
        }

        private NumericStepperTextFieldStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _style = new ProgramaticStyle
            {
                Style = _instance,
                Alignment = TextAnchor.MiddleLeft,
                Padding = new RectOffset(6, 6, 6, 6),
                Font = FontMapper.GetWithFallback("pixel").Font,
                TextClipping = TextClipping.Clip
            };

            int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            _style.NormalTextColor = Color.black; //Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Color.white),
                                             new Stroke(Weight) { Color = Color.black }
                );
            _style.HoverTextColor = Color.black;
            _style.HoverGraphics = new Rect(w, w,
                                            new Fill(Color.white),
                                            new Stroke(Weight) { Color = Color.black }
                );
            _style.ActiveTextColor = Color.black;
            _style.ActiveGraphics = new Rect(w, w,
                                            new Fill(Color.white),
                                            new Stroke(Weight) { Color = Color.black }
                );

            _style.Commit();
        }
    }
}