using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class LoadingMaskBackgroundStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting RectButtonSkin instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle(); //new GUIStyle(); // NOTE: new GUIStyle() is your enemy :) Don't use it!
                    Initialize();
                }
                return _instance;
            }
        }

        private LoadingMaskBackgroundStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 3;

        private static void Initialize()
        {
            _instance.name = "LoadingMaskBackgroundStyle";

            _style = new ProgramaticStyle {Style = _instance};

            //_style.Alignment = TextAnchor.MiddleCenter;

            //_style.Font = FontMapper.GetDefault();

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            //_style.Padding = new RectOffset(2, 2, 2, 2);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.white;
            //_style.NormalGraphics = new Rect(w, w, new Fill(RgbColor.FromHexAndAlpha(/*6328252*/0xffffff, /*0.65f*/ 0.3f).ToColor())); // new Color(0, 0, 0, 0.35f))/*, new Stroke(Color.grey, Weight)*/);
            _style.NormalGraphics = new Rect(w, w, new Fill(Color.white));

            _style.Commit();

        }
    }
}