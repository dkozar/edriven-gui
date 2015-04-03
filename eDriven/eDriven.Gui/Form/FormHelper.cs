using System.Collections.Generic;
using eDriven.Core.Data.Collections;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;

namespace eDriven.Gui.Form
{
    public static class FormHelper
    {
        public class StringGridRow
        {
            public StringGridRow(string caption)
            {
                Caption = caption;
            }

            public string Caption;
        }

        public class KeyValueGridRow
        {
            public KeyValueGridRow(string key, object value)
            {
                Key = key;
                Value = value;
            }

            public string Key;
            public object Value;
        }
        
        
        //public static FormTextFieldItem CreateTextFormItem(string formItemId, string labelText, string defaultText)
        //{
        //    FormTextFieldItem txtItem = new FormTextFieldItem();

        //    txtItem.Id = formItemId;
        //    txtItem.NavigatorDescriptor = labelText;
        //    //txtItem.LabelStyleName = "Form.Item.Label";
        //    //txtItem.ControlStyleName = "TextField";
        //    txtItem.Text = defaultText;
        //    txtItem.PercentWidth = 100;
        //    txtItem.MarginRight = 15;

        //    return txtItem;
        //}

        //public static FormTextFieldItem CreateMultilineTextFormItem(string formItemId, string labelText, string defaultText)
        //{
        //    FormTextFieldItem txtItem = CreateTextFormItem(formItemId, labelText, defaultText);
        //    ((TextField) txtItem.Children[0]).Multiline = true;
        //    txtItem.Children[0].Height = 200;
        //    txtItem.VerticalAlign = VerticalAlign.Top;
        //    return txtItem;
        //}


        //public static FormLabelItem CreateLabelFormItem(string formItemId, string labelText, string defaultText)
        //{
        //    FormLabelItem labItem = new FormLabelItem();

        //    labItem.Id = formItemId;
        //    labItem.NavigatorDescriptor = labelText;
        //    //labItem.LabelStyleName = "Form.Item.Label";
        //    //labItem.ControlStyleName = "Form.Item.Label";
        //    labItem.Text = defaultText;

        //    return labItem;
        //}

        //public static FormHeading CreateFormHeaderItem(string labelText)
        //{
        //    FormHeading fh = new FormHeading();
        //    //fh.LabelStyleName = "FormHeading";
        //    fh.DisplayLabel = true;
        //    fh.NavigatorDescriptor = labelText;
            
            
        //    return fh;
        //}

        //public static FormHeading CreateFormGroupHeaderItem(string labelText)
        //{
        //    FormGroupHeading fh = new FormGroupHeading();
        //    //fh.LabelStyleName = "FormHeading";
        //    fh.DisplayLabel = true;
        //    fh.NavigatorDescriptor = labelText;

        //    return fh;
        //}

        //public static FormHeading CreateFormCaption(string labelText)
        //{
        //    FormHeading fh = new FormHeading();
        //    //fh.LabelStyleName = "FormCaption";
        //    fh.DisplayLabel = true;
        //    fh.NavigatorDescriptor = labelText;

        //    return fh;
        //}

        //public static FormRadioItem CreateFormRadioItem(string formItemId, string labelText, string[] options, object selectedValue)
        //{
            
        //    FormRadioItem fri = new FormRadioItem();

        //    fri.Id = formItemId;
        //    fri.NavigatorDescriptor = labelText;
        //    //fri.LabelStyleName = "Form.Item.Label";
        //    fri.ButtonDirection = LayoutDirection.Vertical;

        //    List<object> data = new List<object>();
        //    for (int i = 0; i < options.Length; i++)
        //    {
        //        data.Add(new ListItem(i+1, options[i]));
        //    }
        //    fri.DataProvider = data;

        //    fri.SelectedValue = selectedValue;

        //    return fri;

        //}

        //public static FormCheckBoxItem CreateFormCheckBoxItem(string formItemId, string labelText)
        //{
            
        //    FormCheckBoxItem fci = new FormCheckBoxItem();

        //    fci.Id = formItemId;
        //    fci.NavigatorDescriptor = labelText;
        //    //fci.LabelStyleName = "Form.Item.Label";

        //    return fci;

        //}
        
        //public static FormButtonItem CreateFormButtonItem(string formItemId, string labelText, string buttonText)
        //{
        //    FormButtonItem buttonItem = new FormButtonItem();

        //    buttonItem.Id = formItemId;
        //    buttonItem.DisplayLabel = true;
        //    buttonItem.NavigatorDescriptor = labelText;
        //    //buttonItem.LabelStyleName = "Form.Item.Label";
        //    //buttonItem.ControlStyleName = "OptionButton";
        //    buttonItem.AddButton(new FormButtonItem.FormButtonDescriptor(buttonText));

        //    return buttonItem;
            
        //}

        //public static FormItem CreateSliderFormItem(string formItemId, string labelText, float min, float max, float initialValue, string minLabel, string maxLabel)
        //{
        //    FormItem item = new FormItem();
        //    item.Id = formItemId;
        //    item.DisplayLabel = true;
        //    item.NavigatorDescriptor = labelText;
        //    //item.LabelStyleName = "Form.Item.Label";

        //    Label labelMin = new Label();
        //    labelMin.Text = minLabel;
        //    labelMin.PaddingRight = 16;

