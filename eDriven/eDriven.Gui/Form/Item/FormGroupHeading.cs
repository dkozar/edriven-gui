//using eDriven.Gui.Components;
//using EventHandler = eDriven.Core.Events.EventHandler;

//namespace eDriven.Gui.Form
//{
//    public class FormGroupHeading : FormHeading
//    {
        
//        private float _height;
//        private Button _button;
//        private float _marginTop;
//        private float _marginBtm;

//        public FormGroupHeading()
//        {
//            // ctor
//        }

//        protected override void CreateChildren()
//        {
//            base.CreateChildren();

//            _button = new Button();
//            //_button.StyleName = "OptionButton";
//            _button.Width = 32;
//            //_button.MarginRight = 8;
//            _button.Text = "-";
//            _button.Click += new EventHandler(delegate
//                                                  {
//                                                      Component next =
//                                                          Parent.GetChildAt(Parent.GetChildIndex(this) + 1) as Component;

//                                                      if (next is FormItem && (next as FormItem).Children.Count > 0)
//                                                      {
//                                                          Component component = (next as FormItem).GetChildAt((next as FormItem).Children.Count - 1) as Component;
                                                          
//                                                          if (_button.Text == "-")
//                                                          {
//                                                              _height = component.Height;
//                                                              //_marginTop = next.MarginTop;
//                                                              //_marginBtm = next.MarginBottom;

//                                                              //next.MarginTop = 0;
//                                                              //next.MarginBottom = 0;
//                                                              component.Height = 0;

//                                                              _button.Text = "+";
//                                                          }
//                                                          else if (_button.Text == "+")
//                                                          {
//                                                              component.Height = _height;
//                                                              //next.MarginTop = _marginTop;
//                                                              //next.MarginBottom = _marginBtm;

//                                                              _button.Text = "-";
//                                                          }
//                                                      }
//                                                  }
//                );

//            AddChild(_button);
//        }

        
//    }
//}