using eDriven.Animation;
using eDriven.Audio;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private readonly TweenFactory _dialogAddedEffect = new TweenFactory(
        new Sequence(
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
            new FadeInUp()
            //new Jumpy()
        )
    );

    private readonly TweenFactory _dialogRemovedEffect = new TweenFactory(
        new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_close"); })
    );

    private readonly TweenFactory _modalOverlayFadeIn = new TweenFactory(
        new ZeroFadeIn { Duration = 0.35f }
    );

// ReSharper disable UnusedMember.Local
    void Start()
// ReSharper restore UnusedMember.Local
    {
        Dialog.AddedEffect = _dialogAddedEffect;
        Dialog.RemovedEffect = _dialogRemovedEffect;

        ModalOverlay.AddedEffect = _modalOverlayFadeIn;
    }
}