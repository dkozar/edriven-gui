namespace eDriven.Gui.Components
{
    public class Progress
    {
        public int Queued
        {
            get
            {
                return Finished - Active - Finished;
            }
        }

        public int Active;
        public int Finished;
        public int Total;

        public Progress(int finished, int total)
        {
            Finished = finished;
            Total = total;
        }

        public Progress(int active, int finished, int total)
        {
            Active = active;
            Finished = finished;
            Total = total;
        }

        public override string ToString()
        {
            return string.Format(@"[{0} Queued; {1} Active; {2} Finished; {3} Total]", Queued, Active, Finished, Total);
        }
    }
}