namespace eDriven.Gui.Components
{
    public interface IProgressIndicator
    {
        void Play();
        void Stop();
        string Message { get; set; }
    }
}