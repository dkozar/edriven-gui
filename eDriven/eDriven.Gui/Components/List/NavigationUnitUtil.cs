using UnityEngine;

namespace eDriven.Gui.Components
{
    ///<summary>
    ///</summary>
    internal static class NavigationUnitUtil
    {
        ///<summary>
        ///</summary>
        ///<param name="keyCode"></param>
        ///<returns></returns>
        internal static bool IsNavigationUnit(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.LeftArrow:
                    return true;
                case KeyCode.RightArrow:
                    return true;
                case KeyCode.UpArrow:
                    return true;
                case KeyCode.DownArrow:
                    return true;
                case KeyCode.PageUp:
                    return true;
                case KeyCode.PageDown:
                    return true;
                case KeyCode.Home:
                    return true;
                case KeyCode.End:
                    return true;
                default: return false;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="keyCode"></param>
        ///<returns></returns>
        internal static NavigationUnit GetNavigationUnit(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.LeftArrow:
                    return NavigationUnit.Left;
                case KeyCode.RightArrow:
                    return NavigationUnit.Right;
                case KeyCode.UpArrow:
                    return NavigationUnit.Up;
                case KeyCode.DownArrow:
                    return NavigationUnit.Down;
                case KeyCode.PageUp:
                    return NavigationUnit.PageUp;
                case KeyCode.PageDown:
                    return NavigationUnit.PageDown;
                case KeyCode.Home:
                    return NavigationUnit.Home;
                //case KeyCode.End:
                    default:
                    return NavigationUnit.End;
            }
        }
    }
}