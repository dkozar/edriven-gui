using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Playground.eDrivenSite
{
    public class ImageStyle
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
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private ImageStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            //_instance.font = FontMapper.IsMapping("pixel") ? FontMapper.Get("pixel") : FontMapper.GetDefault();

            //Debug.Log("ImageStyle initializer");

            const int w = (Weight + 1) * 2;
            
            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleLeft,
                             Padding = new RectOffset(Weight, Weight, Weight, Weight),
                             Margin = new RectOffset(0, 0, 0, 0),
                             Font = FontMapper.GetWithFallback("pixel").Font,
                             Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1),
                             NormalGraphics = new Rect(w, w,
                                                       new Fill(ColorMixer.FromHex(0xC0C0C0).ToColor()),
                                                       new Stroke(Weight) { Color = ColorMixer.FromHex(0x404040).ToColor() }
                                 )
                         };
            //_style.Font = CoreSkinMapper.Instance.System.font;
            //_style.Font = FontMapper.GetDefault();

            
            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;

            //_style.FontSize = 8;
            //_style.NormalTextColor = Color.white;

            _style.Commit();
        }
    }
}