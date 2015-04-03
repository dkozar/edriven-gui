using System;
using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Core.Signals;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Component = UnityEngine.Component;

namespace eDriven.Gui.Styles
{
    ///<summary>
    /// Contains methods for traversing component trees for styling purposes
    ///</summary>
    public class UnityComponentTraverser : ComponentTraverser<Component>
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static UnityComponentTraverser _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private UnityComponentTraverser()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static UnityComponentTraverser Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating UnityComponentTraverser instance"));
#endif
                    _instance = new UnityComponentTraverser();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {

        }

        private static Signal _selectorSignal;
        
        ///<summary>
        /// Emits the list of component references that are being processed by tweaking up style sheet in play mode
        ///</summary>
        public static Signal SelectorSignal
        {
            get { return _selectorSignal ?? (_selectorSignal = new Signal()); }
        }

        ///<summary>
        ///</summary>
        override public List<Component> GetComponentsMatchingSelector(Selector selector)
        {
            //Debug.Log("##### GetComponentsMatchingSelector #####");
            var topTransforms = TransformUtil.FetchTopTransforms();
            var count = topTransforms.Count;

            List<Component> components = new List<Component>();

            for (int i = 0; i < count; i++)
            {
                var component = topTransforms[i];
                FindComps(components, component, selector);
            }

            if (null != _selectorSignal)
                _selectorSignal.Emit(components);
            
            return components;
        }

        public override void InitStyles(Selector selector, StyleTable styles)
        {
            var components = GetComponentsMatchingSelector(selector);
            foreach (Component component in components)
            {
                // 2. for additions, set the style
                foreach (KeyValuePair<string, object> pair in styles)
                {
                    MemberWrapper wrapper = new MemberWrapper(component.GetType(), pair.Key);
                    wrapper.SetValue(component, pair.Value);
                }
            }
        }

        public override void UpdateStyles(Selector selector, DictionaryDelta delta)
        {
            // nothing yet
            //Debug.Log("UpdateStyles: " + delta);
            var components = GetComponentsMatchingSelector(selector);

            delta.Process();

            foreach (Component component in components)
            {
                // TODO: find out which property changed and its value
                //var changedProp = _modifiedPropertyName;
                //component.SetStyle("paddingLeft", 30);

                // 1. for removals, clear the style
                foreach (string removal in delta.Removals.Keys)
                {
                    // TODO: set the default value (if exists)
                    // (default values should be implemented via the attribute)
                    //Debug.Log("Removing -> " + removal);
                    MemberWrapper wrapper = new MemberWrapper(component.GetType(), removal);

                    object value = null;
                    var attributes = CoreReflector.GetMemberAttributes<StyleAttribute>(wrapper.MemberInfo);
                    if (attributes.Count > 0)
                        value = attributes[0].GetDefault();

                    wrapper.SetValue(component, value);
                }
                // 2. for additions, set the style
                foreach (KeyValuePair<string, object> addition in delta.Additions)
                {
                    MemberWrapper wrapper = new MemberWrapper(component.GetType(), addition.Key);
                    wrapper.SetValue(component, addition.Value);
                }

                // 3. for updates, set the style
                foreach (KeyValuePair<string, object> update in delta.Updates)
                {
                    MemberWrapper wrapper = new MemberWrapper(component.GetType(), update.Key);
                    wrapper.SetValue(component, update.Value);
                }

                UnityComponentStylingGizmo.Show(components);
            }
        }

        private void BuildStyleTree(ICollection<Component> components, Transform transform)
        {
            /*if (transform.name == "Test")
            {
                Debug.Log("transform: " + transform);
            }*/

            var list = new List<Component>();

            var componentsOnTransform = transform.GetComponents<Component>();
            list.AddRange(componentsOnTransform);

            /*var mbsOnTransform = transform.GetComponents<MonoBehaviour>();
            list.AddRange(mbsOnTransform);*/

            foreach (var component in list)
            {
                if (null == component)
                    continue; // TODO: there is a case when component is null -> investigate!!!
               
                components.Add(component);
            }

            foreach (Transform child in transform.transform)
            {
                BuildStyleTree(components, child);
            }
        }

        #region Helper

        private void FindComps(ICollection<Component> components, Transform transform, Selector selector)
        {
            /*if (transform.name == "Test")
            {
                Debug.Log("transform: " + transform);
            }*/

            var list = new List<Component>();

            var componentsOnTransform = transform.GetComponents<Component>();
            list.AddRange(componentsOnTransform);

            /*var mbsOnTransform = transform.GetComponents<MonoBehaviour>();
            list.AddRange(mbsOnTransform);*/

            foreach (var component in list)
            {
                if (null == component)
                    continue; // TODO: there is a case when component is null -> investigate!!!

                /**
                 * Note: not equal, but containing
                 * We are looking for components having this selector as a part of theirs
                 * For instance, if selector is "Button", then "Button.miki" should also be processed (but only if not overriden with .miki declaration TODO)
                 * */
                if (StyleClientManager.Instance.MatchesSelector(component, selector))
                    components.Add(component);
            }

            foreach (Transform child in transform.transform)
            {
                FindComps(components, child, selector);
            }
        }

        

        #endregion

    }
}
