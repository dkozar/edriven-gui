//#region Copyright

///*
 
//Copyright (c) Danko Kozar 2012. All rights reserved.
 
//*/

//#endregion Copyright

//using eDriven.Gui.Data.Binding;
//using UnityEngine;

//namespace eDriven.Editor.Gui
//{
//    public class DataSourceSingleton
//    {
//#if DEBUG
//    // ReSharper disable UnassignedField.Global
//        public static bool DebugMode;
//        // ReSharper restore UnassignedField.Global
//#endif

//        #region Singleton

//        private static DataSourceSingleton _instance;

//        /// <summary>
//        /// Singleton class for handling focus
//        /// </summary>
//        private DataSourceSingleton()
//        {
//            // Constructor is protected
//        }

//        /// <summary>
//        /// Singleton instance
//        /// </summary>
//        public static DataSourceSingleton Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//#if DEBUG
//                    if (DebugMode)
//                        Debug.Log(string.Format("Instantiating DataSource instance"));
//#endif
//                    _instance = new DataSourceSingleton();
//                    _instance.Initialize();
//                }

//                return _instance;
//            }
//        }

//        #endregion

//        private int TestInt;

//        public Bindable Bindable1;

//        /// <summary>
//        /// Initializes the Singleton instance
//        /// </summary>
//        private void Initialize()
//        {
//            Bindable1 = new Bindable("TestInt");
//        }
//    }
//}