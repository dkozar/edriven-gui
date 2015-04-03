using Assets.eDriven.Demo.Gui.Code;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using eDriven.Gui.Styles;

// ReSharper disable once CheckNamespace
namespace Assets.eDriven.Demo
{
    [Style(Name = "skinClass", Default = typeof(ToolbarSkin))]

    public class Toolbar : SkinnableContainer
    {
        /*private readonly TweenFactory _showEffect = new TweenFactory(
            new Sequence(
                new ToolbarShow()
            )
        );*/

        public Toolbar()
        {
            PercentWidth = 100;
            /*SetStyle("showBackground", true);
            SetStyle("backgroundColor", ColorMixer.FromHex(0x439dde).ToColor());*/
            //SetStyle("addedEffect", _showEffect);

            Layout = new HorizontalLayout
            {
                VerticalAlign = VerticalAlign.Middle,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            };
        }
    }
}