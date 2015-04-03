using System.Collections;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Form;

namespace Assets.eDriven.Demo.Gui.Code.GridDemo
{
    public class ExamplePopupEditor : Dialog
    {
        readonly Hashtable _buttonStyles = new Hashtable {{"cursor", "pointer"}};

        private TextField _txtFirstName;
        private TextField _txtLastName;
        private NumericStepper _nsAge;
        private CheckBox _chkDrivingLicense;

        private ExampleItem _item;

        private bool _dataChanged;
        private object _data;
        public override object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                _item = (ExampleItem)Data;
                _dataChanged = true;
                InvalidateProperties();
            }
        }

        public ExamplePopupEditor()
        {
            Width = 400;
            //Height = 600;
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            Form form = new Form { PercentWidth = 100, Left = 10, Right = 10, Top = 10, Bottom = 10 };
            AddContentChild(form);

            #region Text Fields

            _txtFirstName = new TextField { PercentWidth = 100/*, Optimized = true*/ };
            _txtFirstName.TextChange += delegate
            {
                // update item
                _item.FirstName = _txtFirstName.Text;
            };
            form.AddField("first_name", "First name:", _txtFirstName);

            _txtLastName = new TextField { PercentWidth = 100/*, Optimized = true*/ };
            _txtLastName.TextChange += delegate
            {
                // update item
                _item.LastName = _txtLastName.Text;
            };
            form.AddField("last_name", "Last name:", _txtLastName);

            _nsAge = new NumericStepper { Width = 60, FocusEnabled = true, HighlightOnFocus =  true };
            _nsAge.ValueCommit += delegate
            {
                // update item
                _item.Age = (int)_nsAge.Value;
            };
            form.AddField("age", "Age:", _nsAge);

            _chkDrivingLicense = new CheckBox();
            _chkDrivingLicense.Change += delegate
            {
                // update item
                _item.DrivingLicense = _chkDrivingLicense.Selected;
            };
            form.AddField("driving_license", "Driving license:", _chkDrivingLicense);

            #endregion

            ControlBarGroup.AddChild(new Spacer {PercentWidth = 100});

            #region Close button

            var button = new Button
            {
                SkinClass = typeof (ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/cancel"),
                FocusEnabled = false,
                Styles = _buttonStyles
            };
            button.Click += delegate
            {
                DispatchEvent(new CloseEvent(CloseEvent.CLOSE));
            };
            ToolGroup.AddChild(button);

            #endregion
            
            #region Control bar buttons

            button = new Button
            {
                Text = "Close dialog",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                FocusEnabled = false,
                Styles = _buttonStyles
            };
            button.Click += delegate
            {
                DispatchEvent(new CloseEvent(CloseEvent.CLOSE));
            };
            ControlBarGroup.AddChild(button);

            #endregion
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_dataChanged)
            {
                _dataChanged = false;

                var item = (ExampleItem) _data;
                _txtFirstName.Text = item.FirstName;
                _txtLastName.Text = item.LastName;
                _nsAge.Value = item.Age;
                _chkDrivingLicense.Selected = item.DrivingLicense;
            }
        }

        public override void SetFocus()
        {
            Defer(delegate
            {
                _txtFirstName.SetFocus();
            }, 1);
            //_txtFirstName.SetFocus();
        }
    }
}