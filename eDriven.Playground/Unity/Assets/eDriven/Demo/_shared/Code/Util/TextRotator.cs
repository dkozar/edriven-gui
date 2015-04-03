using eDriven.Core.Events;
using eDriven.Core.Util;

namespace Assets.eDriven.Demo._shared.Code.Util
{
    public class TextRotator
    {
        /// <summary>
        /// Callback signature
        /// </summary>
        public delegate void CallbackFunction(string line);

        private readonly Timer _timer;

        public string[] Lines;

        public float Delay;

        public CallbackFunction Callback;

        public TextRotator()
        {
            _timer = new Timer {TickOnStart = true};
            _timer.Tick += TimerHandler;
        }

        public void Start()
        {
            _timer.Delay = Delay;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private int _count;
        private void TimerHandler(Event e)
        {
            Callback(Lines[_count]);

            _count++;
            if (_count >= Lines.Length)
                _count = 0;
        }
    }
}