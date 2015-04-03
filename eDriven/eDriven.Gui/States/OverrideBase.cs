using eDriven.Core.Reflection;
using eDriven.Gui.Components;

namespace eDriven.Gui.States
{
    public abstract class OverrideBase : IOverride
    {
        protected bool Applied;

        public abstract void Initialize();

        public abstract void Apply(Component parent);

        public abstract void Remove(Component parent);

        /**
         * 
         * Param: parent The document level context for this override.
         * Param: target The component level context for this override.
         */
        /// <summary>
        /// Gets the targeted member based on the folowing logic:
        /// If target is supplied - as object - get its member using name
        /// If target is supplied - as string - find a target (object) and then get its member using name
        /// If target is not supplied - get the member on parent using name
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected MemberProxy GetMember(object target, Component parent, string name)
        {
            if (target == null)
                return new MemberProxy(parent, name);

            if (target is string)
                target = new MemberProxy(parent, (string) target).GetValue();

            return new MemberProxy(target, name);
        }

        /// <summary>
        /// Returns the target based on the folowing logic:
        /// If target is supplied - as object - return it
        /// If target is supplied - as string - find a target (object) and return it
        /// If target is not supplied - get the parent object
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected object GetTarget(object target, Component parent)
        {
            if (target == null)
                return parent;

            if (target is string)
                return new MemberProxy(parent, (string)target).GetValue();

            return target;
        }
    }
}