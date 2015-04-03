using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Reflection;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Vertical slider skin
    /// </summary>

    [HostComponent(typeof(VSlider))]

    public class VSliderSkin2 : Skin
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
        public VSliderSkin2()
        {
            MinWidth = 30;
            MinHeight = 60;
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
                                SkinClass = typeof(VSliderTrackSkin), // typeof(VScrollbarTrackSkin),
                                FocusEnabled = false,
                                //ResizeWithStyleBackground = true,
                                //MinWidth = 30,
                                Left = 0,
                                Right = 0,
                                Top = 0,
                                Bottom = 0,
                                HorizontalCenter = 0
                            };
                AddChild(Track);
            }

            _cover = new Button
            {
                SkinClass = typeof(VSliderButtonSkin),
                FocusEnabled = false,
                MouseEnabled = false,
                //MinWidth = 30,
                Left = 0,
                Right = 0,
                Bottom = 0,
                HorizontalCenter = 0
            };
            AddChild(_cover);

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(VSliderButtonSkin),
                                //MinWidth = 30,
                                Left = 0,
                                Right = 0,
                                MinHeight = 20,
                                FocusEnabled = false,
                                //ResizeWithStyleBackground = true,
                                HorizontalCenter = 0
                            };
                Thumb.AddEventListener(FrameworkEvent.Y_CHANGED, OnThumbPositionChanged);
                AddChild(Thumb);
            }
        }

        private void OnThumbPositionChanged(Event e)
        {
            var button = (Button)e.Target;
            //Debug.Log(button.X);
            _cover.Height = Height - (button.Y + button.Height / 2); // get it under the thumb
        }
    }
}