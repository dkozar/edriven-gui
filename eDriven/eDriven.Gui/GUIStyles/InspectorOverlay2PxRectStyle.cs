using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class InspectorOverlay2PxRectStyle
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
                    _instance = new GUIStyle {name = "InspectorOverlay2PxRectStyle"};
                    Initialize();
                }
                return _instance;
            }
        }

        private InspectorOverlay2PxRectStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            _style = new ProgramaticStyle
            {
                Style = _instance
            };

            const int w = (Weight + 1) * 2;

            _style.NormalGraphics = new Rect(w, w, new Stroke(Weight) { Color = Color.white });
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            _style.Commit();

        }
    }
}