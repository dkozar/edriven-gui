using System.Collections;
using Assets.eDriven.Demo._shared.Code.Tweens;
using Assets.eDriven.Skins;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Cursor;
using eDriven.Gui.Events;
using eDriven.Gui.Managers;
using eDriven.Gui.Util;
using eDriven.Networking.Rpc;
using eDriven.Networking.Rpc.Loaders;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;

namespace Assets.eDriven.Demo.Gui.Code.LoadData
{
    public class ImageViewer : Dialog
    {
        private static readonly TextureLoader ImageLoader = new TextureLoader
        {
            Cache = false,
        }; // loads textures

        private static bool _imageLoaderSet;
        
        private static readonly Hashtable ButtonStyles = new Hashtable { { "cursor", "pointer" } };

        private Image _image;

        private bool _imageUrlChanged;
        private string _imageUrl;
        public string ImageUrl
        {
            get { 
                return _imageUrl;
            }
            set
            {
                if (value == _imageUrl)
                    return;

                _imageUrl = value;
                _imageUrlChanged = true;
                InvalidateProperties();
            }
        }

        public ImageViewer()
        {
            Id = "viewer";

            //SetStyle("contentBackgroundColor", Color.green); // TODO: propagate style to skin

            if (!_imageLoaderSet)
            {
                _imageLoaderSet = true;
                ImageLoader.Connector.MaxConcurrentRequests = 1;
                ImageLoader.Connector.ConcurencyMode = ConcurencyMode.Multiple;
                ImageLoader.Connector.ProcessingMode = ProcessingMode.Sync;
            }

            // minimum size (for window resizing)
            MinWidth = 200;
            MinHeight = 150;
            
            // default size
            Width = 400;
            Height = 300;

            // close the top window on Esc key
            CloseOnEsc = true;
        }

        /* TEMP override - TODO: StyleProxy! */
        protected override void CreationComplete()
        {
            base.CreationComplete();
            Skin.SetStyle("contentBackgroundColor", Color.grey); //RgbColor.FromHex(0x004CFF).ToColor());
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            #region Tools

            Button button = new Button
            {
                SkinClass = typeof (ImageButtonSkin),
                Icon = global::eDriven.Core.Caching.ImageLoader.Instance.Load("Icons/cancel"),
                FocusEnabled = false,
                Styles = ButtonStyles
            };
            button.Click += delegate
            {
                DispatchEvent(new CloseEvent(CloseEvent.CLOSE));
            };
            ToolGroup.AddChild(button);

            #endregion

            #region Image

            _image = new Image { Id="image", PercentWidth = 100, PercentHeight = 100, ScaleMode = ImageScaleMode.ScaleToFit };
            AddContentChild(_image);

            #endregion

            #region Control bar

            ControlBarGroup.AddChild(new Spacer {PercentWidth = 100});

            button = new Button
            {
                Text = "Close preview",
                SkinClass = typeof (ImageButtonSkin),
                Icon = global::eDriven.Core.Caching.ImageLoader.Instance.Load("Icons/color_swatch"),
                FocusEnabled = false,
                Styles = ButtonStyles
            };
            button.Click += delegate
            {
                DispatchEvent(new CloseEvent(CloseEvent.CLOSE));
            };
            ControlBarGroup.AddChild(button);

            #endregion

        }

        private LoadingMask _mask;

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_imageUrlChanged)
            {
                _imageUrlChanged = false;

                _mask = new LoadingMask(ContentGroup, "Loading image...");
                var cursor = CursorManager.Instance.SetCursor(CursorType.Wait);

                var oldBounds = Rectangle.FromWidthAndHeight(GetExplicitOrMeasuredWidth(), GetExplicitOrMeasuredHeight());
                oldBounds = oldBounds.CenterInside(Rectangle.FromSize(SystemManager.Instance.ScreenSize));
                
                ImageLoader.Load(_imageUrl, delegate(Texture texture)
                {
                    //Debug.Log("texture size: " + texture.width + ", " + texture.height);
                    _image.Texture = texture;
                    _image.Visible = false;
                    
                    // reset explicit size to allow measurement
                    ExplicitWidth = null;
                    ExplicitHeight = null;

                    ValidateSize(true);
                    
                    var newBounds = Rectangle.FromWidthAndHeight(GetExplicitOrMeasuredWidth(), GetExplicitOrMeasuredHeight());
                    // center on screen
                    newBounds = newBounds.CenterInside(Rectangle.FromSize(SystemManager.Instance.ScreenSize));
                    //Debug.Log("newBounds: " + newBounds);

                    // return to the old size immediatelly, because of the flicker
                    Width = oldBounds.Width;
                    Height = oldBounds.Height;

                    #region Alternative

                    //SetActualSize(GetExplicitOrMeasuredWidth(), GetExplicitOrMeasuredHeight());
                    //PopupManager.Instance.CenterPopUp(this);

                    #endregion

                    Defer(delegate
                    {
                        // tween the window size
                        //var newBounds = Bounds.CenterInside(Rectangle.FromSize(SystemManager.Instance.ScreenSize));
                        ScaleBoundsTween tween = new ScaleBoundsTween(this, oldBounds, newBounds)
                        {
                            Callback = delegate
                            {
                                _image.Visible = true; // show image
                                //_image.SkipRender(2); // avoid flicker
                                _image.Alpha = 0;

                                // fade in the image
                                FadeIn fadeIn = new FadeIn
                                {
                                    Duration = 1
                                };
                                fadeIn.Play(_image);
                            }
                        };
                        tween.Play();
                    }, 1);

                    //GlobalLoadingMask.Hide();
                    _mask.Unmask();
                    CursorManager.Instance.RemoveCursor(cursor);
                });
            }
        }
    }
}