using eDriven.Gui.Reflection;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Vertical scrollbar skin
    /// </summary>
     
    [HostComponent(typeof(VScrollBar))]

    public class VScrollBarSkin : Skin
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

        public Button IncrementButton;

        public Button DecrementButton;

        public Button Track;

        public Button Thumb;

// ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 1591

        public override bool Enabled
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
        }

        /// <summary>
        /// Contructor
        /// </summary>
        public VScrollBarSkin()
        {
            MinWidth = 30;
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
                                SkinClass = typeof(VScrollBarTrackSkin),
                                FocusEnabled = false,
                                Top = 29,
                                Bottom = 29,
                                HorizontalCenter = 0
                            };
                AddChild(Track);
            }

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(VScrollBarThumbSkin),
                                FocusEnabled = false,
                                HorizontalCenter = 0
                            };
                AddChild(Thumb);
            }

            // Create the down-arrow button, layered above the track.
            if (null == DecrementButton)
            {
                DecrementButton = new Button
                                      {
                                          Id = "decrement",
                                          SkinClass = typeof(VScrollBarUpButtonSkin),
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          HorizontalCenter = 0,
                                          Top = 0
                                      };
                AddChild(DecrementButton);
            }

            // Create the up-arrow button, layered above the track.
            if (null == IncrementButton)
            {
                IncrementButton = new Button
                                      {
                                          Id = "increment",
                                          SkinClass = typeof(VScrollBarDownButtonSkin),
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          HorizontalCenter = 0,
                                          Bottom = 0
                                      };
                AddChild(IncrementButton);
            }
        }
    }
}