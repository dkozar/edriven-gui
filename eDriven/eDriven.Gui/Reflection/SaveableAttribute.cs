namespace eDriven.Gui.Reflection
{
    public class SaveableAttribute : System.Attribute
    {
        private bool _isSaveable = true; // default
        public SaveableAttribute(bool value)
        {
            _isSaveable = value;
        }

        public SaveableAttribute()
        {
        }

        public bool IsSaveable
        {
            get { return _isSaveable; }
            set { _isSaveable = value; }
        }
    }
}