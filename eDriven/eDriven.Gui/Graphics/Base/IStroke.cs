using eDriven.Gui.Geom;

namespace eDriven.Gui.Graphics.Base
{
    public interface IStroke : IDraw
    {
        BorderMetrics Border { get; set; }
        //Color Color { get; set; }
    }
}