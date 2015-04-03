using System;

namespace Assets.eDriven.Demo.Helpers
{
    public static class TileHelper
    {
        #region Helper

        public static int CalculateItemsPerPage(float itemWidth, float itemHeight, float parentWidth, float parentHeight, float horizontalSpacing, float verticalSpacing)
        {
            //var xCount = 1;
            //var yCount = 1;

            var xCount = 0;
            var yCount = 0;

            var spcX = parentWidth;
            while (spcX > itemWidth)
            {
                xCount++;
                spcX -= (itemWidth + horizontalSpacing);
            }

            var spcY = parentHeight;
            while (spcY > itemHeight)
            {
                yCount++;
                spcY -= (itemHeight + verticalSpacing);
            }

            xCount = Math.Max(xCount, 1);
            yCount = Math.Max(yCount, 1);

            //// calc x
            //if (itemWidth < parentWidth)
            //{
            //    xCount = Math.Max((int)Math.Floor(parentWidth / (itemWidth + horizontalSpacing)), 1);
            //}

            //// calc y
            //if (itemHeight < parentHeight)
            //{
            //    yCount = Math.Max((int)Math.Floor(parentHeight / (itemHeight + verticalSpacing)), 1);
            //}

            return xCount * yCount;
        }

        #endregion
    }
}