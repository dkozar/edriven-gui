using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Panel skin
    /// </summary>

    [HostComponent(typeof(ModalOverlay))]

    [Style(Name = "backgroundTexture", Type = typeof(Texture))]
    [Style(Name = "backgroundImageMode", Type = typeof(ImageMode), Default = ImageMode.Tiled)]
    [Style(Name = "alpha", Type = typeof(float), Default = 0.5f)]
    
    public class ModalOverlaySkin2 : Skin
    {
        public ModalOverlaySkin2()
        {
            States = new List<State>(new[]
            {
                new State("normal"),
                new State("disabled")
            });
        }

        #region Members

        private Image _backgroundImage;

        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _backgroundImage = new Image
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                AlphaBlend = true
            };
            AddChild(_backgroundImage);
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _backgroundImage)
            {
                _backgroundImage.Texture = (Texture)GetStyle("backgroundTexture");
                _backgroundImage.Mode = (ImageMode)GetStyle("backgroundImageMode");
                _backgroundImage.Alpha = (float)GetStyle("alpha");
            }

            base.UpdateDisplayList(width, height);
        }
    }
}