using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class PanelHeaderBackgroundStyle
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
                    _instance = new GUIStyle {name = "PanelHeaderBackgroundStyle"};
                    Initialize();
                }
                return _instance;
            }
        }

        private PanelHeaderBackgroundStyle()
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
                             Padding = new RectOffset(10, 10, 10, 10)
                         };

            //_style.Font = CoreSkinMapper.Instance.System.font;
            //_style.Font = FontMapper.GetDefault();
            //_style.Font = FontMapper.GetDefault();

            const int w = (Weight + 1)*2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.white;
            //_style.NormalGraphics = new Rect(w, w, new Fill(RgbColor.FromHex(0x214078).ToColor())); /*new Color(0.3f, 0.3f, 0.3f, 0.85f)*/
            _style.NormalGraphics = new Rect(w, w, new Fill(ColorMixer.FromHex(0xFF0000).ToColor())); /*new Color(0.3f, 0.3f, 0.3f, 0.85f)*/
            
            _style.Commit();
        }
    }
}