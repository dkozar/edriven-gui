/*using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;

namespace eDriven.Gui.Components
{
    [HostComponent(typeof(SkinnableContainer))]

    public class SkinnableContainerSkin2 : Skin
    {
        /* skin part #1#
        public Group ContentGroup;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            RectShape background = new RectShape { Color = UnityEngine.Color.green };
            background.Left = background.Right = background.Top = background.Bottom = 0;
            AddChild(background);

            RectShape box = new RectShape { Color = UnityEngine.Color.red };
            box.Left = box.Right = box.Top = box.Bottom = 30;
            AddChild(box);

            box = new RectShape { Color = UnityEngine.Color.yellow };
            box.Left = box.Right = box.Bottom = 50;
            box.PercentHeight = 50;
            AddChild(box);

// ReSharper disable InconsistentNaming
            ContentGroup = new Group {Layout = new AbsoluteLayout()};
// ReSharper restore InconsistentNaming
            ContentGroup.Left = ContentGroup.Right = ContentGroup.Top = ContentGroup.Bottom = 0;
            AddChild(ContentGroup);
        }
    }
}*/