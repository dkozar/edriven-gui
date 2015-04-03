using UnityEngine;

namespace eDriven.Gui.Graphics.Base
{
    public delegate Color DrawFunctionDelegate(int x, int y, Color c);

    public class DrawBase : IDraw
    {
        public Color _color = Color.white;
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private DrawFunctionDelegate _drawFunction;
        public DrawFunctionDelegate DrawFunction
        {
            get { return _drawFunction; }
            set { _drawFunction = value; }
        }
    }
}