namespace eDriven.Gui.Styles.MediaQueries
{
    /// <summary>
    /// The base class for media queries
    /// </summary>
    public abstract class MediaQueryBase
    {
        /// <summary>
        /// Media query ID
        /// </summary>
        public string Id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public abstract bool Evaluate(float width, float height); // params string[] parameters
    }
}