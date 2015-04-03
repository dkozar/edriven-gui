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
    public class ResizeInstance : AnimatorInstanceBase
    {
        private static readonly object[] O = new object[] { };

        private static object StartWidthValueReaderFunc(object animTarget)
        {
            return (float)animTarget.GetType().GetProperty("Width").GetValue(animTarget, O);
        }

        private static object StartHeightValueReaderFunc(object animTarget)
        {
            return (float)animTarget.GetType().GetProperty("Height").GetValue(animTarget, O);
        }

        private TweenBase _tx;
        private TweenBase _ty;

        public override void Configure(object target)
        {
            //Tween.Delay = Delay;
            //Tween.Duration = Duration;

            bool goingX = false;
            bool goingY = false;

            if (null != WidthTo || null != WidthBy)
                goingX = true;
                //throw new Exception("XTo nor XBy value not defined");

            if (null != HeightTo || null != HeightBy)
                goingY = true;
                //throw new Exception("YTo nor YBy value not defined");

            if (!goingX && !goingY)
            {
                Debug.LogWarning("Not Width nor Height resize");
                return;
            }
                
            Parallel parallel = new Parallel();

            if (goingX)
            {
                float widthStart = (float)(WidthFrom ?? StartWidthValueReaderFunc(target));

                _tx = new Tween
                {
                    Name = "ResizeX",
                    Property = "Width",
                    Duration = Duration,
                    Easer = Easer,
                    StartValue = widthStart,
                    EndValue = WidthTo ?? widthStart + WidthBy
                };

                parallel.Add(_tx);
            }

            if (goingY)
            {
                float heightStart = (float)(HeightFrom ?? StartHeightValueReaderFunc(target));

                _ty = new Tween
                {
                    Name = "ResizeY",
                    Property = "Height",
                    Duration = Duration,
                    Easer = Easer,
                    StartValue = heightStart,
                    EndValue = HeightTo ?? heightStart + HeightBy
                };

                parallel.Add(_ty);
            }

            Tween = parallel;

            //Debug.Log("_tx.StartValue " + _tx.StartValue);
            //Debug.Log("_tx.EndValue " + _tx.EndValue);
            //Debug.Log("_ty.StartValue " + _ty.StartValue);
            //Debug.Log("_ty.EndValue " + _ty.EndValue);
        }

        public float? WidthFrom { get; set; }
        public float? WidthTo { get; set; }
        public float? HeightFrom { get; set; }
        public float? HeightTo { get; set; }
        public float? WidthBy { get; set; }
        public float? HeightBy { get; set; }

        public override string ToString()
        {
            return string.Format(@"ResizeInstance [Target: {6}][WidthFrom: {0}, WidthTo: {1}, HeightFrom: {2}, HeightTo: {3}, WidthBy: {4}, HeightBy: {5}]",
                null != WidthFrom ? Convert.ToString(WidthFrom) : "-",
                null != WidthTo ? Convert.ToString(WidthTo) : "-",
                null != HeightFrom ? Convert.ToString(HeightFrom) : "-",
                null != HeightTo ? Convert.ToString(HeightTo) : "-",
                null != WidthBy ? Convert.ToString(WidthBy) : "-",
                null != HeightBy ? Convert.ToString(HeightBy) : "-",
                Target ?? "-"
            );
        }

        public override object Clone()
        {
            return new ResizeInstance
                       {
                           //Tween = (ITween)Tween.Clone()
                       };
        }
    }
}
