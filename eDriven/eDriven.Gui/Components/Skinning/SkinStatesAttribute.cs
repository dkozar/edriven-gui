using System;

namespace eDriven.Gui.Components
{
    public class SkinStatesAttribute : Attribute
    {
        private readonly string[] _states;
        public string[] States
        {
            get { return _states; }
        }

        public SkinStatesAttribute(params string[] states)
        {
            _states = states;
        }
    }
}