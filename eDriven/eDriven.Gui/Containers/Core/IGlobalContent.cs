using eDriven.Core.Geom;

namespace eDriven.Gui.Containers
{
    /// <summary>
    /// The ability for converting global to content coordinates and vice versa
    /// </summary>
    public interface IGlobalContent
    {
        /// <summary>
        /// Converts content coordinates to global coordinates
        /// </summary>
        /// <param name="p">Content coordinates</param>
        /// <returns>Global coordinates</returns>
        Point ContentToGlobal(Point p);

        /// <summary>
        /// Converts content bounds to global bounds
        /// </summary>
        /// <param name="r">Content bounds</param>
        /// <returns>Global bounds</returns>
        Rectangle ContentToGlobal(Rectangle r);

        /// <summary>
        /// Converts global coordinates to content coordinates
        /// </summary>
        /// <param name="p">Content coordinates</param>
        /// <returns>Global coordinates</returns>
        Point GlobalToContent(Point p);

        /// <summary>
        /// Converts global bounds to local bounds
        /// </summary>
        /// <param name="r">Global bounds</param>
        /// <returns>Local bounds</returns>
        Rectangle GlobalToContent(Rectangle r);
    }
}