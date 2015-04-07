#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

#if TRIAL

using eDriven.Animation;
using eDriven.Animation.Easing;

namespace eDriven.Gui.Check
{
    internal class LockAlertAddedAnimation : Sequence
    {
        private static object StartValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { }) + 100f;
        }

        public LockAlertAddedAnimation()
        {
            Add(

                new SetProperty("Visible", true) { Name = "Setting Visible" },

                new SetProperty("Alpha", 0f) { Name = "Setting Alpha" },

                new Parallel(

                    new Tween
                        {
                            Property = "Alpha",
                            Duration = 0.5f,
                            Easer = Linear.EaseIn,
                            StartValue = 0f,
                            //StartValueReader = new PropertyReader("Alpha"),
                            EndValue = 1f
                        },

                    new Tween
                        {
                            Property = "Y",
                            Duration = 0.5f,
                            Easer = Expo.EaseOut,
                            EndValueReader = new PropertyReader("Y"),
                            StartValueReaderFunction = StartValueReaderFunc
                        }
                    )

                //new Action(delegate { Debug.Log("Finished"); })
                );
        }
    }
}

#endif