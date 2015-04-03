using System.Collections.Generic;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    [HostComponent(typeof(Button))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xcccccc)]
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]

    public class DropDownListButtonSkin : Skin
    {
        public DropDownListButtonSkin()
        {
            MinHeight = 25;

            States = new List<State>(new[]
            {
                new State("up"), 
                new State("over"),
                new State("down"),
                new State("disabled"),
                new State("upAndSelected"), 
                new State("overAndSelected"),
                new State("downAndSelected"),
                new State("disabledAndSelected")
            });
        }

        #region Members

        private RectShape _background;
        private RectShape _background2;

        #endregion

        #region Skin parts

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Label LabelDisplay;
        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        protected override void CreateChildren()
        {
            //Debug.Log("Button skin creating children");
            base.CreateChildren();

            #region Background

            _background = new RectShape
            {
                //Id = "background",
                //Color = (Color)GetStyle("backgroundColor"),
                Left = 0,
                Right = 24,
                Top = 0,
                Bottom = 0
            };
            _background.SetStyle("backgroundStyle", PopupButtonStyle.Instance);
            AddChild(_background);

            #endregion

            #region Arrow button

            _background2 = new RectShape
            {
                Right = 0,
                Top = 0,
                Bottom = 0,
                Width = 25,
                Height = 25
            };
            _background2.SetStyle("backgroundStyle", ArrowButtonStyle.Instance);
            AddChild(_background2);

            #endregion

            //#region Label

            //LabelDisplay = new Label
            //{
            //    HorizontalCenter = 0,
            //    VerticalCenter = 0,
            //    Right = 10,
            //    Left = 10,
            //    Top = 10,
            //    Bottom = 10,
            //    MouseEnabled = false
            //};
            ////LabelDisplay.SetStyle("color", GetStyle("textColor"));
            //AddChild(LabelDisplay);

            //#endregion
        }
    }
}