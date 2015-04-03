namespace eDriven.Gui.Components
{
    public abstract class ProgressIndicatorBase : SkinnableComponent, IProgressIndicator
    {
        /// <summary>
        /// Sets the message
        /// </summary>
        public abstract string Message { get; set; }

        /// <summary>
        /// Starts animation
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Stops animation
        /// </summary>
        public abstract void Stop();
    }
}