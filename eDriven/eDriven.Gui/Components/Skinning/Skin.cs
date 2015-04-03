namespace eDriven.Gui.Components
{
    public class Skin : Group
    {
        public override float?  ExplicitMinWidth
        {
            get 
            { 
                if (Parent is SkinnableComponent)
                {
                    float? parentExplicitMinWidth = ((SkinnableComponent)Parent).ExplicitMinWidth;
                    if (null != parentExplicitMinWidth)
                        return parentExplicitMinWidth;
                }
                return base.ExplicitMinWidth;
            }
            set 
            { 
                base.ExplicitMinWidth = value;
            }
        }

        public override float?  ExplicitMinHeight
        {
            get 
            { 
                if (Parent is SkinnableComponent)
                {
                    float? parentExplicitMinHeight = ((SkinnableComponent)Parent).ExplicitMinHeight;
                    if (null != parentExplicitMinHeight)
                        return parentExplicitMinHeight;
                }
                return base.ExplicitMinHeight;
            }
            set 
            { 
                base.ExplicitMinHeight = value;
            }
        }

        public override float? ExplicitMaxWidth
        {
            get
            {
                if (Parent is SkinnableComponent)
                {
                    float? parentExplicitMaxWidth = ((SkinnableComponent)Parent).ExplicitMaxWidth;
                    if (null != parentExplicitMaxWidth)
                        return parentExplicitMaxWidth;
                }
                return base.ExplicitMaxWidth;
            }
            set
            {
                base.ExplicitMaxWidth = value;
            }
        }

        public override float? ExplicitMaxHeight
        {
            get
            {
                if (Parent is SkinnableComponent)
                {
                    float? parentExplicitMaxHeight = ((SkinnableComponent)Parent).ExplicitMaxHeight;
                    if (null != parentExplicitMaxHeight)
                        return parentExplicitMaxHeight;
                }
                return base.ExplicitMaxHeight;
            }
            set
            {
                base.ExplicitMaxHeight = value;
            }
        }

        public override object GetStyle(string styleName)
        {
            /**
             * Important!
             * The skin should use the same styles as component
             * We are looking up styles in th efolowing order:
             * 1. in the skin itself
             * 2. in the owner component
             * */
            var s = base.GetStyle(styleName);
            if (null != s)
                return s;

            // style not found in the skin. Look to the component
            var ownerAsIStyleClient = Owner as IStyleClient;
            if (null != ownerAsIStyleClient)
            {
                return ownerAsIStyleClient.GetStyle(styleName);
            }

            return null;
        }
    }
}