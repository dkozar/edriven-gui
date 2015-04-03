using System;
using eDriven.Animation.Wrappers;
using eDriven.Audio;
using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Core.Util;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Stages;
using Assets.eDriven.Demo.Components;
using Assets.eDriven.Demo.Scripts;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

[AddComponentMenu("eDriven/Gui/GuiInspector")]

public class OptionsToolbar : Gui
{
    private readonly Move _show = new Move { YTo = 0, Duration = 0.7f, Name = "Options show"};

    private readonly Move _hide = new Move { YTo = -150, Duration = 0.7f, Name = "Options hide" };

    private Options _options;

    private static VGroup _vbox;

    Timer _timer;

    public bool HideInEditor;

    protected override void OnStart()
    {
        base.OnStart();

        SystemManager.Instance.DisposingSignal.Connect(DisposingSlot, true);
    }

    private static void DisposingSlot(object[] parameters)
    {
        if (null != _vbox)
        {
            _vbox.Dispose();
            _vbox = null;
        }
    }

    /// <summary>
    /// Note: Since Load could happen, if both scene containing OptionsToolbar, we wouldn't like to instantiate twice
    /// Thus we made 2 checks in for toolbar existance in this class
    /// </summary>
    protected override void CreateChildren()
    {
        if (HideInEditor && Application.isEditor)
            return;

        base.CreateChildren();

        if (null != _vbox)
            return; // meaning toolbar created in the previous scene

        _vbox = new VGroup
                        {
                            Right = 10,
                            MinWidth = 160,
                            Gap = 0
                        };
        OptionsToolbarStage.Instance.AddChild(_vbox); // add to InspectorOverlayStage

        _options = new Options();
        _options.SetStyle("showBackground", true);
        _vbox.AddChild(_options);
        
        Button handle = new Button
                         {
                             StyleName = "handle_horiz", 
                             FocusEnabled = false, 
                             PercentWidth = 100, 
                             ResizeWithStyleBackground = true,
                             Text = "Options"
                         };
        _vbox.AddChild(handle);

        handle.AddEventListener(MouseEvent.ROLL_OVER,
            delegate(Event e)
            {
                if (handle == e.Target)
                {
                    Show();
                }
                e.CancelAndStopPropagation();
            },
            EventPhase.Capture | EventPhase.Target
        );
        _vbox.AddEventListener(MouseEvent.ROLL_OVER, // deffering hide
            delegate
            {
                if (null != _timer)
                {
                    _timer.Reset();
                    _timer.Stop();
                }
            },
            EventPhase.Capture | EventPhase.Target
        );
        _vbox.AddEventListener(MouseEvent.ROLL_OUT,
            delegate(Event e)
            {
                if (_vbox == e.Target)
                {
                    if (null == _timer)
                        _timer = new Timer(1, 1);
                    _timer.Complete += delegate
                    {
                        Hide();
                        _timer.Dispose();
                        _timer = null;
                    };
                    _timer.Start();
                }
                e.CancelAndStopPropagation();
            },
            EventPhase.Capture | EventPhase.Target
        );

        _vbox.ValidateNow(); // invoke measure

        float h = _options.Height;

        _vbox.Y = -h;
        _hide.YTo = -h;
    }

    private bool _isShown;
    private void Show()
    {
        if (_isShown)
            return;

        _show.Play(_vbox);
        AudioPlayerMapper.GetDefault().PlaySound("panel_show");
        _options.UpdateValues();
        _isShown = true;
    }

    private void Hide()
    {
        if (!_isShown)
            return;

        _hide.Play(_vbox);
        AudioPlayerMapper.GetDefault().PlaySound("panel_hide");
        _isShown = false;
    }

    protected override void OnCreationComplete()
    {
        base.OnCreationComplete();

        if (null == _options)
            return; // meaning toolbar created in the previous scene

        _hide.YTo = - _options.Height;
        _hide.Play(_vbox);
    }
}