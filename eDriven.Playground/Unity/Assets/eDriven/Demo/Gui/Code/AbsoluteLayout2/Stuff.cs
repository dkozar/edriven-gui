using Assets.eDriven.Skins;
using eDriven.Core.Data.Collections;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Helpers;
using UnityEngine;

namespace Assets.eDriven.Demo.Gui.Code.AbsoluteLayout2
{
    public class Stuff : Window
    {
        public Stuff()
        {
            Title = "Panel with controls";
            Icon = Resources.Load<Texture>("Icons/star");
            SkinClass = typeof (PanelSkin2);
            
            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Left,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            };

            Plugins.Add(new TabManager());
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            #region List 1

            List list1 = new List
                             {
                                 PercentWidth = 100,
                                 PercentHeight = 100,
                                 MinWidth = 200,
                                 MinHeight = 100,
                                 Height = 200,
                                 DataProvider = new ArrayList(EasingHelper.GetEasingList()),
                                 SelectedItem = "Bounce"
                             };
            AddContentChild(list1);

            #endregion

            #region List 2

            List list2 = new List
                             {
                                 PercentWidth = 100,
                                 PercentHeight = 100,
                                 MinHeight = 100,
                                 Height = 200,
                                 DataProvider = new ArrayList(
                                     Application.isWebPlayer
                                         ? ResolutionHelper.GetResolutionList()
                                         : ResolutionHelper.GetDummyResolutionList())
                             };
            AddContentChild(list2);

            #endregion

            #region Drow down list

            DropDownList dropDownList = new DropDownList
                               {
                                   PercentWidth = 100,
                                   MinWidth = 200,
                                   DataProvider = new ArrayList(
                                     Application.isWebPlayer
                                         ? ResolutionHelper.GetResolutionList()
                                         : ResolutionHelper.GetDummyResolutionList())
                               };
            AddContentChild(dropDownList);

            dropDownList = new DropDownList
                      {
                          PercentWidth = 100,
                          MinWidth = 200,
                          DataProvider = new ArrayList(new System.Collections.Generic.List<object>
                                             {
                                                 new ListItem(1, "Zagreb"),
                                                 new ListItem(2, "Osijek"),
                                                 new ListItem(3, "Rijeka"),
                                                 new ListItem(4, "Split"),
                                                 new ListItem(5, "Ljubljana"),
                                                 new ListItem(6, "Wiena"),
                                                 new ListItem(7, "Munich"),
                                                 new ListItem(8, "Berlin")
                                             })
                      };
            AddContentChild(dropDownList);

            List list3 = new List
                             {
                                 PercentWidth = 100,
                                 MinHeight = 100,
                                 Height = 200,
                                 DataProvider = new ArrayList(EasingHelper.GetEasingInOutList()),
                                 SelectedItem = "EaseIn"
                             };
            AddContentChild(list3);

            #endregion

            #region Button

            Button btn = new Button
            {
                Text = "Show alert",
                SkinClass = typeof (ImageButtonSkin),
                Icon = Resources.Load<Texture>("Icons/tab-comment")
                //StyleDeclaration = new StyleDeclaration(RectButtonSkin.Instance, "button"),
            };

            btn.Click += delegate
            {
                Alert.Show("Hi!", "This is the alert", AlertButtonFlag.Ok);
            };
            ControlBarGroup.AddChild(btn);

            #endregion
        }
    }
}