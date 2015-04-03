using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Components;

namespace eDriven.Gui.States
{
    /// <summary>
    /// Represents a single state
    /// </summary>
    public class State : EventDispatcher
    {
        private readonly StateTable _properties;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public State(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        public State(string name, StateTable properties)
        {
            Name = name;
            _properties = properties;
        }

        private bool _initialized;

        /// <summary>
        /// State name
        /// </summary>
// ReSharper disable once FieldCanBeMadeReadOnly.Global
        public string Name;

        /// <summary>
        /// Based on state name
        /// </summary>
// ReSharper disable once FieldCanBeMadeReadOnly.Global
        public string BasedOn;

        /// <summary>
        /// The list of overrides
        /// </summary>
        public List<IOverride> Overrides = new List<IOverride>();

        public List<string> StateGroups = new List<string>();

        internal void Initialize()
        {
            if (!_initialized)
            {
                foreach (IOverride t in Overrides)
                {
                    t.Initialize();
                }
                _initialized = true;
            }
        }

        internal void DispatchEnterState()
        {
            if (HasEventListener(FrameworkEvent.ENTER_STATE))
                DispatchEvent(new FrameworkEvent(FrameworkEvent.ENTER_STATE));
        }

        internal void DispatchExitState()
        {
            if (HasEventListener(FrameworkEvent.EXIT_STATE))
                DispatchEvent(new FrameworkEvent(FrameworkEvent.EXIT_STATE));
        }

        public override string ToString()
        {
            return string.Format(@"Name: ""{0}"", BasedOn: ""{1}"", Overrides: {2}, StateGroups: {3}", Name, BasedOn, Overrides.Count, StateGroups.Count);
        }
    }
}
