using eDriven.Gui.Geom;

namespace eDriven.Gui.Graphics.Base
{
    public class Stroke : DrawBase, IStroke
    {
        public Stroke(int weight)
        {
            _border = new BorderMetrics(weight);
        }

        public Stroke(BorderMetrics border)
        {
            _border = border;
        }
        
        private BorderMetrics _border;
        public BorderMetrics Border
        {
            get { return _border; }
            set { _border = value; }
        }
    }
}