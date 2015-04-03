namespace Assets.eDriven.Demo.Scripts
{
    public class TestClass
    {
        private string _property = "foo";
        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public string Field = "bar";

        public string Method()
        {
            return "baz";
        }
    }
}