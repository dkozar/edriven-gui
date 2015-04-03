//using eDriven.Animation;
//using eDriven.Animation.Easing;
//using eDriven.Animation.Interpolators;
//using eDriven.Gui;
//using eDriven.Gui.Components;
//using eDriven.Gui.Stages;
//using UnityEngine;

//public class EDrivenLogo : Gui
//{
//    public bool LogoVisible;
//    private TweenFactory _showEffect;
//    private Image _img;

//    private Texture _texture;

//    protected override void CreateChildren()
//    {
//        base.CreateChildren();

//        _texture = (Texture) Resources.Load("edriven");

//        _img = new Image
//                   {
//                       Texture = _texture,
//                       X = Screen.width + 10,
//                       Bottom = 10,
//                       MouseEnabled = false
//                   };

//        InspectorOverlayStage.Instance.AddChild(_img);
//    }

//    protected override void OnCreationComplete()
//    {
//        base.OnCreationComplete();

//        if (LogoVisible)
//        {
//            RunLogoAnimation();
//        }
//    }

//    private void RunLogoAnimation()
//    {
//        //Debug.Log("RunLogoAnimation");

//        _showEffect = new TweenFactory(

//            new Sequence(

//               new Pause { Duration = Application.isWebPlayer ? 9 : 1 },

//               new Action(delegate { Debug.Log("Started"); }),

//               Tween.New()
//                   .SetOptions(
//                       new TweenOption(TweenOptionType.Property, "X"),
//                       new TweenOption(TweenOptionType.Interpolator, new FloatInterpolator()),
//                       new TweenOption(TweenOptionType.Duration, 1f),
//                       new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
//                       new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("X")),
//                       new TweenOption(TweenOptionType.EndValue, Screen.width - _texture.width - 10f)
//                    ),

//                new Pause { Duration = 4f },

//                Tween.New()
//                   .SetOptions(
//                       new TweenOption(TweenOptionType.Property, "X"),
//                       new TweenOption(TweenOptionType.Interpolator, new FloatInterpolator()),
//                       new TweenOption(TweenOptionType.Duration, 0.7f),
//                       new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Back.EaseIn),
//                       //new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Right")),
//                       new TweenOption(TweenOptionType.StartValue, Screen.width - _texture.width - 10f),
//                       new TweenOption(TweenOptionType.EndValue, Screen.width + 10f)
//                    ),

//                new Action(delegate { InspectorOverlayStage.Instance.RemoveChild(_img); })

//            ){Name = "Logo animation"}
//        );

//        _showEffect.Play(_img);
//    }
//}