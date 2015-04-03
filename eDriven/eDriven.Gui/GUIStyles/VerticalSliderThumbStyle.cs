/*using eDriven.Gui.GUIStyles.Helper;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.GUIStyles
{
    public class VerticalSliderThumbStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting HorizontalSliderStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private VerticalSliderThumbStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            int w = (Weight + 1) * 2;
            int w2 = 30;

            _instance.name = "VerticalSliderThumbStyle";
            //_instance.font = FontMapper.GetDefault();

            _style = new ProgramaticStyle {Style = _instance};

            SliderHelper.ApplyThumbStyle(_style, w, Weight);

            _style.FixedWidth = w2;
            _style.FixedHeight = w2;

            _style.Commit();
        }
    }
}*/