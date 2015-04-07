using Random = System.Random;
#if TRIAL
using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Core.Managers;
using eDriven.Core.Serialization;
using eDriven.Core.Util;
using UnityEngine;

namespace eDriven.Gui.Check
{
    public class Acme : MonoBehaviour
    {
        private Color _oldColor;
        private Rect _labelBounds = new Rect(10, 10, 300, 50);
        private Rect _labelMinBounds = new Rect(10, 10, 150, 40);
        private Rect _logoBounds = new Rect(0, 0, 400, 300);
        private int _placement;
        private int _logoPlacement = 5;
        private int _textIndex;
        private const float Interval = 20;
        private const float FadeInInterval = 2;
        private const float FadeOutInterval = 2;
        private const float LogoFadeInInterval = 2;
        private const float LogoFadeOutInterval = 2;
        private static float _logoInterval = 1000;
        private float _time = -10; // 10 seconds for getting the internet connection
        private Rect _bounds;
        private Texture2D _texture;
        private float _logoAlpha = 1;
        private bool _logoInEditor = true;
        private bool _logoInBuild = true;
        private XmlLoadingState _xmlState = XmlLoadingState.Loading;
        private Color _logoColor = Color.white;
        private readonly Random _random = new Random();
        private float _alpha;
    
        private WWW _request;
    
        private string[] _texts =
        {
            @"Created using the free version of eDriven.Gui
www.edrivengui.com", 
            @"Created using the free version of eDriven.Gui
Copyright © Danko Kozar 2010-2014. All rights reserved.", 
            @"Created using the free version of eDriven.Gui
NOT FOR COMMERCIAL USE!",
            @"Created using the free version of eDriven.Gui
Please purchase a full version!",
            @"Please purchase a full version of eDriven.Gui
and support further development!"
        };

        private float _logoLoadingTime;
    
        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void Start()
// ReSharper restore UnusedMember.Local
        {
            string url = string.Format("http://edriven.dankokozar.com/gui/trial/config?v={0}&{1}", Info.AssemblyVersion, (DateTime.Now - new DateTime(1970, 1, 1)).Ticks);
            //Debug.Log(url);
            _request = new WWW(url);

            MeasureLabel();
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
            ProcessData();
        }

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            /**
         * Draw messages if:
         * 1. We are in build (always drawing it)
         * 2. Settings not retrieved from the server
         * 3. We are in editor and settings say we should draw it in editor
         * */
            if (!Application.isEditor || 
                null == _settings ||
                Application.isEditor && _settings.ShowInEditor)
            {
                DrawLabel();
            }
        
            DrawLogo();
        }

        private void DrawLabel()
        {
            _bounds = CalculateBounds(_placement, ref _labelBounds);

            _oldColor = GUI.color;

            float alpha = 1;
            if (_time < FadeInInterval)
                alpha = _time/FadeInInterval;
            else if (_time > Interval)
                alpha = 0;
            else if (_time > Interval - FadeOutInterval)
                alpha = (Interval - _time) / FadeOutInterval;
        
            GUI.color = new Color(1, 1, 1, alpha);
        
            if (GUI.Button(_bounds, _texts[_textIndex], LockButtonStyle.Instance))
            {
                Dice();
            }
            GUI.color = _oldColor;
        }

        private void DrawLogo()
        {
            if (null == _texture)
                return;

            //_bounds = new Rect((Screen.width - _logoBounds.width) / 2, (Screen.height - _logoBounds.height - _logoBounds.y), _logoBounds.width, _logoBounds.height);
            _bounds = CalculateBounds(_logoPlacement, ref _logoBounds);

            var delta = Time.time - _logoLoadingTime;
            float alpha = 1;
            if (delta < LogoFadeInInterval)
                alpha = delta / LogoFadeInInterval;
            else if (delta > _logoInterval)
                alpha = 0;
            else if (delta > _logoInterval - LogoFadeOutInterval)
                alpha = (_logoInterval - delta) / LogoFadeOutInterval;
        
            _alpha = _logoAlpha*alpha;

            if (_alpha == 0)
                return;

            if (_alpha < 1)
            {
                _oldColor = GUI.color;
                GUI.color = new Color(1, 1, 1, _alpha);
            }
            GUI.DrawTexture(_bounds, _texture);
            if (_alpha < 1)
            {
                GUI.color = _oldColor;
            }
        }

