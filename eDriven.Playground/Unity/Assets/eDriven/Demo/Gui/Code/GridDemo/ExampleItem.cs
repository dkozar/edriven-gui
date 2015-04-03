using eDriven.Core.Events;
using eDriven.Gui.Events;

namespace Assets.eDriven.Demo.Gui.Code.GridDemo
{
    /// <summary>
    /// The data item must extend EventDispatcher
    /// </summary>
    public class ExampleItem : EventDispatcher
    {
        
        private string _firstName;
        public string FirstName
        {
            get { 
                return _firstName;
            }
            set
            {
                if (value == _firstName)
                    return;

                var oldValue = _firstName;
                _firstName = value;
                DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "FirstName", FirstName, oldValue));
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (value == _lastName)
                    return;

                var oldValue = _lastName;
                _lastName = value;
                DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "LastName", LastName, oldValue));
            }
        }

        private int _age;
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                if (value == _age)
                    return;

                var oldValue = _age;
                _age = value;
                DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "Age", Age, oldValue));
            }
        }

        private bool _drivingLicense;
        public bool DrivingLicense
        {
            get { 
                return _drivingLicense;
            }
            set
            {
                if (value == _drivingLicense)
                    return;

                var oldValue = _drivingLicense;
                _drivingLicense = value;
                DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "DrivingLicense", DrivingLicense, oldValue));
            }
        }

        public override string ToString()
        {
            return string.Format(@"First name: {0}
Last name: {1}
Age: {2}
Driving license: {3}", FirstName, LastName, Age, DrivingLicense);
        }
    }
}
