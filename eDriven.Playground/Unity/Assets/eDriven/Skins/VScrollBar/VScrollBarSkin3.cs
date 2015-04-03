using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Vertical scrollbar skin
    /// </summary>
     
    [HostComponent(typeof(VScrollBar))]

    [Style(Name = "upImage", Type = typeof(Texture))]
    [Style(Name = "downImage", Type = typeof(Texture))]

    public class VScrollBarSkin3 : Skin
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
        public VScrollBarSkin3()
        {
            MinWidth = 50;
            MinHeight = 80;
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
                                SkinClass = typeof(VScrollBarTrackSkin), // typeof(VScrollBarTrackSkin),
                                FocusEnabled = false,
                                MinWidth = 50,
                                Top = 49,
                                Bottom = 49,
                                HorizontalCenter = 0
                            };
                AddChild(Track);
            }

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(VScrollBarButtonSkin),
                                MinWidth = 50,
                                MinHeight = 10,
                                FocusEnabled = false,
                                HorizontalCenter = 0
                            };
                AddChild(Thumb);
            }

            // Create the down-arrow button, layered above the track.
            if (null == DecrementButton)
            {
                //Debug.Log("Image: " + Resources.Load("eDriven/Editor/Icons/no"));
                DecrementButton = new Button
                                      {
                                          Id = "decrement",
                                          SkinClass = typeof(VScrollBarButtonSkin), // typeof(VScrollBarUpButtonSkin),
                                          MinWidth = 50,
                                          MinHeight = 50,
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          HorizontalCenter = 0,
                                          Top = 0,
                                          Icon = (Texture2D)Resources.Load("eDriven/Editor/Icons/arrow-090")
                                      };
                AddChild(DecrementButton);
            }

            // Create the up-arrow button, layered above the track.
            if (null == IncrementButton)
            {
                IncrementButton = new Button
                                      {
                                          Id = "increment",
                                          SkinClass = typeof(VScrollBarButtonSkin),
                                          MinWidth = 50,
                                          MinHeight = 50,
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          HorizontalCenter = 0,
                                          Bottom = 0,
                                          Icon = (Texture2D)Resources.Load("eDriven/Editor/Icons/arrow-270")
                                      };
                AddChild(IncrementButton);
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            if (null != DecrementButton)
            {
                Texture icon = GetStyle("upImage") as Texture;
                if (null != icon)
                    DecrementButton.Icon = icon;
            }
            if (null != IncrementButton)
            {
                Texture icon = GetStyle("downImage") as Texture;
                if (null != icon)
                    IncrementButton.Icon = icon;
            }
        }
    }
}