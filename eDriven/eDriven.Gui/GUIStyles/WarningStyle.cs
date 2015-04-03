/*using eDriven.Gui.Geom;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class WarningStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting WarningStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private WarningStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 3;

        private static void Initialize()
        {
            _instance.name = "warning";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle();
            _style.Font = FontMapper.GetDefault().Font;
            _style.Style = _instance;
            _style.Alignment = TextAnchor.MiddleCenter;
            _style.Padding = new RectOffset(10, 10, 5, 5);

            int w = (Weight + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 14;
            _style.NormalTextColor = Color.white;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Color.magenta),
                                             new Stroke(Weight) { Color = Color.white }
                );

            _style.Commit();
        }
    }
}*/