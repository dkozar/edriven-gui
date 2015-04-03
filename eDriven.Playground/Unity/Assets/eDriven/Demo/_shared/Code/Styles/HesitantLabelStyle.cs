using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;

namespace Assets.eDriven.Demo.Styles
{
    public class HesitantLabelStyle
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

        private HesitantLabelStyle()
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
                             Alignment = TextAnchor.MiddleLeft,
                             Padding = new RectOffset(12, 12, 6, 6),
                             Font = FontMapper.Get("olney").Font,
                             FontSize = 40,
                             NormalTextColor = Color.white
                         };

            _style.Commit();
        }
    }
}