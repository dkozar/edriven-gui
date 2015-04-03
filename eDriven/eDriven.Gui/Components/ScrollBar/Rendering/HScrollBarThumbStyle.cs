using eDriven.Gui.GUIStyles.Helper;
using UnityEngine;

namespace eDriven.Gui.Components.Rendering
{
    public class HScrollBarThumbStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting HScrollBarThumbStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private HScrollBarThumbStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            const int w = (Weight + 1) * 2;
            _style = new ProgramaticStyle {Style = _instance};
            ScrollbarHelper.ApplyThumbStyle(_style, w, Weight);
            _style.Commit();
        }
    }
}