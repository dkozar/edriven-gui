using eDriven.Gui.Reflection;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Horizontal scrollbar skin
    /// </summary>

    [HostComponent(typeof(VSlider))]

    public class VSliderSkin : Skin
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
        public VSliderSkin()
        {
            MinWidth = 32;
            MinHeight = 32;
            //StopMouseWheelPropagation = true;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            // Create the scroll track.
            if (null == Track)
            {
                Track = new Button
                            {
                                Id = "track",
                                SkinClass = typeof(VSliderTrackSkin),
                                Left = 0,
                                Right = 0,
                                Top = 0,
                                Bottom = 0,
                                Height = 100
                            };

                AddChild(Track);
            }

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(VSliderThumbSkin),
                                Left = 0,
                                Right = 0,
                                Width = 11,
                                Height = 11
                            };

                AddChild(Thumb);
            }
        }
    }
}