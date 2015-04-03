namespace eDriven.Gui.Components
{
    public interface IProgressBar
    {
        /// <summary>
        /// Updates progress
        /// </summary>
        /// <param name="progress">Progress that has to be supplied by developer</param>
        void UpdateProgress(Progress progress);
    }
}