        private static Rect CalculateBounds(int mode, ref Rect itemBounds)
        {
            Rect bounds;
            switch (mode)
            {
                default: //TopLeft: (case 0:)
                    bounds = itemBounds;
                    break;
                case 1: //Top:
                    bounds = new Rect((Screen.width - itemBounds.width) / 2, itemBounds.y, itemBounds.width, itemBounds.height);
                    break;
                case 2: //TopRight:
                    bounds = new Rect((Screen.width - itemBounds.width - itemBounds.x), itemBounds.y, itemBounds.width, itemBounds.height);
                    break;
                case 3: //Right:
                    bounds = new Rect((Screen.width - itemBounds.width - itemBounds.x), (Screen.height - itemBounds.height) / 2, itemBounds.width, itemBounds.height);
                    break;
                case 4: //BottomRight:
                    bounds = new Rect((Screen.width - itemBounds.width - itemBounds.x), (Screen.height - itemBounds.height - itemBounds.y), itemBounds.width, itemBounds.height);
                    break;
                case 5: //Bottom:
                    bounds = new Rect((Screen.width - itemBounds.width) / 2, (Screen.height - itemBounds.height - itemBounds.y), itemBounds.width, itemBounds.height);
                    break;
                case 6: //BottomLeft:
                    bounds = new Rect(itemBounds.x, (Screen.height - itemBounds.height - itemBounds.y), itemBounds.width, itemBounds.height);
                    break;
                case 7: //Left:
                    bounds = new Rect(itemBounds.x, (Screen.height - itemBounds.height)/2, itemBounds.width, itemBounds.height);
                    break;
                case 8: //Center:
                    bounds = new Rect((Screen.width - itemBounds.width) / 2, (Screen.height - itemBounds.height) / 2, itemBounds.width, itemBounds.height);
                    break;
            }
            return bounds;
        }

        private void ProcessData()
        {
            switch (_xmlState)
            {
                case XmlLoadingState.Loading:
                    if (_request.isDone)
                    {
                        if (null != _request.error)
                        {
                            //Debug.Log("Error loading data: " + _request.error);
                            _xmlState = XmlLoadingState.Error;
                        }
                        else
                        {
                            string xml = _request.text;
                            //Debug.Log(xml);
                            //_xmlState = XmlLoadingState.Error;
                            ProcessXml(xml);
                            //_texture = _request.texture;
                            //_logoBounds = new Rect(0, 0, _texture.width, _texture.height);
                        }
                    }
                    break;
                case XmlLoadingState.LoadingLogo:
                    if (_request.isDone)
                    {
                        if (null != _request.error)
                        {
                            //Debug.Log("Error loading logo: " + _request.error);
                            _xmlState = XmlLoadingState.Error;
                        }
                        else
                        {
                            _texture = _request.texture;
                            //_texture = _request.texture;
                            //_logoBounds = new Rect(0, 0, _texture.width, _texture.height);
                            _xmlState = XmlLoadingState.Finished;
                            _logoLoadingTime = Time.time;
                            //Alert.Show("Finished", "Finished!");
                        }
                    }
                    break;
                    //case XmlLoadingState.Finished:
                    //case XmlLoadingState.Error:
                default:
                    // do nothing
                    break;
            }
        }

        private void OnTimerComplete(eDriven.Core.Events.Event e)
        {
            _timer.Stop();
            SystemManager.Instance.Enabled = false;
        }

        private Timer _timer;
        
        private Settings _settings;

