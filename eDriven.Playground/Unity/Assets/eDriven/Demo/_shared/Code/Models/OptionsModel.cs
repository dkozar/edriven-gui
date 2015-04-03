using Assets.eDriven.Demo.Helpers;
using eDriven.Audio;
using eDriven.Core.Events;
using eDriven.Gui;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace Assets.eDriven.Demo.Models
{
    /// <summary>
    /// Going MVC here
    /// </summary>
    public class OptionsModel : EventDispatcher
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static OptionsModel _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private OptionsModel()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static OptionsModel Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating OptionsModel instance"));
#endif
                    _instance = new OptionsModel();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            var player = AudioPlayerMapper.GetDefault();
            if (null != player) {
                _volume = player.Volume;
            }

            _inspectorActive = GuiInspector.Instance.enabled;
        }

// ReSharper disable InconsistentNaming
        public const string INSPECTOR_ACTIVE_CHANGED = "inspectorActiveChanged";
        public const string DETAILS_ACTIVE_CHANGED = "detailsActiveChanged";
        public const string RUN_IN_BACKGROUND_CHANGED = "runInBackgroundChanged";
        public const string VOLUME_CHANGED = "volumeChanged";
        public const string FULL_SCREEN_CHANGED = "fullScreenChanged";
        public const string RESOLUTION_CHANGED = "resolutionChanged";
// ReSharper restore InconsistentNaming

        private bool _runInBackground;
        public bool RunInBackground
        {
            get { 
                return _runInBackground;
            }
            set
            {
                if (value == _runInBackground)
                    return;
                
                _runInBackground = value;

                Application.runInBackground = _runInBackground; // setting the value

                DispatchEvent(new Event(RUN_IN_BACKGROUND_CHANGED));
            }
        }

        private float _volume;
        public float Volume
        {
            get { return _volume; }
            set
            {
                if (value == _volume)
                    return;

                _volume = value;

                AudioPlayerMapper.GetDefault().Volume = _volume; // setting the value

                DispatchEvent(new Event(VOLUME_CHANGED));
            }
        }

        private bool _inspectorActive;

        public bool InspectorActive
        {
            get { return _inspectorActive; }
            set
            {
                if (value == _inspectorActive)
                    return;

                _inspectorActive = value;

                GuiInspector.Instance.enabled = _inspectorActive; // setting the value

                DispatchEvent(new Event(INSPECTOR_ACTIVE_CHANGED));
            }
        }

        private bool _detailsActive;

        /*private InspectorDetailsWindow _popup;
        public bool DetailsActive
        {
            get { return _detailsActive; }
            set
            {
                if (value == _detailsActive)
                    return;

                _detailsActive = value;

                if (_detailsActive)
                {
                    _popup = InspectorDetailsWindow.Instance;
                    _popup.Right = 0;
                    _popup.Bottom = 0;
                    
                    PopupManager.Instance.AddPopup(_popup,
                        new PopupOption(PopupOptionType.Modal, false), 
                        new PopupOption(PopupOptionType.Centered, false), 
                        new PopupOption(PopupOptionType.Stage, InspectorOverlayStage.Instance));

                    _popup.AddEventListener(FrameworkEvent.REMOVE, OnPopupClose);
                }
                else
                {
                    //Debug.Log("Removing");
                    if (PopupManager.Instance.HasPopup(_popup))
                        PopupManager.Instance.RemovePopup(_popup);

                    _popup = null;
                }

                DispatchEvent(new Event(DETAILS_ACTIVE_CHANGED));
            }
        }*/

        private bool _fullScreen;
        public bool FullScreen
        {
            get
            {
                return _fullScreen;
            }
            set
            {
                if (value == _fullScreen)
                    return;

                _fullScreen = value;
                DispatchEvent(new Event(FULL_SCREEN_CHANGED));
            }
        }

        private ResolutionDescriptor _resolution;
        public ResolutionDescriptor Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;
                DispatchEvent(new Event(RESOLUTION_CHANGED));
            }
        }

        /*private void OnPopupClose(Event e)
        {
            _popup.RemoveEventListener(FrameworkEvent.REMOVE, OnPopupClose);
            DetailsActive = false;
        }*/
    }
}