using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Data.eDriven.Gui.Data.Collections;

namespace eDriven.Gui.Data
{
	/// <summary>
	/// Sort
	/// </summary>
	public class Sort : EventDispatcher
	{
		/*--------------------------------------------------------------------------
		//
		//  Class constants
		//
		//--------------------------------------------------------------------------*/

		public const string ANY_INDEX_MODE = "any";

		public const string FIRST_INDEX_MODE = "first";

		public const string LAST_INDEX_MODE = "last";

		/**
		 *  
		 */
		private bool usingCustomCompareFunction;

		private SortCompareFunction _compareFunction;

		/// <summary>
		/// Compare function
		/// </summary>
		public SortCompareFunction CompareFunction {
			get { return usingCustomCompareFunction ? _compareFunction : InternalCompare; }
			set
			{
				_compareFunction = value;
				usingCustomCompareFunction = _compareFunction != null;
			}
		}

		/**
		 *  
		 *  Storage for the fields property.
		 */
		private List<SortField> _fields;

		/**
		 *  
		 */
		private List<string> fieldList = new List<string>();

		public List<SortField> Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				_fields = value;
				fieldList = new List<string>();
				if (null != _fields)
				{
					for (var i = 0; i<_fields.Count; i++)
					{
						SortField field = _fields[i];
						fieldList.Add(field.Name);
					}
				}
				DispatchEvent(new Event("fieldsChanged"));
			}
		}



		/**
		 *  
		 *  Compares the values specified based on the sort field options specified
		 *  for this sort.  The fields parameter is really just used to get the
		 *  number of fields to check.  We don't look at the actual values
		 *  to see if they match the actual sort.
		 */
		private int InternalCompare(object a, object b, List<SortField> fields = null)
		{
			var result = 0;
			if (null != _fields)
			{
				result = NoFieldsCompare(a, b);
			}
			else
			{
				var i = 0;
				var len = null != fields ? fields.Count : _fields.Count;
				while (result == 0 && (i < len))
				{
					var sf = (SortField)_fields[i];
					result = sf.InternalCompare(a, b);
					i++;
				}
			}

			return result;
		}

		private SortField defaultEmptyField;
		private bool noFieldsDescending = false;

		private int NoFieldsCompare(object a, object b, List<SortField>fields = null)
		{
			if (null == defaultEmptyField)
			{
				defaultEmptyField = new SortField();
				try
				{
					defaultEmptyField.InitCompare(a);
				}
				catch(Exception ex)
				{
					throw new Exception("Sort error", ex);
				}
			}

			int result = defaultEmptyField.CompareFunction(a, b);

			if (noFieldsDescending)
			{
				result *= -1;
			}

			return result;
		}

		/**
		 *  
		 *  Storage for the unique property.
		 */
		private bool _unique;
		public bool Unique
		{
			get
			{
				return _unique;
			}
			set
			{
				_unique = value;
			}
		}
		
		public void DoSort(List<object> items)
		{
			if (null == items || items.Count <= 1)
			{
				return;
			}

			/*if (usingCustomCompareFunction)
			{
				// bug 185872
				// the Sort.internalCompare function knows to use Sort._fields; that same logic
				// needs to be part of calling a custom compareFunction. Of course, a user shouldn't
				// be doing this -- so I wrap calls to compareFunction with _fields as the last parameter
				var fixedCompareFunction = CompareFunction;
					/*delegate (object a, object b)
					{
						// append our fields to the call, since items.sort() won't
						return CompareFunction(a, b, _fields);
					};#1#
				
				if (Unique)
				{
					object uniqueRet1 = items.Sort(fixedCompareFunction); //, Array.UNIQUESORT);
					if (uniqueRet1 is int && (int)uniqueRet1 == 0)
					{
						throw new Exception("nonUnique");
					}
				}
				else
				{
					items.Sort(fixedCompareFunction);
				}
			}
			else
			{
				var fields = Fields;
				if (fields && fields.Count > 0)
				{
					int i;
					//doing the init value each time may be a little inefficient
					//but allows for the data to change and the comparators
					//to update correctly
					//the sortArgs is an object that if non-null means
					//we can use Array.sortOn which will be much faster
					//than going through internalCompare.  However
					//if the Sort is supposed to be unique and fields.length > 1
					//we cannot use sortOn since it only tests uniqueness
					//on the first field
					object sortArgs = initSortFields(items[0], true);

					if (Unique)
					{
						object uniqueRet2;
						if (null != sortArgs && fields.Count == 1)
						{
							uniqueRet2 = items.sortOn(sortArgs.fields[0], sortArgs.options[0] | Array.UNIQUESORT);
						}
						else
						{
							uniqueRet2 = items.sort(internalCompare,
													Array.UNIQUESORT);
						}
						if (uniqueRet2 == 0)
						{
							message = resourceManager.getString(
								"collections", "nonUnique");
							throw new SortError(message);
						}
					}
					else
					{
						if (sortArgs)
						{
							items.sortOn(sortArgs.fields, sortArgs.options);
						}
						else
						{
							items.sort(internalCompare);
						}
					}
				}
				else
				{
					items.Sort(internalCompare);
				}
			}*/
		}
	}
}