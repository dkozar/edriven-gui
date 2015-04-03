using System;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.Styles
{
	internal static class TypeHierarchyHelper
	{
#if DEBUG
		// ReSharper disable UnusedMember.Global
		// ReSharper disable InconsistentNaming
		// ReSharper disable UnassignedField.Global
		// ReSharper disable FieldCanBeMadeReadOnly.Global
		// ReSharper disable MemberCanBePrivate.Global
		public static Type TYPE_TO_MONITOR; // = typeof(ImageButtonSkin);
		// ReSharper restore MemberCanBePrivate.Global
		// ReSharper restore FieldCanBeMadeReadOnly.Global
		// ReSharper restore UnassignedField.Global
		// ReSharper restore InconsistentNaming
		// ReSharper restore UnusedMember.Global
#endif
		/**
		 *  
		 *  Param: object - the IStyleClient to be introspected  
		 *  Param: qualified - whether qualified type names should be used
		 *  Returns: an ordered map of class names, starting with the object's class
		 *  name and then each super class name until we hit a stop class, such as
		 *  mx.core::Component.
		 */

	    internal static OrderedObject<bool> GetTypeHierarchy(Type type/*, bool qualified*/)
		{
			StyleManager styleManager = StyleManager.Instance;
			//Type type = client.GetType();
			string className = type.FullName;
			OrderedObject<bool> hierarchy = null;

			if (styleManager.TypeHierarchyCache.ContainsKey(className))
				hierarchy = styleManager.TypeHierarchyCache[className];

			if (hierarchy == null)
			{
				hierarchy = new OrderedObject<bool>();

				styleManager.TypeHierarchyCache[className] = hierarchy;
				while (!IsStopClass(type))
				{
					try
					{
						if (null != type)
						{
							hierarchy.Add(className, true);
							type = type.BaseType;
							if (null != type)
								className = type.FullName;
						}
					}
					catch (Exception ex)
					{
						className = null;
					}

					//try
					//{
					//    //var type:String;
					//    //if (qualified)
					//    //    type = className.replace("::", ".");
					//    //else
					//        type = NameUtil.getUnqualifiedClassName(className);

					//    hierarchy.object_proxy::setObjectProperty(type, true);
					//    className = getQualifiedSuperclassName(
					//        myApplicationDomain.getDefinition(className));
					//}
					//catch(e:ReferenceError)
					//{
					//    className = null;
					//}
				}

				#region Monitor

#if DEBUG
				if (null != TYPE_TO_MONITOR)
				{
					if (type == TYPE_TO_MONITOR)
						Debug.Log(string.Format(@"### {0} type hierarchy ###
{1}", TYPE_TO_MONITOR, hierarchy));
				}
#endif

				#endregion

			}

			return hierarchy;
		}

		/**
		 *  
		 *  Our style type hierarhcy stops at Component, UITextField or
		 *  GraphicElement, not Object.
		 */
		private static bool IsStopClass(Type value)
		{
			return null == value || typeof(InteractiveComponent) == value;
		}
	}
}
