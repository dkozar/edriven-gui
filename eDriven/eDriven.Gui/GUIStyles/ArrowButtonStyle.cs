using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ArrowButtonStyle
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
                    _instance = new GUIStyle {name = "ArrowButtonStyle"};
                    Initialize();
                }
                return _instance;
            }
        }

        private ArrowButtonStyle()
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

            const int w = 25; //(Weight + 1)*2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            _style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new GraphicGroup(w, w,
                                                     new Rect(
                                                         new Fill(Color.white),
                                                         new Stroke(Weight) { Color = Color.grey }
                                                     ),
                                                     new Triangle(new Fill(Color.grey))
                                                     {
                                                         Direction = TriangleDirection.Down,
                                                         Pixels = 5
                                                     }
                );
                
            _style.HoverTextColor = Color.white;
            _style.HoverGraphics = new GraphicGroup(w, w,
                                            new Rect(
                                                //new Fill(Color.red),
                                                new Fill(Color.white), // 20121212
                                                new Stroke(Weight) { Color = Color.blue }
                                            ),
                                             new Triangle(new Fill(Color.blue))
                                             {
                                                 Direction = TriangleDirection.Down,
                                                 Pixels = 5
                                             }
                );
            _style.ActiveTextColor = Color.white;
            _style.ActiveGraphics = new GraphicGroup(w, w,
                                            new Rect(
                                                //new Fill(Color.red),
                                                new Fill(Color.white), // 20121212
                                                new Stroke(Weight) { Color = Color.black }
                                            ),
                                             new Triangle(new Fill(Color.black))
                                             {
                                                 Direction = TriangleDirection.Down,
                                                 Pixels = 5
                                             }
            );
            //_style.FocusedGraphics = new Rect(w, w,
            //                                  new Fill(Color.blue),
            //                                  new Stroke(Color.yellow, Weight)
            //    );

            _style.FixedWidth = w;
            _style.FixedHeight = w;

            _style.Commit();
        }
    }
}