using System.Collections.Generic;
using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using eDriven.Gui.States;

namespace eDriven.Gui.Components
{
    /*[Style(Name = "firstButtonSkinClass", Type = typeof(Type), Default = typeof(ButtonBarFirstButtonSkin))]
    [Style(Name = "middleButtonSkinClass", Type = typeof(Type), Default = typeof(ButtonBarMiddleButtonSkin))]
    [Style(Name = "lastButtonSkinClass", Type = typeof(Type), Default = typeof(ButtonBarLastButtonSkin))]*/

    /// <summary>
    /// Button bar default skin
    /// </summary>
    public class ButtonBarSkin : Skin
    {
        public ButtonBarSkin()
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

        public IFactory FirstButton/* = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarFirstButtonSkin>()*/;

        public IFactory MiddleButton/* = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarMiddleButtonSkin>()*/;

        public IFactory LastButton/* = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarLastButtonSkin>()*/;

        public DataGroup DataGroup;

// ReSharper restore UnusedMember.Global
// ReSharper restore CSharpWarnings::CS1591
// ReSharper restore NotAccessedField.Global

        protected override void CreateChildren()
        {
            base.CreateChildren();

            FirstButton = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarFirstButtonSkin>();
            //FirstButton = new SkinnedItemRendererFactory(typeof(ButtonBarButton), (Type) GetStyle("firstButtonSkinClass"));

            MiddleButton = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarMiddleButtonSkin>();
            LastButton = new SkinnedItemRendererFactory<ButtonBarButton, ButtonBarLastButtonSkin>();

            DataGroup = new DataGroup
            {
                PercentWidth = 100,
                PercentHeight = 100,
                Layout = new HorizontalLayout()
            };
            AddChild(DataGroup);
        }

        /*protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            FirstButton = new SkinnedItemRendererFactory(typeof(ButtonBarButton), (Type)GetStyle("firstButtonSkinClass"));
        }*/
    }
}