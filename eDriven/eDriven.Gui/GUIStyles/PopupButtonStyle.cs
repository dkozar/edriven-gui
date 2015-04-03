using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class PopupButtonStyle
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

        private PopupButtonStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            //Skin = (GUISkin) ScriptableObject.CreateInstance(typeof (GUISkin));
            //_instance.font = CoreSkinMapper.Instance.System.font;
            //_instance.button.alignment = TextAnchor.MiddleCenter;
            //Skin.font.fontNames = new string[]{"arial"};

            //StyleName = "button";

            //Debug.Log("PopupButtonStyle initializer");

            _instance.name = "PopupButtonStyle";

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Padding = new RectOffset(10, 10, 4, 4),
                             Font = FontMapper.GetDefault().Font
                         };
            //_style.Alignment = TextAnchor.MiddleLeft;

            //_style.Font = CoreSkinMapper.Instance.System.font;

            const int w = (Weight + 1)*2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(new Color(0.95f, 0.95f, 0.95f, 1f)),
                                             new Stroke(Weight)
                                             {
                                                 Color = Color.grey
                                             }
                );
            _style.HoverTextColor = Color.blue;
            _style.HoverGraphics = new Rect(w, w,
                //new Fill(Color.green)/*,
                                            new Fill(new Color(0.95f, 0.95f, 0.95f, 1f)),
                                            new Stroke(Weight)
                                            {
                                                Color = Color.grey
                                            }
                );
            _style.ActiveTextColor = Color.black;
            _style.ActiveGraphics = new Rect(w, w,
                                             new Fill(new Color(0.95f, 0.95f, 0.95f, 1f)),
                                             new Stroke(Weight)
                                             {
                                                 Color = Color.grey
                                             }
                );
            //_style.FocusedGraphics = new Rect(w, w,
            //                                  new Fill(Color.blue),
            //                                  new Stroke(Color.yellow, Weight)
            //    );

            //_style.FixedWidth = 320;
            _style.FixedHeight = 25;

            _style.Commit();
        }
    }
}