using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using HScrollBarTrackSkin = Assets.eDriven.Skins.HScrollBarTrackSkin;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Horizontal scrollbar skin
    /// </summary>
     
    [HostComponent(typeof(HScrollBar))]

    [Style(Name = "upImage", Type = typeof(Texture))]
    [Style(Name = "downImage", Type = typeof(Texture))]

    public class HScrollBarSkin3 : Skin
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
        public HScrollBarSkin3()
        {
            MinWidth = 80;
            MinHeight = 50;
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
                                MinHeight = 50,
                                Left = 49,
                                Right = 49,
                                VerticalCenter = 0
                            };
                AddChild(Track);
            }

            if (null == Thumb)
            {
                Thumb = new Button
                            {
                                Id = "thumb",
                                SkinClass = typeof(HScrollBarButtonSkin),
                                MinHeight = 50,
                                MinWidth = 10,
                                FocusEnabled = false,
                                VerticalCenter = 0
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
                                          SkinClass = typeof(HScrollBarButtonSkin), // typeof(VScrollBarUpButtonSkin),
                                          MinWidth = 50,
                                          MinHeight = 50,
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          VerticalCenter = 0,
                                          Left = 0,
                                          Icon = (Texture2D)Resources.Load("eDriven/Editor/Icons/arrow-180")
                                      };
                AddChild(DecrementButton);
            }

            // Create the up-arrow button, layered above the track.
            if (null == IncrementButton)
            {
                IncrementButton = new Button
                                      {
                                          Id = "increment",
                                          SkinClass = typeof(HScrollBarButtonSkin),
                                          MinWidth = 50,
                                          MinHeight = 50,
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          VerticalCenter = 0,
                                          Right = 0,
                                          Icon = (Texture2D)Resources.Load("eDriven/Editor/Icons/arrow")
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