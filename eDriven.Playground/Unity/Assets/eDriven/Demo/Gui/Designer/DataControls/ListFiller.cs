using eDriven.Core.Data.Collections;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

public class ListFiller : MonoBehaviour {

    void ComponentInstantiated(Component component)
    {
        List list = (List)component;
        FillList(list);
    }

    void InitializeComponent(Component component)
    {
        //List list = (List)component;
        //FillList(list);
    }

    //public void FillList(List list)
    //{
    //    list.DataProvider.Add(new ListItem(1, "One"));
    //    list.DataProvider.Add(new ListItem(2, "Two"));
    //    list.DataProvider.Add(new ListItem(3, "Three"));
    //}

    public static void FillList(List list)
    {
        System.Collections.Generic.List<object> items = new System.Collections.Generic.List<object>
        {
            new ListItem(1, "One"),
            new ListItem(2, "Two"),
            new ListItem(3, "Three")
        };
        list.DataProvider = new ArrayList(items);
    }
}
