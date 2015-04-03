using eDriven.Core.Geom;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// The ability for converting global to local coordinates and vice versa
    ///</summary>
    public interface IGlobalLocal
    {
        /// <summary>
        /// Converts local (component) coordinates to global (screen) coordinates
        /// </summary>
        /// <param name="p">Local coordinates</param>
        /// <returns>Global coordinates</returns>
        Point LocalToGlobal(Point p);

        /// <summary>
        /// Converts local (component) bounds to global (screen) bounds
        /// </summary>
        /// <param name="r">Local rectangle</param>
        /// <returns>Global rectangle</returns>
        Rectangle LocalToGlobal(Rectangle r);

        /// <summary>
        /// Converts global (screen) coordinates to local (component) coordinates
        /// </summary>
        /// <param name="p">Global coordinates</param>
        /// <returns>Local coordinates</returns>
        Point GlobalToLocal(Point p);

        /// <summary>
        /// Converts global (screen) bounds to local (component) bounds
        /// </summary>
        /// <param name="r">Local rectangle</param>
        /// <returns>Global rectangle</returns>
        Rectangle GlobalToLocal(Rectangle r);
    }
}