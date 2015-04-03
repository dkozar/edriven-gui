using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.GUIStyles
{
    public class FormHeadingStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting FormHeadingStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private FormHeadingStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            //Debug.Log("FormHeadingStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(10, 10, 10, 10),
                             Font = FontMapper.GetDefault().Font,
                             Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1),
                             //FontSize = 30,
                             FontStyle = FontStyle.Italic,
                             NormalTextColor = new Color(1f, 0.7f, 0f, 1)
                         };

            //_style.Font = CoreSkinMapper.Instance.System.font;

            //const int w = (Weight + 1)*2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;

            //_style.NormalGraphics = new Rect(w, w,
            //                                 new Fill(new Color(0.3f, 0.3f, 0.3f, 0.2f))
            //    );
            
            _style.Commit();
        }
    }
}