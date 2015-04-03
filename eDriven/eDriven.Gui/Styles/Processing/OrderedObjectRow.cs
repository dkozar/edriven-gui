namespace eDriven.Gui.Styles
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class OrderedObjectRow<T>
    {
        ///<summary>
        ///</summary>
        public readonly string Name;
        
        ///<summary>
        ///</summary>
// ReSharper disable FieldCanBeMadeReadOnly.Global
        public T Value;
// ReSharper restore FieldCanBeMadeReadOnly.Global

        ///<summary>
        ///</summary>
        ///<param name="name"></param>
        ///<param name="value"></param>
        public OrderedObjectRow(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}