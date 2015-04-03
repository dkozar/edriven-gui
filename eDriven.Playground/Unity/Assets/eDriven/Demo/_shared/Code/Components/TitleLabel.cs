using System.Collections.Generic;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using UnityEngine;
using Random=System.Random;

namespace Assets.eDriven.Demo.Components
{
    public class TitleLabel : Label
    {
        private string _finalText;

        private List<char> _chars;
        private List<bool> _done;
        private int _notDoneCount;
        private int _len;

// ReSharper disable ConvertToConstant.Global
        public string Replacements = @"$$$$$$$$$$$$$$$$$$$\\___+-_"; // NOTE: $ = blank
// ReSharper restore ConvertToConstant.Global
/*
        public string Blank = "$";
*/

// ReSharper disable ConvertToConstant.Global
        public float Fps = 40;
// ReSharper restore ConvertToConstant.Global
        private float _delay;
        private float _last;

        private int _rlen;

        private readonly Random _rnd = new Random();

// ReSharper disable ConvertToConstant.Global
        public float Factor = 0.8f;
// ReSharper restore ConvertToConstant.Global

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                _finalText = value;
                _len = _finalText.Length;

                _rlen = Replacements.Length;
                
                _chars = new List<char>(_len);
                _done = new List<bool>(_len);

                _notDoneCount = _len;

                for (int i = 0; i < _len; i++)
                {
                    _chars.Add(new char()); //_finalText[i];
                    _done.Add(false);
                }

                _phase = 1;

                if (Fps > 0)
                    _delay = 1/Fps;

                _last = Time.time;

                //Debug.Log("Connecting");
                SystemManager.Instance.PreRenderSignal.Connect(PreRenderSlot);
            }
        }

        #region Implementation of ISlot

        private int _phase;

        public void PreRenderSlot(params object[] parameters)
        {
            if (Fps > 0)
            {
                if (Time.time - _last < _delay)
                    return;

                _last = Time.time;
            }

            for (int i = 0; i < _len; i++)
            {
                if (!_done[i]) {

                    if (_rnd.Next((int) (_notDoneCount * Factor)) == 0)
                    {
                        _chars[i] = _phase == 1 ? Replacements[_rnd.Next(_rlen)] : _finalText[i];
                        _done[i] = true;
                        _notDoneCount--;
                    }
                    else
                    {
                        _chars[i] = Replacements[_rnd.Next(_rlen)];
                    }
                }
            }

            string s = new string(_chars.ToArray());
            s = s.Replace("$", "");
            base.Text = s;

            if (!_done.Exists(delegate(bool b) { return !b; }))
            {
                if (_phase == 1)
                {
                    _done.Clear();
                    for (int i = 0; i < _len; i++)
                    {
                        _done.Add(false);
                    }
                    _notDoneCount = _len;
                    _phase = 2;
                }
                else
                {
                    //Debug.Log("Disconnecting");
                    SystemManager.Instance.PreRenderSignal.Disconnect(PreRenderSlot);
                }
            }
        }

        #endregion
    }
}