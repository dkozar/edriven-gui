using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class TooltipStyle
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

        private TooltipStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            //_instance.font = FontMapper.IsMapping("pixel") ? FontMapper.Get("pixel") : FontMapper.GetDefault();

            //Debug.Log("TooltipStyle initializer");

            _style = new ProgramaticStyle();
            _style.Style = _instance;
            _style.Alignment = TextAnchor.MiddleLeft;
            _style.Padding = new RectOffset(6, 6, 4, 4);

            //_style.Font = CoreSkinMapper.Instance.System.font;
            //_style.Font = FontMapper.GetDefault();
            _style.Font = FontMapper.GetWithFallback("pixel").Font;

            const int w = (Weight + 1)*2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.black;
            _style.NormalGraphics = new Rect(w, w, new Fill(new Color(0.9f, 0.9f, 0.3f, 1)), new Stroke(1) { Color = Color.black});
            
            _style.Commit();
        }
    }
}