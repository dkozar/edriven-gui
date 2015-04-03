namespace eDriven.Gui.Util
{
    /// <summary>
    /// Proxies call to Resources.Load()
    /// You could do your stuff with it, for instance cache it to prevent memory leaks TM ^_^
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResourceLoaderProxy<T>
    {
        T Load(string path);
    }
}