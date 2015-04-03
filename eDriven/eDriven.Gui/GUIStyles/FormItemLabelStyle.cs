using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.GUIStyles
{
    public class FormItemLabelStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting FormItemLabelStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private FormItemLabelStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            //Debug.Log("FormItemLabelStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleLeft,
                             Padding = new RectOffset(4, 4, 7, 4),
                             Font = FontMapper.GetDefault().Font,
                             //FontSize = 8,
                             NormalTextColor = new Color(0.2f, 0.2f, 0.2f, 1)
                         };

            //_style.Font = CoreSkinMapper.Instance.System.font;

            //const int w = (Weight + 1)*2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            //_style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            _style.Commit();
        }
    }
}