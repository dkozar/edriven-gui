using System;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Util;

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// Skin utility
    /// </summary>
    public static class SkinUtil
    {
        /// <summary>
        /// Returns true if the skin fits to component<br/>
        /// The skin fits to component if it contains all the required parts (being of required types)
        /// </summary>
        /// <param name="skinType"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static bool SkinFitsComponent(Type skinType, Type componentType)
        {
            var parts = SkinPartCache.Instance.Get(componentType);

            /**
             * 1. Must have all the required skin parts
             * */
            foreach (var id in parts.Keys)
            {
                if (!parts[id]) // if part not required, continue
                    continue;

                var cmpMember = GlobalMemberCache.Instance.Get(componentType, id);
                if (null == cmpMember)
                {
                    cmpMember = new MemberWrapper(componentType, id);
                    GlobalMemberCache.Instance.Put(componentType, id, cmpMember);
                }

                var skinMember = GlobalMemberCache.Instance.Get(componentType, id);
                if (null == skinMember)
                {
                    /* TODO: undirty this */
                    try
                    {
                        skinMember = new MemberWrapper(skinType, id);
                        GlobalMemberCache.Instance.Put(componentType, id, skinMember);
                    }
                    catch (MemberNotFoundException ex)
                    {
                        // ignore because the member doesn't exist
                    }
                }

                if (null == skinMember || skinMember.GetType() != cmpMember.GetType())
                {
                    return false;
                }
            }

            /**
             * 2. Must have the same set of states
             * */
            // TODO:
            var skinStates = GuiReflector.GetSkinStates(componentType);

            var hostComponentType = GetHostComponent(skinType);
            var componentStates = GuiReflector.GetSkinStates(hostComponentType);

            // in current impl. the states have to be in the same order
            var sameStates = ListUtil<string>.Equals(skinStates, componentStates);
            return sameStates;
            //return true;
        }

        public static Type GetHostComponent(Type skinType)
        {
            var hostComponentAttributes = CoreReflector.GetClassAttributes<HostComponentAttribute>(skinType);

            //Debug.Log("hostComponentAttributes.Length: " + hostComponentAttributes.Length);

            if (hostComponentAttributes.Count > 0)
            {
                var attr = hostComponentAttributes[0];
                return attr.Type;
            }
            return null;
        }
    }
}
