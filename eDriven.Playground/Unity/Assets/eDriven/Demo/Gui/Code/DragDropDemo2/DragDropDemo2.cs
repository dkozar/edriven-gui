using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.DragDrop;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Components;
using Assets.eDriven.Demo.Models;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

public class DragDropDemo2 : Gui
{
    private Group _box;

    private Panel _pnlSource;
    private Panel _pnlDest;

    private readonly string[] _icons =
    {
        "accessories-dictionary",
        "akonadi",
        "application-pgp",
        "application-pgp-signature",
        "applications-games",
        "applications-graphics",
        "applications-office",
        "applications-science",
        "appointment",
        "ardour",
        "battery",
        "camera-photo",
        "emblem-documents",
        "emblem-money",
        "ethereal",
        "gcrontab",
        "gtk-color-picker"
    };

    #region Effects

    private readonly TweenFactory _searchShowEffect = new TweenFactory(
        new Sequence(
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("portlet_add"); }),
            new FadeInLeft2()
        )
    ) { Delay = 0.07f };

    private readonly TweenFactory _panelShowEffect = new TweenFactory(
        new Sequence(
            new FadeIn()
        )
    ) { Delay = 0.07f };

    #endregion

    protected override void OnInitialize()
    {
        base.OnInitialize();

        OptionsModel.Instance.Volume = 0.5f;

        //Stage.AutoLayout = false;
        _searchShowEffect.Callback = delegate(IAnimation anim)
        {
            Stage.AutoLayout = true;
        };
    }

    protected override void CreateChildren()
    {
        base.CreateChildren();

        #region Heading

        TitleLabel button = new TitleLabel
        {
            StyleName = "title",
            HorizontalCenter = 0,
            Top = 20
        };

        AddChild(button);

        new TextRotator
        {
            Delay = 5, // 5 seconds delay
            Lines = new[]
                            {
                                "Drag and Drop Demo 2", 
                                "Created using eDriven.Gui",
                                //"Author: Danko Kozar",
                                "Drag items from the left panel (source)",
                                "Drop them to the right panel (destination)"
                            },
            Callback = delegate(string line) { button.Text = line; }
        }
            .Start();


        #endregion

        #region Box

        _box = new Group
                   {
                       HorizontalCenter = 0, VerticalCenter = 0,
                       Layout = new HorizontalLayout
                                    {
                                        HorizontalAlign = HorizontalAlign.Center,
                                        VerticalAlign = VerticalAlign.Top
                                    }
                   };

        // mandatory listeners
        AddEventListener(MouseEvent.MOUSE_DOWN, OnMouseDown); // mouse down
        AddEventListener(DragEvent.DRAG_ENTER, OnDragEnter); // drag enter
        AddEventListener(DragEvent.DRAG_DROP, OnDragDrop); // drag drop (on drop target)

        // optional listeners
        AddEventListener(DragEvent.DRAG_START, OnDragStart); // drag start(on drag initiator)
        AddEventListener(DragEvent.DRAG_EXIT, OnDragExit); // drag exit (on drop target)
        AddEventListener(DragEvent.DRAG_COMPLETE, OnDragComplete); // drag complete (on drag initiator)

        //_box.AddEventListener(MouseEvent.MOUSE_OVER, OnMouseOver);
        //_box.AddEventListener(MouseEvent.MOUSE_OUT, OnMouseOut);
        //_box.AddEventListener(MouseEvent.MOUSE_UP, OnMouseUp);

        AddChild(_box);

        #endregion

        #region Source

        _pnlSource = new Panel
                         {
                             Title = "Source",
                             Icon = Resources.Load<Texture>("Icons/star"),
                             //Width = 450,
                             //Height = 500,
                             MouseEnabled = true,
                             SkinClass = typeof(PanelSkin2),
                             Layout = new TileLayout
                             {
                                 Orientation = TileOrientation.Rows,
                                 HorizontalGap = 10,
                                 VerticalGap = 10,
                                 RowHeight = 128 + 20, // image = 128x128, padding = 10 + 10
                                 ColumnWidth = 128 + 20,
                                 RequestedRowCount = 3,
                                 RequestedColumnCount = 3
                             }
                         };
        _pnlSource.SetStyle("addedEffect", _panelShowEffect);

        _box.AddChild(_pnlSource);

        Button btnReset = new Button
        {
            Text = "Reset",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/arrow_refresh")
        };
        btnReset.Press += delegate { InitChildren(); };
        _pnlSource.ControlBarGroup.AddChild(btnReset);

        /*Label lbl = new Label { Text = "miki" };
        _pnlSource.ControlBarGroup.AddChild(lbl);*/

        #endregion

        #region Destination

        _pnlDest = new Panel
                       {
                           Title = "Destination",
                           Icon = Resources.Load<Texture>("Icons/star"),
                           //Width = 450,
                           //Height = 500,
                           MouseEnabled = true,
                           SkinClass = typeof(PanelSkin2),
                           Layout = new TileLayout
                           {
                               Orientation = TileOrientation.Rows,
                               HorizontalGap = 10,
                               VerticalGap = 10,
                               RowHeight = 128 + 20,
                               ColumnWidth = 128 + 20,
                               RequestedRowCount = 3,
                               RequestedColumnCount = 3
                           }
                       };
        _pnlDest.SetStyle("addedEffect", _panelShowEffect);

        _box.AddChild(_pnlDest);

        InitChildren();

        #endregion

    }

    private void InitChildren()
    {
        _pnlSource.RemoveAllContentChildren();
        _pnlDest.RemoveAllContentChildren();

        for (int i = 0; i < _icons.Length; i++)
        {
            Image image = new Image
                              {
                                  Texture = (Texture)Resources.Load(string.Format("IconsBig/{0}", _icons[i])),
                                  MouseEnabled = true,
                                  //ProcessClicks = true, 
                                  Data = (i + 1) * 100
                              };
            _pnlSource.AddContentChild(image);
        }
    }

    #region Drag & drop handlers

    private void OnMouseDown(Event e)
    {
        Image comp = e.Target as Image;

        if (null == comp)
            return;

        // check if dragged item is child of _pnlSource or _pnlDest
        if (_pnlSource.ContentContains(comp) || _pnlDest.ContentContains(comp))
        {
            DragSource dataSource = new DragSource();
            //dataSource.AddData(comp.Text, "text"); // add text for COPY_TEXT mode
            //dataSource.AddData(comp.StyleName, "style"); // add text for COPY_STYLE mode
            //dataSource.AddData(comp, "control"); // add reference to control for Move mode

            Image proxy = new Image
                              {
                                  Texture = comp.Texture, // reference the same texture
                                  ScaleMode = ImageScaleMode.ScaleToFit
                                  //// TEMP: handles the DragDropManager missing bounds clonning
                                  //Bounds = (Rectangle) comp.GlobalBounds.Clone(),

                                  //// TEMP: handles the DragDropManager missing MouseEnabled enabled turning off on the proxy
                                  //MouseEnabled = false
                              };

            DragDropManager.DoDrag(comp, dataSource, (MouseEvent)e, proxy, 0, 0, 0.5f, false);
            /*new DragOption(DragOptionType.ProxyVisible, false),
            new DragOption(DragOptionType.FeedbackVisible, false)*/
        }
    }

    private void OnDragEnter(Event e)
    {
        //Debug.Log("OnDragEnter: " + e.Target.GetType().Name);
        Component comp = (Component)e.Target;
        if (null == comp)
            return;

        AudioPlayerMapper.GetDefault().PlaySound("pager_click", new AudioOption(AudioOptionType.Volume, 0.3f));

        // check if drag enter item is child of _pnlDest or _pnlSource itself
        // allow the whole box to be drop target when moving
        if (_pnlDest.ContentContains(comp) || comp == _pnlDest.ContentGroup)
        {
            DragDropManager.AcceptDragDrop((Component) e.Target);
            DragDropManager.ShowFeedback(DragDropManager.Action.Move);
        }
    }

    private void OnDragDrop(Event e)
    {
        DragEvent dragEvent = (DragEvent) e;

        //Debug.Log("OnDragDrop: " + e.Target.GetType().Name);
        Component src = dragEvent.DragInitiator; //(UiComponent)dragEvent.DragSource.Formats["control"];
        Component dest = (Component)e.Target;

        Panel srcPanel = _pnlSource.ContentContains(src) ? _pnlSource : _pnlDest;

        if (dest == _pnlDest.ContentGroup)
        {
            srcPanel.RemoveContentChild(src);
            _pnlDest.AddContentChild(src);
        }
        else if (_pnlDest.ContentContains(dest))
        {
            srcPanel.RemoveContentChild(src); // NOTE: needed for drag&drop, because src and dest could be the same
            _pnlDest.AddContentChildAt(src, _pnlDest.GetContentChildIndex(dest));
        }

        AudioPlayerMapper.GetDefault().PlaySound("drag_drop", new AudioOption(AudioOptionType.Volume, 0.3f));
    }

    private void OnDragStart(Event e)
    {
        //Debug.Log("OnDragStart: " + e.Target.GetType().Name);
    }

    private void OnDragComplete(Event e)
    {
        //Debug.Log("OnDragComplete: " + e.Target.GetType().Name);
    }

    private void OnDragExit(Event e)
    {
        //Debug.Log("OnDragExit: " + e.Target.GetType().Name);
    }

    #endregion

}