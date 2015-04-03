/*using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class DraggableAreaStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting FocusIndicatorStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private DraggableAreaStyle()
        {
            // constructor is protected
        }

        #endregion

        public static int BorderWidth = 8;
        public static Color Color = new Color(0, 0, 0, 0.1f);
        
        private static ProgramaticStyle _style;

        public static int Divider = 3;
        
        //private const int Weight = 2;

        public static void Draw()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //Debug.Log("FocusIndicatorStyle initializer");

            _style = new ProgramaticStyle();
            _style.Style = _instance;
            
            // NOTE: we don't need font if we are not calculating the styled label size!
            //_style.Font = FontMapper.GetDefault();
            
            //_style.Alignment = TextAnchor.MiddleLeft;
            //_style.Padding = new RectOffset(10, 10, 0, 0);

            int w = (BorderWidth + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            int p = BorderWidth + 1;
            _style.Border = new RectOffset(p, p, p, p); // new RectOffset(Divider, Divider, 0, 0);
            //_style.Overflow = new RectOffset(BorderWidth, BorderWidth, BorderWidth, BorderWidth);

            //_style.FontSize = 30;
            //_style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Color)
                                             );

            _style.Commit();
        }
    }
}*/