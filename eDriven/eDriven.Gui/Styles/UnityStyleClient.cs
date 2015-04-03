/*
using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using Component = UnityEngine.Component;

namespace eDriven.Gui.Styles
{
    internal class UnityStyleClient : IStyleClient
    {
        public static Dictionary<Component, IStyleClient> Dict = new Dictionary<Component, IStyleClient>();

        public Component Component { get; private set; }
        public UnityStyleClient(Component component)
        {
            if (null == component)
                throw new Exception("Cannot pass null as a component");
            Component = component;

            Dict[component] = this;
        }

        public void StyleChanged(string styleName)
        {
            
        }

        public object StyleName
        {
            get { return Component.gameObject.name; }
            set { }
        }

        public StyleTable InheritingStyles { get; set; }
        public StyleTable NonInheritingStyles { get; set; }
        public StyleDeclaration StyleDeclaration { get; set; }
        public object GetStyle(string styleName)
        {
            return null;
        }

        public void SetStyle(string styleName, object style)
        {
            
        }

        public void ClearStyle(string styleName)
        {
            
        }

        public bool HasStyle(string styleName)
        {
            return false;
        }

        public void RegenerateStyleCache(bool recursive)
        {
            InitProtoChain();

            // Recursively call this method on each child.
            var n = NumberOfChildren; // QNumberOfChildren;

            for (int i = 0; i < n; i++)
            {
                var child = GetChildAt(i); // QGetChildAt(i);

                var client = child as IStyleClient;
                if (client != null)
                {
                    // Does this object already have a proto chain? 
                    // If not, there's no need to regenerate a new one.
                    if (client.InheritingStyles !=
                        StyleProtoChain.STYLE_UNINITIALIZED)
                    {
                        client.RegenerateStyleCache(recursive);
                    }
                }
            }
        }

        private IStyleClient GetChildAt(int i)
        {
            Dict[]

            return Component.transform.GetChild(i);
        }

        public decimal NumberOfChildren
        {
            get
            {
                return Component.transform.childCount;
            }
        }

        internal void InitProtoChain()
        {
            StyleProtoChain.InitProtoChain(this);
        }

        public void NotifyStyleChangeInChildren(string prop, object value, bool recursive)
        {
            
        }

        public List<StyleDeclaration> GetClassStyleDeclarations()
        {
            return null;
        }

        public string Id
        {
            get { return Component.gameObject.name; }
            set { }
        }

        public IStyleClient StyleParent { get; private set; }
        public bool MatchesCSSState(string cssState)
        {
            return false;
        }

        public bool MatchesCSSType(string cssState)
        {
            return false;
        }
    }
}
*/
