using Assets.eDriven.Skins;
using eDriven.Gui.Components;
using eDriven.Gui.Shapes;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace Assets.eDriven.Demo.Gui.Code.CompositeControlDemo
{
    /// <summary>
    /// Composite component example
    /// Note that this component is not skinned. Skinning will be described in another example.
    /// </summary>
    public class RgbMixer : VGroup
    {
        private VSlider _red;
        private VSlider _green;
        private VSlider _blue;
        private RectShape _display;

        public RgbMixer()
        {
            MinHeight = 400;
            Gap = 10;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _display = new RectShape { PercentWidth = 100, Height = 150 };
            AddChild(_display);

            HGroup hGroup = new HGroup { Gap = 10, PercentHeight = 100 };
            AddChild(hGroup);

            _red = new VSlider
            {
                SkinClass = typeof(VSliderSkin2),
                Width = 50,
                PercentHeight = 100, Maximum = 255/*, BoolExample = true*/
            };
            _red.Change += ChangeHandler;
            hGroup.AddChild(_red);

            _green = new VSlider
            {
                SkinClass = typeof(VSliderSkin2),
                PercentHeight = 100,
                Width = 50,
                Maximum = 255
            };
            _green.Change += ChangeHandler;
            hGroup.AddChild(_green);

            _blue = new VSlider
            {
                SkinClass = typeof(VSliderSkin2),
                Width = 50,
                PercentHeight = 100,
                Maximum = 255
            };
            _blue.Change += ChangeHandler;
            hGroup.AddChild(_blue);
        }

        private void ChangeHandler(Event e)
        {
            //ValueChangedEvent ve = (ValueChangedEvent)e;
            //Debug.Log(ve.NewValue);
            //VSlider s = (VSlider)e.Target; // we might care about the slider touched
            RgbColor = new Color(_red.Percentage, _green.Percentage, _blue.Percentage);
        }

        private bool _rgbColorChanged;
        private Color _rgbColor;
        /// <summary>
        /// A collor getter/setter
        /// </summary>
        public Color RgbColor
        {
            get
            {
                return _rgbColor;
            }
            set
            {
                _rgbColor = value;
                _rgbColorChanged = true;
                InvalidateProperties();
            }
        }

        /// <summary>
        /// Delay setting on children because they might not be created yet
        /// </summary>
        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_rgbColorChanged)
            {
                //Debug.Log("_rgbColorChanged: " + _rgbColor);
                _rgbColorChanged = false;
                _red.Value = _rgbColor.r * 255;
                _green.Value = _rgbColor.g * 255;
                _blue.Value = _rgbColor.b * 255;

                /*_red.Color = new Color(MakeSliderColor(_rgbColor.r), 0, 0);
                _green.Color = new Color(0, MakeSliderColor(_rgbColor.g), 0);
                _blue.Color = new Color(0, 0, MakeSliderColor(_rgbColor.b));*/

                _display.BackgroundColor = _rgbColor;
            }
        }
    }
}