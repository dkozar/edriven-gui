//using eDriven.Gui.Graphics;
//using eDriven.Gui.Components;
//using UnityEngine;

//namespace eDriven.Gui.GUIStyles
//{
//    public class SpectrumStyle
//    {
//        #region Quasi-Singleton

//        private static GUIStyle _instance;
//        public static GUIStyle Instance
//        {
//            get
//            {
//                //Debug.Log("Getting SpectrumStyle instance");

//                if (_instance == null)
//                {
//                    _instance = new GUIStyle();
//                    Initialize();
//                }
//                return _instance;
//            }
//        }

//        private SpectrumStyle()
//        {
//            // constructor is protected
//        }

//        public float Width;
//        public float Height;

//        #endregion

//        private static ProgramaticStyle _style;

//        private static void Initialize()
//        {
//            //Debug.Log("SpectrumStyle initializer");

//            _style = new ProgramaticStyle();
//            _style.Style = _instance;
//            _style.NormalGraphics = new Spectrum(300, 300); // (int) _instance.label.fixedWidth, (int) _instance.label.fixedHeight
//            _style.Commit();
//        }
//    }
//}