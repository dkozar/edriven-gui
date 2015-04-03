using eDriven.Gui.Containers;

namespace eDriven.Gui.Components
{
    public abstract class ProgressBarBase : Group, IProgressBar
    {
        /// <summary>
        /// Currently active elements
        /// </summary>
        public virtual int Active { get; set; }

        /// <summary>
        /// Currently finished elements
        /// </summary>
        public virtual int Finished { get; set; }

        /// <summary>
        /// Total number of elements
        /// </summary>
        public virtual int Total { get; set; }

        public void Reset()
        {
            Finished = 0;
        }

        public abstract void Show();

        public abstract void Hide();

        internal abstract void InitGraphics();

        internal abstract void UpdateGraphics();

        //public override void Initialize()
        //{
        //    LayoutDescriptor = LayoutDescriptor.Horizontal;

        //    base.Initialize();
        //}

        #region Implementation of IProgressBar

        /// <summary>
        /// Updates progress
        /// </summary>
        /// <param name="progress">Progress that has to be supplied by developer</param>
        public abstract void UpdateProgress(Progress progress);

        #endregion
    }
}