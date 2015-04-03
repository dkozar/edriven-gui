using System.Collections.Generic;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.States
{
    public class DefaultButtonStateFactory
    {
        /// <summary>
        /// Note: state creation cannot be static/singleton!
        /// Each and every object needs its own set of states
        /// </summary>
        /// <returns></returns>
        public static List<State> CreateStates()
        {
            return new List<State>
            {
                new State("up")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", ColorMixer.FromHex(0xeeeeee).ToColor()), // 0xfffffe??
                    }
                }, 
                new State("over")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", ColorMixer.FromHex(0xf9f9f9).ToColor()), // 0xfffffe??

                        #region Just for test
                        
                        /* Can set child property */
                        //new SetProperty("IconDisplay", "Color", Color.red),
                        //new SetProperty("IconDisplay", "Scale", new Vector2(2f, 2f)),
                        
                        /* Can access private part */
                        //new SetProperty("_hGroup", "Rotation", 30)
                        
                        #endregion
                    }
                }, 
                new State("down")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0xdadada).ToColor())
                    }
                },
                new State("disabled")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Alpha", 0.6f)
                    }
                },
                new State("upAndSelected")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0x439dde).ToColor()),
                        new SetStyle("textColor", Color.white)
                    }
                }, 
                new State("overAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0x4488fc).ToColor())
                    }
                }, 
                new State("downAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0x1261c1).ToColor())
                    }
                },
                new State("disabledAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Alpha", 0.6f)
                    }
                }
            };
        }
    }
}
