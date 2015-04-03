using eDriven.Core.Geom;

namespace eDriven.Gui.Containers
{
    /// <summary>
    /// The ability for converting local to contenr coordinates and vice versa
    /// </summary>
    public interface ILocalContent
    {
        /// <summary>
        /// Converts local coordinates to content coordinates
        /// </summary>
        /// <param name="p">Local coordinates</param>
        /// <returns>Content coordinates</returns>
        Point LocalToContent(Point p);

        /// <summary>
        /// Converts local bounds to content bounds
        /// </summary>
        /// <param name="r">Local bounds</param>
        /// <returns>Content bounds</returns>
        Rectangle LocalToContent(Rectangle r);

        /// <summary>
        /// Converts content coordinates to local coordinates
        /// </summary>
        /// <param name="p">Content coordinates</param>
        /// <returns>Local coordinates</returns>
        Point ContentToLocal(Point p);

        /// <summary>
        /// Converts content bounds to local bounds
        /// </summary>
        /// <param name="r">Content bounds</param>
        /// <returns>Local bounds</returns>
        Rectangle ContentToLocal(Rectangle r);
    }
}