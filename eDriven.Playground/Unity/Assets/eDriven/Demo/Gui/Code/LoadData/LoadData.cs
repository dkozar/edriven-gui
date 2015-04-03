using System;
using System.Collections.Generic;
using Assets.eDriven.Demo.Gui.Code.Flickr;
using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Demo.Gui.Code.LoadData;
using eDriven.Gui;
using eDriven.Gui.Animation;
using eDriven.Gui.Components;
using eDriven.Gui.Cursor;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Networking.Rpc;
using eDriven.Networking.Rpc.Loaders;
using Assets.eDriven.Demo.Components;
using Assets.eDriven.Demo.Tweens;
using JsonFx.Json;
using UnityEngine;
using Action = eDriven.Animation.Action;
using Event = eDriven.Core.Events.Event;
using Random = System.Random;

namespace Assets.eDriven.Demo.Gui.Code.LoadData
{
    /// <summary>
    /// </summary>
// ReSharper disable UnusedMember.Global
    public class LoadData : global::eDriven.Gui.Gui
// ReSharper restore UnusedMember.Global
    {
        #region Properties exposed to editor

        public string FlickrAppKey = "19a0493934d366a8d534c578e9385b49"; // please use this API key for this demo only!
        public string FlickrSearchUrl = "http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&text={1}&page={2}&per_page={3}&format=json&nojsoncallback=1";
        public bool CacheThumbnails;
        public string ThumbnailSize = "q";
        public string ImageSize = "z";

        #endregion

        #region Loaders

        private readonly HttpConnector _httpConnector = new HttpConnector { ResponseMode = ResponseMode.WWW }; // loads data
        private readonly TextureLoader _thumbnailLoader = new TextureLoader { Cache = false }; // loads textures
        
        #endregion
        
        #region Members

        private ArrayList _dataProvider;
        private readonly Random _random = new Random();
        private TextField _txtSearch;
        private bool _newSearch;
        private Scroller _scroller;
        private List _list; 
        
        private readonly TweenFactory _windowEffect = new TweenFactory(
            new Sequence(
                new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
                new FadeIn { Duration = 0.3f }
            )
        );

        private readonly TweenFactory _scrollerShow = new TweenFactory(
            new Sequence(
                new ZeroFadeIn()
            )
        );

        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Left,
                Gap = 0
            };

