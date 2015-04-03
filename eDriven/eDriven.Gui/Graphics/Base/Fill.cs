using UnityEngine;

namespace eDriven.Gui.Graphics.Base
{
    public class Fill : DrawBase, IFill
    {
        public Fill()
        {
        }

        public Fill(Color color)
        {
            _color = color;
        }
    }
}