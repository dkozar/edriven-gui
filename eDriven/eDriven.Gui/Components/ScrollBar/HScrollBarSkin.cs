using eDriven.Gui.Reflection;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Horizontal scrollbar skin
    /// </summary>
    
    [HostComponent(typeof(HScrollBar))]

    public class HScrollBarSkin : Skin
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
        public HScrollBarSkin()
        {
            MinWidth = 32;
            MinHeight = 30;
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
                                SkinClass = typeof(HScrollBarTrackSkin),
                                FocusEnabled = false,
                                //ResizeWithStyleBackground = true,
                                Left = 29,
                                Right = 29,
                                VerticalCenter = 0
                            };

                AddChild(Track);
            }

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(HScrollBarThumbSkin),
                                //Width = 100,
                                FocusEnabled = false,
                                //ResizeWithStyleBackground = true,
                                VerticalCenter = 0
                            };

                AddChild(Thumb);
            }

            // Create the down-arrow button, layered above the track.
            if (null == DecrementButton)
            {
                DecrementButton = new Button
                                      {
                                          Id = "decrement",
                                          SkinClass = typeof(HScrollBarLeftButtonSkin),
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          VerticalCenter = 0,
                                          Left = 0
                                      };

                AddChild(DecrementButton);
            }

            // Create the up-arrow button, layered above the track.
            if (null == IncrementButton)
            {
                IncrementButton = new Button
                                      {
                                          Id = "increment",
                                          SkinClass = typeof(HScrollBarRightButtonSkin),
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          VerticalCenter = 0,
                                          Right = 0
                                      };

                AddChild(IncrementButton);
            }
        }

        ///// <summary>
        ///// TEMP!
        ///// </summary>
        //protected override void CreationComplete()
        //{
        //    base.CreationComplete();

        //    Track.Left = DecrementButton.Width; // +1;
        //    Track.Right = IncrementButton.Width; // + 1;
        //}
    }
}