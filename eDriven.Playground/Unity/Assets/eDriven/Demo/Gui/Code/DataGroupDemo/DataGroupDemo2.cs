using System;
using Assets.eDriven.Demo.Gui.Code;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Core.Caching;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Gui.Util;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using System.Collections.Generic;

namespace Assets.eDriven.Demo.DataGroupDemo
{
    /// <summary>
    /// </summary>
// ReSharper disable UnusedMember.Global
    public class DataGroupDemo2 : global::eDriven.Gui.Gui
// ReSharper restore UnusedMember.Global
    {
// ReSharper disable UnusedMember.Local
        void Awake()
// ReSharper restore UnusedMember.Local
        {
            //Debug.Log("Adding to debug");
            //StyleDebugging.DebugComponents.Add(typeof(NumericStepperSkin2));
            TweenErrorSignal.Instance.Connect(TweenErrorSlot);
            //CursorManager.RenderingMode = CursorRenderingMode.Stage;
        }

        private int _count;

        private ArrayList _dataProvider;

        override protected void CreateChildren()
        {
            base.CreateChildren();

            //Scroller scroller = new Scroller
            //                        {
            //                            SkinClass = typeof(ScrollerSkin2),
            //                            Left = 0,
            //                            Right = 0,
            //                            Top = 0,
            //                            Bottom = 0
            //                        };
            ////scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.Off);
            ////scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.Off);
            //AddChild(scroller);

            //Group viewport = new Group
            //                     {
            //                         Layout = new VerticalLayout
            //                                      {
            //                                          HorizontalAlign = HorizontalAlign.Left,
            //                                          PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10,
            //                                          Gap = 10
            //                                      }
            //                     };
            //scroller.Viewport = viewport;

            #region Controls

            HGroup hbox = new HGroup
                              {
                                  VerticalAlign = VerticalAlign.Middle,
                                  PaddingLeft = 10,
                                  PaddingRight = 10,
                                  PaddingTop = 10,
                                  PaddingBottom = 10,
                                  Gap = 10
                              };

            hbox.SetStyle("showBackground", true);
            hbox.SetStyle("backgroundColor", ColorMixer.FromHex(0x004CFF).ToColor());
            /*viewport.*/AddChild(hbox);

            Button button = new Button
                                {
                                    Text = "Increase width",
                                    SkinClass = typeof(ImageButtonSkin),
                                    Icon = ImageLoader.Instance.Load("Icons/shape_square_add")
                                };
            button.Click += delegate
                                {
                                    button.Width += 10;
                                    _dataProvider.AddItem("miki " + button.Width);
                                };
            hbox.AddChild(button);

            Button btnWindow = new Button
                                   {
                                       Text = "New window",
                                       FocusEnabled = false,
                                       SkinClass = typeof(ImageButtonSkin),
                                       Icon = ImageLoader.Instance.Load("Icons/comment")
                                   };
            btnWindow.Click += delegate
                                   {
                                       //_container.RemoveAllChildren();
                                       //for (int i = _container.NumberOfChildren - 1; i >= 0; i--)
                                       //{
                                       //    _container.RemoveContentChildAt(i);
                                       //}
                                       //_container.RemoveAllContentChildren();
                                       //_count = 0;
                                       _count++;

                                       var window = new MyWindow
                                                        {
                                                            Title = "The Window " + _count,
                                                            Id = "window_" + _count,
                                                            SkinClass = typeof (WindowSkin2),
                                                            Icon = ImageLoader.Instance.Load("Icons/balloon_32"),
                                                            Width = 400,
                                                            Height = 600
                                                        };

                                       //System.Random rnd = new System.Random();
                                       //if (rnd.Next(2) > 0)

                                       window.Plugins.Add(new Resizable { ShowOverlay = false });
                                       window.AddEventListener(CloseEvent.CLOSE, delegate
                                                                                     {
                                                                                         PopupManager.Instance.RemovePopup(window);
                                                                                     });

                                       #region Cannot use plugin!

                                       // NOTE: We cannot use the plugin having a DragHandle because the placehoder group is being switched live
                                       // we just have to use the MoveArea skin part and do the dragging thing from inside the Dialog

                                       //var draggable = new Draggable
                                       //                    {
                                       //                        DragHandle = b.HeaderGroup
                                       //                    };
                                       //b.Plugins.Add(draggable);
            
                                       #endregion

                                       PopupManager.Instance.AddPopup(window, false);
                                       PopupManager.Instance.CenterPopUp(window);
                                   };
            hbox.AddChild(btnWindow);

            #endregion

            hbox = new HGroup
            {
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            };

            //hbox.SetStyle("showBackground", true);
            //hbox.SetStyle("backgroundColor", RgbColor.FromHex(0x004CFF).ToColor());
            /*viewport.*/AddChild(hbox);

            #region Data controls

            List<object> source = new List<object> {"Failure", "Teaches", "Success", "One", "Two", "Three", "Four", "Five", "Six"};

            _dataProvider = new ArrayList(source);

            var factory = new ItemRendererFactory<DefaultItemRenderer>();

            //DataGroup dataGroup = new DataGroup
            //                          {
            //                              Layout = new VerticalLayout(),
            //                              DataProvider = _dataProvider,
            //                              //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            //                              ItemRendererFunction = delegate(object item)
            //                                                         {
            //                                                             return factory;
            //                                                         }
            //                          };
            //hbox.AddChild(dataGroup);

            //dataGroup = new DataGroup
            //                {
            //                    Layout = new TileLayout { RequestedColumnCount = 4 },
            //                    //Width = 200, Height = 200,
            //                    DataProvider = _dataProvider,
            //                    //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            //                    ItemRendererFunction = delegate
            //                                               {
            //                                                   return factory;
            //                                               },
            //                    //ClipAndEnableScrolling = true
            //                };
            //hbox.AddChild(dataGroup);

            List list = new List
                            {
                                //Layout = new TileLayout { RequestedColumnCount = 4 },
                                Width = 200, Height = 200,
                                Left = 100, Top = 50,
                                DataProvider = _dataProvider,
                                //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
                                ItemRendererFunction = delegate
                                {
                                    return factory;
                                }
                            };
            /*viewport.*/AddChild(list);

            #endregion
        }

        #region Helper

        /// <summary>
        /// Parameters: target, fraction, startValue, endValue
        /// </summary>
        /// <param name="parameters"></param>
        private static void TweenErrorSlot(object[] parameters)
        {
            object target = parameters[0];
            Tween tween = (Tween) parameters[1];
            float fraction = (float) parameters[2];
            object startValue = parameters[3];
            object endValue = parameters[4];
            Exception exception = (Exception) parameters[5];

            Component component = target as Component;
        
            Debug.LogError(string.Format(@"Interpolation error, Target: {0}{1}
fraction: {2}, startValue: {3}, endValue: {4}
exception: {5}", ComponentUtil.PathToString(component, "->"), string.IsNullOrEmpty(tween.Name) ? string.Empty : string.Format(" [{0}]", tween.Name),
                                         fraction, startValue.GetType(), endValue.GetType(),
                                         exception));

//        if (null != component)
//        {
//            Debug.LogError("Component: " + ComponentUtil.PathToString(component, "->"));
//        }
        }

        #endregion

    }
}