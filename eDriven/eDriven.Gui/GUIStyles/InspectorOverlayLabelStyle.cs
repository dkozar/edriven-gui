using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class InspectorOverlayLabelStyle
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
                    _instance = new GUIStyle
                                    {
                                        name = "InspectorOverlayLabelStyle"
                                    };
                    Initialize();
                }
                return _instance;
            }
        }

        private InspectorOverlayLabelStyle()
        {
            // constructor is protected
        }

        #endregion

        //public static Color BackgroundColor = Color.white; //new Color(0, 0, 1f, 0.3f);
        //public static Color TextColor = Color.white;
        //public static Font Font;
        //public static int FontSize;

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        public static void Redraw()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //Debug.Log("InspectorOverlayLabelStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(6, 6, 3, 4),
                             Font = FontMapper.GetWithFallback("pixel").Font
                         };

            //_style.Font = FontMapper.GetDefault();
            //Debug.Log("Initialized * " + _style.Font);
            
            /*if (FontMapper.IsMapping("pixel"))
                _style.Font = FontMapper.Get("pixel");*/

            const int w = (Weight + 1)*2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
_style.NormalTextColor = Color.white; // we'll tint it from outside
            _style.NormalGraphics = new Rect(w, w, new Fill(Color.white)); // we'll tint it from outside
            
            _style.Commit();
        }
    }
}