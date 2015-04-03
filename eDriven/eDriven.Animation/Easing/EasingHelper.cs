namespace eDriven.Animation.Easing
{
    public class EasingHelper
    {
        public static Tween.EasingFunction GetEaser(string className, string methodName)
        {
            switch (className)
            {
                case "Back":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Back.EaseIn;
                        case "EaseOut":
                            return Back.EaseOut;
                        case "EaseInOut":
                            return Back.EaseInOut;
                    }
                    break;
                case "Bounce":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Bounce.EaseIn;
                        case "EaseOut":
                            return Bounce.EaseOut;
                        case "EaseInOut":
                            return Bounce.EaseInOut;
                    }
                    break;
                case "Circ":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Circ.EaseIn;
                        case "EaseOut":
                            return Circ.EaseOut;
                        case "EaseInOut":
                            return Circ.EaseInOut;
                    }
                    break;
                case "Cubic":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Cubic.EaseIn;
                        case "EaseOut":
                            return Cubic.EaseOut;
                        case "EaseInOut":
                            return Cubic.EaseInOut;
                    }
                    break;
                case "Elastic":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Elastic.EaseIn;
                        case "EaseOut":
                            return Elastic.EaseOut;
                        case "EaseInOut":
                            return Elastic.EaseInOut;
                    }
                    break;
                case "Expo":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Expo.EaseIn;
                        case "EaseOut":
                            return Expo.EaseOut;
                        case "EaseInOut":
                            return Expo.EaseInOut;
                    }
                    break;
                case "Linear":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Linear.EaseIn;
                        case "EaseOut":
                            return Linear.EaseOut;
                        case "EaseInOut":
                            return Linear.EaseInOut;
                    }
                    break;
                case "Quad":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Quad.EaseIn;
                        case "EaseOut":
                            return Quad.EaseOut;
                        case "EaseInOut":
                            return Quad.EaseInOut;
                    }
                    break;
                case "Quart":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Quart.EaseIn;
                        case "EaseOut":
                            return Quart.EaseOut;
                        case "EaseInOut":
                            return Quart.EaseInOut;
                    }
                    break;
                case "Quint":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Quint.EaseIn;
                        case "EaseOut":
                            return Quint.EaseOut;
                        case "EaseInOut":
                            return Quint.EaseInOut;
                    }
                    break;
                case "Sine":
                    switch (methodName)
                    {
                        case "EaseIn":
                            return Sine.EaseIn;
                        case "EaseOut":
                            return Sine.EaseOut;
                        case "EaseInOut":
                            return Sine.EaseInOut;
                    }
                    break;
                default:
                    return Linear.EaseNone;

            }
            return null;
        }
    }
}
