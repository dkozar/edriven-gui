using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Util;

namespace Assets.eDriven.Demo.Gui.Code
{
    public class ToolbarSkin : Skin
    {
        #region Skin parts

        // ReSharper disable MemberCanBePrivate.Global
        /* skin part */
        ///<summary>Content group
        ///</summary>
        public Group ContentGroup;

        #endregion

        #region Members

        private RectShape _background;

        #endregion

        public ToolbarSkin()
        {
            States = new List<State>
            {
                new State("normal"),
                new State("disabled")
            };
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Background

            _background = new RectShape
                              {
                                  Id = "background",
                                  Left = 0, Right = 0, Top = 0, Bottom = 0
                              };
            _background.SetStyle("backgroundColor", ColorMixer.FromHex(0x439dde).ToColor());
            AddChild(_background);

            #endregion
            
            #region Content group

            ContentGroup = new Group
            {
                Id = "contentGroup",
                //Left = 6,
                //Right = 6,
                //Top = 50,
                //Bottom = 50
            };
            AddChild(ContentGroup);

            #endregion
        }
    }
}
