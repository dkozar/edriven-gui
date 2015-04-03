namespace eDriven.Gui.Graphics.Base
{
    public interface IGraphicsConstrainClient
    {
        float? Left { get; set; }
        float? Right { get; set; }
        float? Top { get; set; }
        float? Bottom { get; set; }
    }
}