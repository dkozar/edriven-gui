using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;

public class HelloWorld : Gui
{
    //private readonly TweenFactory _dlgOpen = new TweenFactory(
    //    new Sequence(
    //        new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
    //        new FadeIn()
    //    )  
    //);

    private readonly TweenFactory _dlgOpen2 = new TweenFactory(
        new Sequence(
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
            new FallDownToCenter()
        )
    );

    private readonly TweenFactory _overlayShow = new TweenFactory(
        new Sequence(
            new FadeIn()
        )
    );

    override protected void OnStart()
    {
        base.OnStart();

        Dialog.AddedEffect = _dlgOpen2;
        ModalOverlay.AddedEffect = _overlayShow;
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        VGroup vbox = new VGroup
        {
            Gap = 10,
            HorizontalCenter = 0, VerticalCenter = 0
        };
        AddChild(vbox);

        Button btn = new Button
        {
            Text = "Button 1",
            Icon = (Texture)Resources.Load("Icons/star"),
            SkinClass = typeof(ImageButtonSkin)
        };
        vbox.AddChild(btn);

        btn = new Button
        {
            Text = "Button 2",
            Icon = (Texture)Resources.Load("Icons/star"),
            StyleName = "button2",
            SkinClass = typeof(ImageButtonSkin)
        };
        vbox.AddChild(btn);

        btn = new Button
        {
            Text = "Button 3",
            Icon = (Texture)Resources.Load("Icons/star"),
            StyleName = "button3",
            SkinClass = typeof(ImageButtonSkin)
        };
        vbox.AddChild(btn);

        vbox.Click += delegate
        {
            Alert.Show(
                "Checking",
                "Are you sure you want to greet the world?",
                AlertButtonFlag.Yes | AlertButtonFlag.No,
                delegate (string action)
                {
                    switch (action)
                    {
                        case "yes":
                            Alert.Show(
                                "Hello",
                                "Hello world!",
                                AlertButtonFlag.Ok,
                                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/accept"))
                            );
                            break;
                        case "no":
                            Alert.Show(
                                "Going to sleep",
                                "Good night.",
                                AlertButtonFlag.Ok,
                                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/cancel"))
                            );
                            break;
                    }
                },
                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/information"))
            );
        };
    }
}