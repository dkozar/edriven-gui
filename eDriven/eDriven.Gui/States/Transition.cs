using eDriven.Animation;

namespace eDriven.Gui.States
{
    /// <summary>
    /// 
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// 
        /// </summary>
// ReSharper disable once InconsistentNaming
        internal const string ALL_STATES = "*";

        /// <summary>
        /// 
        /// </summary>
        public TweenBase Effect;

        /// <summary>
        /// 
        /// </summary>
        public string FromState = ALL_STATES;

        /// <summary>
        /// 
        /// </summary>
        public string ToState = ALL_STATES;

        /// <summary>
        /// 
        /// </summary>
        public bool AutoReverse;
    }
}
