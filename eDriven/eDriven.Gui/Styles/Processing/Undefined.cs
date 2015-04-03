using System;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// A class representing the undefined value
    /// </summary>
    internal sealed class Undefined : IEquatable<Undefined>
    {
        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(Undefined other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Undefined) obj);
        }

        public static bool operator ==(Undefined left, Undefined right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Undefined left, Undefined right)
        {
            return !Equals(left, right);
        }
    }
}