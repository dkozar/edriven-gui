using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    ///<summary>
    ///</summary>
    public class RectBorderStyle
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
                    _instance = new GUIStyle { name = "RectBorderStyle" };
                    Initialize();
                }
                return _instance;
            }
        }

        private RectBorderStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 3;

        private static void Initialize()
        {
            _style = new ProgramaticStyle
                         {
                             Style = _instance
                         };

            const int w = (Weight + 1) * 2;

            _style.NormalGraphics = new Rect(w, w, new Fill(Color.white));

            _style.Commit();

        }
    }
}