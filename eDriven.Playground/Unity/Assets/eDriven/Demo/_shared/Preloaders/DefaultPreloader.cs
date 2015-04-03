using System;
using System.Text;
using eDriven.Core.Geom;
using eDriven.Gui.Reflection;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace eDriven.Demo.Preloaders
{
    /// <summary>
    /// This component is an example of eDriven.Gui preloader
    /// Since eDriven.Gui is not initialized at this point, we cannot use eDriven.Gui for rendering preloader
    /// We have to render it by other means
    /// </summary>
    public class DefaultPreloader : MonoBehaviour
    {
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode/* = true*/;

// ReSharper restore UnassignedField.Global

        public string Message = "Initializing...";

        public float Width = 240;
        public float Height = 80;

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            PreloaderFeedback.Instance.StepStartSignal.Connect(StepStartSlot);
            PreloaderFeedback.Instance.InitializationCompleteSignal.Connect(InitializationCompleteSlot);
        }

        private readonly StringBuilder _sb = new StringBuilder();
        private DateTime? _time;

        private void StepStartSlot(object[] parameters)
        {
            var message = (string)parameters[0];

            switch (message)
            {
                case PreloaderFeedback.LOADING_DEFAULT_STYLES:
                    Message = "Loading default styles...";
                    break;
                case PreloaderFeedback.LOADING_STYLESHEETS:
                    Message = "Loading stylesheets...";
                    break;
                case PreloaderFeedback.INITIALIZING_STYLES:
                    Message = "Initializing...";
                    break;
                default:
                    Message = message;
                    break;
            }
            
            if (DebugMode)
            {
                AppendDuration();
                _sb.Append(message);
                _time = DateTime.Now;
            }
        }

        private void InitializationCompleteSlot(object[] parameters)
        {

            if (DebugMode)
            {
                AppendDuration();
                _sb.AppendLine("Finished");
                Debug.Log(_sb.ToString());
            }
            enabled = false;
            //Destroy(this);
        }

        /// <summary>
        /// Writing to log isn't mandatory, you could easily remove it
        /// </summary>
        private void AppendDuration()
        {
            if (null != _time)
                _sb.AppendLine(string.Format(" ({0} ms)", Math.Round(DateTime.Now.Subtract((DateTime)_time).TotalMilliseconds)));
        }

        private GUIStyle _style;

// ReSharper disable once UnusedMember.Local
        void OnGUI()
        {
            if (string.IsNullOrEmpty(Message))
                return;

            if (null == _style)
            {
                _style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
            }

            var rectangle = Rectangle.CenterRectangle(
                Rectangle.FromWidthAndHeight(Screen.width, Screen.height),
                Rectangle.FromWidthAndHeight(Width, Height)
            );
            if (Message != null)
                GUI.Box(rectangle.ToRect(), Message, _style);
        }
    }
}