            _thumbnailLoader.Connector.MaxConcurrentRequests = 3;
            _thumbnailLoader.Connector.ConcurencyMode = ConcurencyMode.Multiple;
            _thumbnailLoader.Cache = true; // cache thumbnails
        }
        
        override protected void CreateChildren()
        {
            base.CreateChildren();

            #region Controls

            Toolbar toolbar = new Toolbar();
            toolbar.Plugins.Add(new TabManager { ArrowsEnabled = true });
            AddChild(toolbar);

            #region Text field

            _txtSearch = new TextField
            {
                StyleName = "search",
                Text = "croatian coast",
                FocusEnabled = true,
                Width = 400
            };
            _txtSearch.SetFocus();
            _txtSearch.KeyUp += delegate(Event e)
            {
                KeyboardEvent ke = (KeyboardEvent)e;
                if (ke.KeyCode == KeyCode.Return)
                {
                    _newSearch = true;
                    Search();
                }

            };
            toolbar.AddContentChild(_txtSearch);

            #endregion

            Button btnLoad = new Button
            {
                Text = "Load data",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/arrow_refresh"),
                AutoRepeat = true
            };
            btnLoad.ButtonDown += delegate
            {
                Search();
                btnLoad.Text = "Load more...";
            };
            toolbar.AddContentChild(btnLoad);

            Button btnClear = new Button
            {
                Text = "Remove all",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/cancel"),
                AutoRepeat = true
            };
            btnClear.ButtonDown += delegate
            {
                _scroller.Visible = false;
                _dataProvider.RemoveAll();
                btnLoad.Text = "Load data";
            };
            toolbar.AddContentChild(btnClear);

            #endregion

            #region Lower group

            Group group = new Group
            {
                PercentWidth = 100,
                PercentHeight = 100
            };
            AddChild(group);

            #endregion

            #region Title label

            Label label = new TitleLabel { HorizontalCenter = 0, VerticalCenter = 0, StyleName = "title" };
            group.AddChild(label);

            new TextRotator
            {
                Delay = 5, // 5 seconds delay
                Lines = new[]
            {
                "Load Data Demo",
                "Created with eDriven.Gui",
                "Loads images from Flickr"/*,
                "Author: Danko Kozar"*/
            },
                Callback = delegate(string line) { label.Text = line; }
            }
            .Start();

            #endregion
            
            #region Scroller / viewport

            _scroller = new Scroller
            {
                SkinClass = typeof (ScrollerSkin2),
                PercentWidth = 100, PercentHeight = 100,
                Visible = false
            };
            _scroller.SetStyle("showEffect", _scrollerShow);
            group.AddChild(_scroller);

            Group viewport = new Group
            {
                /*MouseEnabled = true,
                Layout = new VerticalLayout
                {
                    HorizontalAlign = HorizontalAlign.Left,
                    PaddingLeft = 10,
                    PaddingRight = 10,
                    PaddingTop = 10,
                    PaddingBottom = 10,
                    Gap = 10
                }*/
            };
            _scroller.Viewport = viewport;

            #endregion

            #region Data controls

            List<object> source = new List<object>();

            _dataProvider = new ArrayList(source);

            /* LISTS */

            #region HGroup

            #endregion

            #region List

            _list = new List
                       {
                           Left = 10, Right = 10, Top = 10, Bottom = 10,
                           Layout = new TileLayout { HorizontalGap = 0, VerticalGap = 0/*, RequestedRowCount = 4, RequestedColumnCount = 3*/ },
                           DataProvider = _dataProvider,
                           SkinClass = typeof(ListSkin2),
                           ItemRenderer = new ItemRendererFactory<ThumbnailItemRenderer>(),
                           LabelFunction = LabelFunction // extracting the title
                       };
            viewport.AddChild(_list);

            #endregion

            #endregion

            // bubbling event listener
            AddEventListener("showImage", ShowImageHandler);
        }
        
        #region Loading data

        /// <summary>
        /// Used by renderers to extract data to display
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string LabelFunction(object item)
        {
            return ((PhotoData)item).Title;
        }

        private void Search()
        {
            if (_newSearch)
            {
                _newSearch = false;
            }

            string url = string.Format(
                FlickrSearchUrl, FlickrAppKey,
                _txtSearch.Text.Replace(" ", "+"),
                _random.Next(1, 20), /*1*/ // just a random page
                20
            );

            // make a Flickr request
            GlobalLoadingMask.Show("Searching...");
            var cursor = CursorManager.Instance.SetCursor(CursorType.Wait);
            _httpConnector.Send(
                url,
                delegate(object data)
                {
                    GlobalLoadingMask.Hide();
                    CursorManager.Instance.RemoveCursor(cursor);
                    _scroller.Visible = true;
                    _list.SetFocus();
                    DisplayData(data);
                }
            );
        }

        private void DisplayData(object result)
        {
            //Debug.Log("Displaying data");
            string json = ((WWW)result).text; //Debug.Log(json);

            try // deserialize
            {
                PhotosSearchResponse response = (PhotosSearchResponse)JsonReader.Deserialize(json, typeof(PhotosSearchResponse));
                if (response.Stat != "ok")
                {
                    Alert.Show("Error", response.Message, AlertButtonFlag.Ok);
                    return;
                }

                var count = 0;

                // load images
                response.Photos.Photo.ForEach(delegate(Photo photo)
                {
                    var data = new PhotoData(photo.Title)
                    {
                        ImageUrl = photo.GetUrl(ImageSize)
                    };
                    _dataProvider.AddItemAt(data, count); // add it to the beginning off the list

                    // load thumbnails asynchronously, then update each item
                    _thumbnailLoader.Load(photo.GetUrl(ThumbnailSize), delegate(Texture texture)
                    {
                        data.Thumbnail = texture;
                        _dataProvider.ItemUpdated(data, "Thumbnail", null, texture);
                    });

                    count++;
                });
            }
            catch (Exception)
            {
                Alert.Show("Error", "No images found", AlertButtonFlag.Ok);
            }
        }

        private void ShowImageHandler(Event e)
        {
            var data = (PhotoData)((IDataRenderer)e.Target).Data;
            var window = new ImageViewer
            {
                Title = data.Title,
                ImageUrl = data.ImageUrl,
                SkinClass = typeof(WindowSkin2),
                Icon = ImageLoader.Instance.Load("Icons/star")
            };

            window.SetStyle("addedEffect", _windowEffect);
            window.Plugins.Add(new Resizable { ShowOverlay = false });
            window.AddEventListener(CloseEvent.CLOSE, delegate
            {
                PopupManager.Instance.RemovePopup(window);
            });

            PopupManager.Instance.AddPopup(window, false);
            PopupManager.Instance.CenterPopUp(window);
        }

        #endregion

    }
}