        private void ProcessXml(string xml)
        {
            try
            {
                Configuration configuration = XmlSerializer<Configuration>.Deserialize(xml);

                if (null == configuration)
                {
                    _xmlState = XmlLoadingState.Error;
                    return;
                }

                _settings = configuration.Settings;

                if (configuration.Blocked)
                {
                    //Alert.Show("Blocked", "The application is being blocked");
                    _timer = new Timer(10, 1);
                    _timer.Complete += OnTimerComplete;
                    _timer.Start();
                }

                if (null != configuration.InfoMessage)
                {
                    var info = configuration.InfoMessage;

                    bool shouldShow = info.Mode == MessageMode.Both;
                    if (Application.isEditor)
                    {
                        shouldShow = shouldShow || info.Mode == MessageMode.Editor;
                    }
                    else
                    {
                        shouldShow = shouldShow || info.Mode == MessageMode.Build;
                    }

                    if (shouldShow)
                    {
                        Alert.Show(
                            delegate(string action)
                            {
                                switch (action)
                                {
                                    case "dismiss":
                                        // do nothing
                                        break;
                                    case "more":
                                        Application.OpenURL(info.Url);
                                        break;
                                    default:
                                        break;
                                }

                            },
                            new AlertOption(AlertOptionType.Title, info.Title),
                            new AlertOption(AlertOptionType.Message, info.Message),
                            new AlertOption(AlertOptionType.Button, new AlertButtonDescriptor("dismiss", "Dismiss", true)),
                            new AlertOption(AlertOptionType.Button, new AlertButtonDescriptor("more", "More..."))
                            );
                    }
                }

                if (null != configuration.Messages)
                {
                    if (configuration.Messages.Count != 0)
                    {
                        List<string> msgs = new List<string>(_texts);

                        if (!configuration.AppendMessages)
                        {
                            msgs = new List<string>();
                            //if (configuration.Messages.Count == 1)
                            //    msgs.Add(_texts[0]);
                        }

                        foreach (InfoMessage message in configuration.Messages)
                        {
                            //Debug.Log("* " + message.Message);
                            msgs.Add(message.Message);
                        }
                        _texts = msgs.ToArray();
                    }
                }

                /**
             * 2. Load logo
             * */

                var logo = configuration.LogoInfo;
                if (null == logo)
                {
                    _xmlState = XmlLoadingState.Error;
                    return;
                }

                _logoInEditor = logo.ShowInEditor;
                _logoInBuild = logo.ShowInBuild;

                if (Application.isEditor)
                {
                    if (!_logoInEditor)
                    {
                        _xmlState = XmlLoadingState.Finished;
                        return;
                    }
                }
                else
                {
                    if (!_logoInBuild)
                    {
                        _xmlState = XmlLoadingState.Finished;
                        return;
                    }
                }

                string logoUrl = logo.Url;

                if (string.IsNullOrEmpty(logoUrl))
                    return;

                _logoAlpha = Mathf.Clamp(logo.Alpha, 0, 1);
                _logoColor = new Color(1, 1, 1, _logoAlpha);
                _logoPlacement = logo.Placement;
                _logoInterval = Mathf.Max(logo.Duration, LogoFadeInInterval + LogoFadeOutInterval);

                if (logo.CacheBuster)
                    logoUrl += string.Format("?{0}", (DateTime.Now - new DateTime(1970, 1, 1)).Ticks);

                _request = new WWW(logoUrl);
                _xmlState = XmlLoadingState.LoadingLogo;
            }
            catch (Exception)
            {
                Debug.Log("Error loading data");
            }
        }

        #region Helper

        private void Dice()
        {
            _placement = DifferentRandom(_placement, 8);
            //Debug.Log("_placement: " + _placement);
            _time = 0;

            _textIndex = DifferentRandom(_textIndex, _texts.Length);

            MeasureLabel();
            //Debug.Log("_textIndex: " + _textIndex);
        }

        private void MeasureLabel()
        {
            var size = LockButtonStyle.Instance.CalcSize(new GUIContent(_texts[_textIndex]));
            _labelBounds.width = Mathf.Max(size.x, _labelMinBounds.width);
            _labelBounds.height = Mathf.Max(size.y, _labelMinBounds.height);
        }

        /// <summary>
        /// Recursive!!!
        /// </summary>
        /// <returns></returns>
        private int DifferentRandom(int current, int length)
        {
            int index = _random.Next(0, length);
            if (current == index && length > 1) // avoid stack overflow
            {
                return DifferentRandom(current, length);
            }
            return index;
        }

        #endregion

        private enum XmlLoadingState
        {
            Loading, LoadingLogo, Finished, Error
        }

    }
}

#endif