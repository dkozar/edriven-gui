namespace eDriven.Gui.Components
{
    public interface IProgramaticStyleClient
    {
        /// <summary>
        /// NormalColor background
        /// </summary>
        ProgramaticStyle Style { get; /*set;*/ }

        void SetMouseAbove(bool above);
    }
}