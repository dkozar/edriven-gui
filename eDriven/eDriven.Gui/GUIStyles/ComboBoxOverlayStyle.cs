/*using eDriven.Gui.Geom;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ComboBoxOverlayStyle
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

        private ComboBoxOverlayStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             ImagePosition = ImagePosition.ImageOnly,
                             Font = FontMapper.GetDefault().Font
                         };

            const int w = (Weight + 1) * 2; //25; //

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            _style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                            new Stroke(1) { Color = Color.grey });
                
            _style.HoverTextColor = Color.white;
            _style.HoverGraphics = new Rect(w, w,
                                            new Stroke(1) { Color = Color.black });
            _style.ActiveTextColor = Color.white;
            _style.ActiveGraphics = new Rect(w, w,
                                             new Stroke(1) { Color = Color.black });
            //_style.FocusedGraphics = new Rect(w, w,
            //                                  new Fill(Color.blue),
            //                                  new Stroke(Color.yellow, Weight)
            //    );

            _style.Commit();
        }
    }
}*/