        //    Slider slider = new Slider();
        //    slider.MinValue = min;
        //    slider.MaxValue = max;
        //    slider.PercentWidth = 100;
        //    slider.Value = initialValue;
        //    slider.NumberMode = SliderNumberMode.Float;
        //    slider.Enabled = true;
        //    slider.FocusEnabled = true;

        //    Label labelMax = new Label();
        //    labelMax.Text = maxLabel;
        //    labelMax.PaddingLeft = 16;

        //    item.AddChild(labelMin);
        //    item.AddChild(slider);
        //    item.AddChild(labelMax);

        //    return item;
        //}

        //public static FormItem CreateListFormItem(string formItemId, string labelText, string[] listTexts)
        //{

        //    DataGrid<StringGridRow> dataGrid = new DataGrid<StringGridRow>();
        //    dataGrid.PercentWidth = 100;
        //    dataGrid.PercentHeight = 100;
        //    dataGrid.PaddingRight = 16;
        //    dataGrid.RowStyleName = "DataGridRow";
        //    dataGrid.SelectedRowStyleName = "DataGridRowSelected";
        //    dataGrid.HeaderHeight = 0;
            
        //    DataGridColumn<StringGridRow> dc1 = new DataGridColumn<StringGridRow>("Caption", "");
        //    dc1.PercentWidth = 100;
        //    dataGrid.AddColumn(dc1);
            
        //    List<StringGridRow> l = new List<StringGridRow>();

        //    foreach (string s in listTexts)
        //    {
        //        l.Add(new StringGridRow(s));
        //    }

        //    ObservableCollection<StringGridRow> list = new ObservableCollection<StringGridRow>(l);
        //    dataGrid.DataProvider = list;

        //    FormItem item = new FormItem();
        //    item.Id = formItemId;
        //    item.DisplayLabel = true;
        //    item.Label = labelText;
        //    //item.LabelStyleName = "Form.Item.Label";

        //    item.AddChild(dataGrid);

        //    return item;

        //}

        //public  static FormItem CreateCheckListFormItem(string formItemId, string labelText, string[] checkTexts)
        //{

        //    DataGrid<StringGridRow> dataGrid = new DataGrid<StringGridRow>();
        //    dataGrid.PercentWidth = 100;
        //    dataGrid.PercentHeight = 100;
        //    dataGrid.PaddingRight = 16;
        //    dataGrid.RowStyleName = "DataGridRow";
        //    dataGrid.SelectedRowStyleName = "DataGridRow";
        //    dataGrid.HeaderHeight = 0;


        //    DataGridColumn<StringGridRow> dc1 = new DataGridColumn<StringGridRow>("Caption", "");
        //    dc1.PercentWidth = 75;
        //    dataGrid.AddColumn(dc1);
            
        //    DataGridColumn<StringGridRow> dc2 = new DataGridColumn<StringGridRow>("", "");
        //    dc2.PercentWidth = 25;
        //    dc2.ItemRendererType = typeof (SelectActionRenderer);
        //    dataGrid.AddColumn(dc2);


        //    List<StringGridRow> l = new List<StringGridRow>();

        //    foreach (string s in checkTexts)
        //    {
        //        l.Add(new StringGridRow(s));
        //    }

        //    ObservableCollection<StringGridRow> list = new ObservableCollection<StringGridRow>(l);
        //    dataGrid.DataProvider = list;

            

        //    FormItem item = new FormItem();
        //    item.Id = formItemId;
        //    item.DisplayLabel = true;
        //    item.Label = labelText;
        //    //item.LabelStyleName = "Form.Item.Label";

        //    item.AddChild(dataGrid);

        //    return item;

        //}

        //public static FormItem CreateKeyValueListFormItem(string formItemId, string labelText, string keyColCaption, string valueColCaption, KeyValuePair<string, object>[] data)
        //{

        //    DataGrid<KeyValueGridRow> dataGrid = new DataGrid<KeyValueGridRow>();
        //    dataGrid.PercentWidth = 100;
        //    dataGrid.PercentHeight = 100;
        //    dataGrid.PaddingRight = 16;
        //    dataGrid.RowStyleName = "DataGridRow";
        //    dataGrid.SelectedRowStyleName = "DataGridRowSelected";


        //    DataGridColumn<KeyValueGridRow> dc1 = new DataGridColumn<KeyValueGridRow>("Key", keyColCaption);
        //    dc1.PercentWidth = 50;
        //    dataGrid.AddColumn(dc1);

        //    DataGridColumn<KeyValueGridRow> dc2 = new DataGridColumn<KeyValueGridRow>("Value", valueColCaption);
        //    dc2.PercentWidth = 50;
        //    dataGrid.AddColumn(dc2);
            
        //    List<KeyValueGridRow> l = new List<KeyValueGridRow>();

        //    foreach (KeyValuePair<string, object> kvp in data)
        //    {
        //        l.Add(new KeyValueGridRow(kvp.Key, kvp.Value));
        //    }

        //    ObservableCollection<KeyValueGridRow> list = new ObservableCollection<KeyValueGridRow>(l);
        //    dataGrid.DataProvider = list;
            

        //    FormItem item = new FormItem();
        //    item.Id = formItemId;
        //    item.DisplayLabel = true;
        //    item.Label = labelText;
        //    //item.LabelStyleName = "Form.Item.Label";

        //    item.AddChild(dataGrid);

        //    return item;

        //}

    }
}