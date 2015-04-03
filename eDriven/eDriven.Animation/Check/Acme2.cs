using Random = System.Random;
#if RELEASE
using System.Reflection;
using UnityEngine;

namespace eDriven.Animation.Check
{
    public class Acme2 : MonoBehaviour
    {
        private Color _oldColor;
        private Rect _labelBounds = new Rect(10, 10, 320, 40);
        private int _placement = 6;
        private int _textIndex;
    
        private readonly Random _random = new Random();
    
        private readonly string[] _texts =
        {
            @"Created using the free version of eDriven.Gui
edrivengui.com", 
            @"Created using the free version of eDriven.Gui
Copyright © Danko Kozar 2010-2014", 
            @"Created using the free version of eDriven.Gui
NOT FOR COMMERCIAL USE",
            @"Created using the free version of eDriven.Gui
Please purchase a full version",
            @"Please purchase a full version of eDriven.Gui
and support further development"
        };

        private float _time;
        private const float Interval = 240;
        private const float FadeInInterval = 2;
        private const float FadeOutInterval = 2;

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void OnEnable()
// ReSharper restore UnusedMember.Local
        {
            _textIndex = DifferentRandom(_textIndex, _texts.Length);
        }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void Update()
// ReSharper restore UnusedMember.Local
        {
            _time += Time.deltaTime;
            if (_time > Interval)
            {
                Dice();
            }
        }

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            DrawLabel(_placement);
        }

        private Rect _bounds;
        private void DrawLabel(int mode)
        {
            switch (mode)
            {
                default: //TopLeft: (case 0:)
                    _bounds = _labelBounds;
                    break;
                case 1: //Top:
                    _bounds = new Rect((Screen.width - _labelBounds.width) / 2, _labelBounds.y, _labelBounds.width, _labelBounds.height);
                    break;
                case 2: //TopRight:
                    _bounds = new Rect((Screen.width - _labelBounds.width - _labelBounds.x), _labelBounds.y, _labelBounds.width, _labelBounds.height);
                    break;
                case 3: //Right:
                    _bounds = new Rect((Screen.width - _labelBounds.width - _labelBounds.x), (Screen.height - _labelBounds.height) / 2, _labelBounds.width, _labelBounds.height);
                    break;
                case 4: //BottomRight:
                    _bounds = new Rect((Screen.width - _labelBounds.width - _labelBounds.x), (Screen.height - _labelBounds.height - _labelBounds.y), _labelBounds.width, _labelBounds.height);
                    break;
                case 5: //Bottom:
                    _bounds = new Rect((Screen.width - _labelBounds.width) / 2, (Screen.height - _labelBounds.height - _labelBounds.y), _labelBounds.width, _labelBounds.height);
                    break;
                case 6: //BottomLeft:
                    _bounds = new Rect(_labelBounds.x, (Screen.height - _labelBounds.height - _labelBounds.y), _labelBounds.width, _labelBounds.height);
                    break;
                case 7: //Left:
                    _bounds = new Rect(_labelBounds.x, (Screen.height - _labelBounds.height)/2, _labelBounds.width, _labelBounds.height);
                    break;
            }

            _oldColor = GUI.color;

            float alpha = 1;
            if (_time < FadeInInterval)
                alpha = _time/FadeInInterval;
            else if (_time > Interval - FadeOutInterval)
                alpha = (Interval - _time) / FadeOutInterval;

            GUI.color = new Color(1, 1, 1, alpha);

            GUI.Label(_bounds, _texts[_textIndex]);
            GUI.color = _oldColor;
        }

        #region Helper

        private void Dice()
        {
            _placement = DifferentRandom(_placement, 8);
            //Debug.Log("_placement: " + _placement);
            _time = 0;

            _textIndex = DifferentRandom(_textIndex, _texts.Length);
            //Debug.Log("_textIndex: " + _textIndex);
        }

        /// <summary>
        /// Recursive!!!
        /// </summary>
        /// <returns></returns>
        private int DifferentRandom(int current, int length)
        {
            int index = _random.Next(0, length);
            if (current == index)
            {
                return DifferentRandom(current, length);
            }
            return index;
        }

        #endregion

    }
}

#endif