//using eDriven.Animation;
//using eDriven.Animation.Easing;
//using eDriven.Gui.Containers;

//namespace eDriven.Playground.Demo.Tweens
//{
//    public class CollapseUpLeftAlpha : Sequence
//    {
//        public Stage Stage;

//        public CollapseUpLeftAlpha()
//        {
//            Add(

//                // tween alpha to 1.0
//                Tween.New()
//                    .SetProperty("Alpha")
//                    .SetOptions(
//                    new TweenOption(TweenOptionType.Duration, 1f),
//                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
//                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Alpha")),
//                    new TweenOption(TweenOptionType.EndValue, 1f)
//                    ),

//                // implode width to stage width
//                Tween.New()
//                    .SetProperty("Width")
//                    .SetOptions(
//                    new TweenOption(TweenOptionType.Duration, 1f),
//                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
//                    new TweenOption(TweenOptionType.StartValueReader,
//                                    new PropertyReader("Width")),
//                    new TweenOption(TweenOptionType.EndValue, 300f)
//                    ),

//                // implode height to stage height
//                Tween.New()
//                    .SetProperty("Height")
//                    .SetOptions(
//                    new TweenOption(TweenOptionType.Duration, 1f),
//                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
//                    new TweenOption(TweenOptionType.StartValueReader,
//                                    new PropertyReader("Height")),
//                    new TweenOption(TweenOptionType.EndValue, 150f)
//                    )
//                );
//        }
//    }
//}