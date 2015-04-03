using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.GUIStyles
{
    public class ButtonLabelStyle
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
                    _instance = new GUIStyle { name = "ButtonLabelStyle" };
                    Initialize();
                }
                return _instance;
            }
        }

        private ButtonLabelStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private static void Initialize()
        {
            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Font = FontMapper.GetWithFallback("pixel").Font,
                             NormalTextColor = Color.white,
                             TextClipping = TextClipping.Clip
                         };
            
            _style.Commit();
        }
    }
}