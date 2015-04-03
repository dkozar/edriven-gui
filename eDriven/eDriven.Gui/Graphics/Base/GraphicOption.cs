namespace eDriven.Gui.Graphics.Base
{
    public class GraphicOption
    {
        public GraphicOptionType Type;
        public object Value;

        public GraphicOption(GraphicOptionType type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}