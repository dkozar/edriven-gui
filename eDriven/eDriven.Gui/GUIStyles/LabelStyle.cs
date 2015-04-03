using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.GUIStyles
{
    public class LabelStyle
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
                    _instance = new GUIStyle {name = "LabelStyle"};
                    Initialize();
                }
                return _instance;
            }
        }

        private LabelStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        //private const int Weight = 2;

        private static void Initialize()
        {
            //_instance.font = FontMapper.IsMapping("pixel") ? FontMapper.Get("pixel") : FontMapper.GetDefault();

            //Debug.Log("LabelStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,

                             // changed from MiddleLeft on 20131006 because of the 
                             // button skin layout (having label left aligned because of using Left, Right..)
                             // should check the AbsoluteLayout why it doesn't take horizonral and vertical align as priority
                             //Alignment = TextAnchor.MiddleLeft,
                             Alignment = TextAnchor.MiddleLeft,
                             //Padding = new RectOffset(6, 6, 4, 4),
                             Font = FontMapper.GetWithFallback("pixel").Font,
                             //Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1),
                             NormalTextColor = Color.white,
                             TextClipping = TextClipping.Clip // clip!
                             //TextClipping = TextClipping.Overflow
                         };
            
            _style.Commit();
        }
    }
}