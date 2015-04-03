using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;

namespace eDriven.Gui.Components
{
    [HostComponent(typeof(SkinnableContainer))]

    public class SkinnableContainerSkin : Skin
    {
        /* skin part */
        public Group ContentGroup;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            RectShape background = new RectShape();
            //background.SetStyle();
            background.Left = background.Right = background.Top = background.Bottom = 0;
            AddChild(background);

            // ReSharper disable InconsistentNaming
            ContentGroup = new Group {Layout = new AbsoluteLayout()};
            // ReSharper restore InconsistentNaming
            ContentGroup.Left = ContentGroup.Right = ContentGroup.Top = ContentGroup.Bottom = 0;
            AddChild(ContentGroup);
        }
    }
}