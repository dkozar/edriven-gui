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

using System;
using UnityEngine;

namespace eDriven.Animation.Wrappers
{
    public class MoveInstance : AnimatorInstanceBase
    {
        private static readonly object[] O = new object[] {};

        private static object StartXValueReaderFunc(object animTarget)
        {
            return (float)animTarget.GetType().GetProperty("X").GetValue(animTarget, O);
        }

        private static object StartYValueReaderFunc(object animTarget)
        {
            return (float)animTarget.GetType().GetProperty("Y").GetValue(animTarget, O);
        }

        private TweenBase _tx;
        private TweenBase _ty;
        
        public override void Configure(object target)
        {
            //Tween.Delay = Delay;
            //Tween.Duration = Duration;

            bool goingX = false;
            bool goingY = false;

            if (null != XTo || null != XBy)
                goingX = true;
                //throw new Exception("XTo nor XBy value not defined");

            if (null != YTo || null != YBy)
                goingY = true;
                //throw new Exception("YTo nor YBy value not defined");

            if (!goingX && !goingY)
            {
                Debug.LogWarning("Not X nor Y move");
                return;
            }
                
            Parallel parallel = new Parallel();

            if (goingX)
            {
                float xStart = (float)(XFrom ?? StartXValueReaderFunc(target));

                _tx = new Tween
                {
                    Name = "MoveX",
                    Property = "X",
                    Duration = Duration,
                    Easer = Easer,
                    StartValue = xStart,
                    EndValue = XTo ?? xStart + XBy
                };

                parallel.Add(_tx);
            }

            if (goingY)
            {
                float yStart = (float)(YFrom ?? StartYValueReaderFunc(target));

                _ty = new Tween
                {
                    Name = "MoveY",
                    Property = "Y",
                    Duration = Duration,
                    Easer = Easer,
                    StartValue = yStart,
                    EndValue = YTo ?? yStart + YBy
                };

                parallel.Add(_ty);
            }

            Tween = parallel;

            //Debug.Log("_tx.StartValue " + _tx.StartValue);
            //Debug.Log("_tx.EndValue " + _tx.EndValue);
            //Debug.Log("_ty.StartValue " + _ty.StartValue);
            //Debug.Log("_ty.EndValue " + _ty.EndValue);
        }

        public float? XFrom { get; set; }
        public float? YFrom { get; set; }
        public float? XTo { get; set; }
        public float? YTo { get; set; }
        public float? XBy { get; set; }
        public float? YBy { get; set; }

        public override string ToString()
        {
            return string.Format(@"MoveInstance [Target: {6}][XFrom: {0}, XTo: {1}, YFrom: {2}, YTo: {3}, XBy: {4}, YBy: {5}]",
                null != XFrom ? Convert.ToString(XFrom) : "-", 
                null != XTo ? Convert.ToString(XTo) : "-", 
                null != YFrom ? Convert.ToString(YFrom) : "-", 
                null != YTo ? Convert.ToString(YTo) : "-", 
                null != XBy ? Convert.ToString(XBy) : "-", 
                null != YBy ? Convert.ToString(YBy) : "-",
                Target ?? "-"
            );
        }

        public override object Clone()
        {
            return new MoveInstance
                       {
                           //Tween = (ITween)Tween.Clone()
                       };
        }
    }
}
