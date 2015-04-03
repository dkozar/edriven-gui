using UnityEngine;

namespace eDriven.Gui.Graphics.Base
{
    public interface IDraw
    {
        DrawFunctionDelegate DrawFunction { get; set; }
        
        Color Color { get; set; }
    }
}