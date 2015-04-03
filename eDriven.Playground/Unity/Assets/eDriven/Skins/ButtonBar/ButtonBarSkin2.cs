using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using eDriven.Gui.States;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Button bar default skin
    /// </summary>
    public class ButtonBarSkin2 : Skin
    {
        public ButtonBarSkin2()
        {
            States = new List<State>
            {
                new State("normal"),
                new State("disabled")
            };
        }

// ReSharper disable NotAccessedField.Global
// ReSharper disable CSharpWarnings::CS1591
// ReSharper disable UnusedMember.Global

        public IFactory FirstButton;

        public IFactory MiddleButton;

        public IFactory LastButton;

        public DataGroup DataGroup;

// ReSharper restore UnusedMember.Global
// ReSharper restore CSharpWarnings::CS1591
// ReSharper restore NotAccessedField.Global

        protected override void CreateChildren()
        {
            base.CreateChildren();

            FirstButton = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarFirstButtonSkin>();
            MiddleButton = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarMiddleButtonSkin>();
            LastButton = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarLastButtonSkin>();

            DataGroup = new DataGroup
            {
                PercentWidth = 100,
                PercentHeight = 100,
                Layout = new HorizontalLayout { Gap = 0 }
            };
            AddChild(DataGroup);
        }
    }
}