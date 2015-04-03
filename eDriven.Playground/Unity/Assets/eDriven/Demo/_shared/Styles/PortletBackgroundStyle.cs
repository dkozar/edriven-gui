using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Components;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Playground.eDrivenSite.Styles
{
    public class PortletBackgroundStyle
    {
        //private static readonly Color Purple = RgbColor.FromHex(0x965FFF).ToColor();
        //private static readonly Color Green = RgbColor.FromHex(0x3FFFAF).ToColor();

        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting FormItemLabelSkin instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private PortletBackgroundStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            //_instance.font = FontMapper.IsMapping("pixel") ? FontMapper.Get("pixel") : FontMapper.GetDefault();

            //Debug.Log("LabelStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleLeft,
                             //Padding = new RectOffset(10, 10, 10, 10)
                         };

            //_style.Font = CoreSkinMapper.Instance.System.font;
            //_style.Font = FontMapper.GetDefault();
            //_style.Font = FontMapper.GetDefault();

            const int w = (Weight + 1)*2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FixedHeight = 50;

            //_style.FontSize = 8;
            //_style.NormalTextColor = Color.white; // no text in header background
            _style.NormalGraphics = new Rect(w, w, 
                                             //new Fill(RgbColor.FromHex(0x214078).ToColor())
                                             new Fill(Color.white) // 0xc0c0c0
                );

            //_style.HoverTextColor = Color.black;
            //_style.HoverGraphics = new Rect(w, w, new Fill(Purple)); // Green
            
            _style.Commit();
        }
    }
}