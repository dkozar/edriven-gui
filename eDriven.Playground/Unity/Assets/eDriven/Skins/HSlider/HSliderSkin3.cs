using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using HSliderTrackSkin = Assets.eDriven.Skins.HSliderTrackSkin;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Horizontal slider skin
    /// </summary>
     
    [HostComponent(typeof(HSlider))]

    public class HSliderSkin3 : Skin
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public new static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        /**
         * Skin parts
         * */

#pragma warning disable 1591
// ReSharper disable MemberCanBePrivate.Global

        public Button Track;

        public Button Thumb;

// ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 1591

        /*public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                // Turn off buttons, or turn on buttons and resync thumb.
                InvalidateDisplayList();
            }
        }*/

        /// <summary>
        /// Contructor
        /// </summary>
        public HSliderSkin3()
        {
            MinWidth = 80;
            MinHeight = 50;
            //StopMouseWheelPropagation = true;
        }

        private Button _cover;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            // Create the scroll track.
            if (null == Track)
            {
                Track = new Button
                            {
                                Id = "track",
                                SkinClass = typeof(HSliderTrackSkin),
                                FocusEnabled = false,
                                //MinHeight = 50,
                                Left = 0,
                                Right = 0,
                                Top = 0,
                                Bottom = 0,
                                VerticalCenter = 0
                            };
                AddChild(Track);
            }

            _cover = new Button
                         {
                             SkinClass = typeof (HSliderButtonSkin),
                             FocusEnabled = false,
                             MouseEnabled = false,
                             //MinHeight = 50,
                             Left = 0,
                             Top = 0,
                             Bottom = 0,
                             VerticalCenter = 0
                         };
            AddChild(_cover);

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(HSliderButtonSkin),
                                //MinHeight = 50,
                                Top = 0,
                                Bottom = 0,
                                MinWidth = 30,
                                FocusEnabled = false,
                                VerticalCenter = 0
                            };
                Thumb.AddEventListener(FrameworkEvent.X_CHANGED, OnThumbPositionChanged);
                AddChild(Thumb);
            }
        }

        private void OnThumbPositionChanged(Event e)
        {
            var button = (Button) e.Target;
            //Debug.Log(button.X);
            _cover.Width = button.X + button.Width / 2; // get it under the thumb
        }
    }
}