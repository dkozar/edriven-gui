using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Simple eDriven.Gui component example
    /// </summary>
    /// <remarks>Copyright (c) 2013 by Danko Kozar</remarks>
    public class Simple : Component
    {
        public Texture Texture; // example of the exposed property

        public Simple() // constructor
        {
            // defaults
            Width = 200;
            Height = 150;
        }

        protected override void Render() // render
        {
            GUI.DrawTexture(RenderingRect, Texture, ScaleMode.ScaleAndCrop, true, 1f); // RenderingRect is provided by the system
        }

        protected override void Measure()
        {
            // override measuring
            // this component won't measure itself since it must have the styling implemented for doing that
            // instead it should be sized from the outside using Width and Height properties
        }
    }
}