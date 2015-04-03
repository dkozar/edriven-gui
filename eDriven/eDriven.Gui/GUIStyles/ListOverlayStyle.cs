using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ListOverlayStyle
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
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private ListOverlayStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "ListOverlayStyle";

            _style = new ProgramaticStyle();
            _style.Style = _instance;

            //_style.Alignment = TextAnchor.MiddleCenter;

            //_style.Font = FontMapper.GetDefault();

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            //_style.Padding = new RectOffset(2, 2, 2, 2);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.white;
            _style.NormalGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 0)), new Stroke(Weight) { Color = Color.grey });

            //_style.HoverGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 0)), new Stroke(new Color(0.2f, 0.2f, 0.2f, 1), Weight));

            //_style.ActiveGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 0)), new Stroke(new Color(0.1f, 0.1f, 0.1f, 1), Weight));

            _style.Commit();

        }
    }
}