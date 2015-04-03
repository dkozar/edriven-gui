using System;
using eDriven.Core.Events;
using eDriven.Core.Reflection;

namespace eDriven.Gui.Data
{
    /// <summary>
	/// Sort field
	/// </summary>
	public class SortField : EventDispatcher
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="caseInsensitive"></param>
		/// <param name="descending"></param>
		/// <param name="numeric"></param>
		public SortField(string name = null, bool caseInsensitive = false, bool descending = false, object numeric = null)
		{
			_name = name;
			_caseInsensitive = caseInsensitive;
			_descending = descending;
			_numeric = numeric;
			_compareFunction = StringCompare;
		}

		/**
		 *  
		 *  Storage for the caseInsensitive property.
		 */
		private bool _caseInsensitive;

		/// <summary>
		/// True for case insensitive sort
		/// </summary>
		public bool CaseInsensitive
		{
			get { return _caseInsensitive; }
			set
			{
				if (value != _caseInsensitive)
				{
					_caseInsensitive = value;
					DispatchEvent(new Event("caseInsensitiveChanged"));
				}
			}
		}
		
		/// <summary>
		/// Data field
		/// Used for default (string) sort
		/// </summary>
		public string DataField;

		/**
		 *  
		 *  Storage for the descending property.
		 */
		private bool _descending;

		/// <summary>
		/// True for descending sort
		/// </summary>
		public bool Descending
		{
			get {
				return _descending;
			}
			set
			{
				if (_descending != value)
				{
					_descending = value;
					DispatchEvent(new Event("descendingChanged"));
				}
			}
		}

		/**
		 *  
		 *  Storage for the name property.
		 */
		private string _name;
		
		/// <summary>
		/// Field name
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				DispatchEvent(new Event("nameChanged"));
			}
		}

		/**
		 *  
		 *  Storage for the numeric property.
		 */
		private object _numeric;
		
		/// <summary>
		/// True for numeric sort
		/// </summary>
		public object Numeric
		{
			get
			{
				return _numeric;
			}
			set
			{
				if (_numeric != value)
				{
					_numeric = value;
					DispatchEvent(new Event("numericChanged"));
				}
			}
		}

		/**
		 *  
		 *  Storage for the compareFunction property.
		 */
		private CompareFunction _compareFunction;

		/// <summary>
		/// Compare function
		/// </summary>
		public CompareFunction CompareFunction
		{
			get { return _compareFunction; }
			set
			{
				_compareFunction = value;
				_usingCustomCompareFunction = (value != null);
			}
		}

		//---------------------------------
		//  usingCustomCompareFunction
		//---------------------------------

		private bool _usingCustomCompareFunction;

		internal bool UsingCustomCompareFunction
		{
			get { return _usingCustomCompareFunction; }
		}

		internal int InternalCompare(object a, object b)
		{
			int result = CompareFunction(a, b);
			if (_descending)
				result *= -1;
			return result;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public SortField(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="caseInsensitive"></param>
		/// <param name="descending"></param>
		/// <param name="numeric"></param>
		public SortField(string name, bool caseInsensitive, bool descending, bool numeric)
		{
			Name = name;
			CaseInsensitive = caseInsensitive;
			Descending = descending;
			Numeric = numeric;
		}

		/// <summary>
		/// Reverses the sort
		/// </summary>
		public void Reverse()
		{
			Descending = !Descending;
		}

		/**
		 *  
		 *  This method allows us to determine what underlying data type we need to
		 *  perform comparisions on and set the appropriate compare method.
		 *  If an option like numeric is set it will take precedence over this aspect.
		 */
		internal void InitCompare(object obj)
		{
			// if the compare function is not already set then we can set it
			if (!UsingCustomCompareFunction)
			{
				if (null != Numeric)
					_compareFunction = NumericCompare;
				else if (_caseInsensitive || (Numeric is bool && (bool)Numeric == false))
					_compareFunction = StringCompare;
				else
				{
					// we need to introspect the data a little bit
					object value = null;
					if (!string.IsNullOrEmpty(_name))
					{
						try
						{
							//value = obj[_name];
							value = CoreReflector.GetValue(obj, _name);
						}
// ReSharper disable once EmptyGeneralCatchClause
						catch(Exception ex)
						{
						}
					}
					//this needs to be an == null check because !value will return true
					//where value == 0 or value == false
					if (value == null)
					{
						value = obj;
					}

					var typ = value.GetType();

					if (typ == typeof(string))
						_compareFunction = StringCompare;
					else if (typ == typeof (object))
					{
						if (value is DateTime)
						{
							_compareFunction = DateCompare;
						}
						else
						{
							_compareFunction = StringCompare;
							string test = null;
							try
							{
								test = value.ToString();
							}
// ReSharper disable once EmptyGeneralCatchClause
							catch(Exception ex)
							{
							}
							if (null == test/* || test == "[object Object]"*/)
							{
								_compareFunction = NullCompare;
							}
						}
					}
					else if (typ == typeof (bool) || typ == typeof (float))
					{
                        _compareFunction = NumericCompare;
					}

				    /*switch (typ)
					{
						case "string":
							_compareFunction = stringCompare;
						break;
						case "object":
							if (value is Date)
							{
								_compareFunction = dateCompare;
							}
							else
							{
								_compareFunction = stringCompare;
								var test:String;
								try
								{
									test = value.toString();
								}
								catch(error2:Error)
								{
								}
								if (!test || test == "[object Object]")
								{
									_compareFunction = nullCompare;
								}
							}
						break;
						case "xml":
							_compareFunction = xmlCompare;
						break;
						case "boolean":
						case "number":
							_compareFunction = numericCompare;
						break;
					}*/
				}  // else
			} // if
		}

		#region Equals

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			// TODO: write your implementation of Equals() here
			SortField sf = (SortField)obj;
			return sf.Name.Equals(Name);
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		public override string ToString()
		{
			return string.Format(@"SortField [Name: ""{0}""; Descending: {1}]", Name, Descending);
		}

		#region Comparisons

		private int NullCompare(object a, object b)
		{
			object value;
			object left = null;
			object right = null;
		
			bool found = false;

			// return 0 (ie equal) if both are null		
			if (a == null && b == null)
			{
				return 0;
			}

			// we need to introspect the data a little bit
			if (null != _name)
			{
				try
				{
					//left = a[_name]; // TODO: reflect
				}
// ReSharper disable once EmptyGeneralCatchClause
				catch(Exception ex)
				{
				}

				try
				{
					//right = b[_name]; // TODO: reflect
				}
// ReSharper disable once EmptyGeneralCatchClause
				catch(Exception ex)
				{
				}
			}

			// return 0 (ie equal) if both are null		
			if (left == null && right == null)
				return 0;

			if (left == null && null == _name)
				left = a;

			if (right == null && null == _name)
				right = b;

		
			/*var typeLeft:String = typeof(left);
			var typeRight:String = typeof(right);*/

			var typeLeft = left.GetType();
			var typeRight = right.GetType();

			if (typeLeft == typeof(string) || typeRight == typeof(string))
			{
					found = true;
					_compareFunction = StringCompare;
			}
			else if (typeLeft == typeof(object) || typeRight == typeof(object))
			{
				if (left is DateTime || right is DateTime)
				{
					found = true;
					_compareFunction = DateCompare;
				}	
			}
			else if (left is float || right is float 
					 || left is bool || right is bool)
			{
					found = true;
					_compareFunction = NumericCompare;
			}
	
			if (found)
			{
				return _compareFunction(left, right);	
			}
			else
			{
				throw new Exception("noComparatorSortField");
			}
		}	

		private int NumericCompare(object a, object b)
		{
			float fa;
			try
			{
				//fa = _name == null ? (float)a : (float)a[_name];
				fa = _name == null ? (float)a : (float)CoreReflector.GetValue(a, _name);
			}
			catch(Exception ex)
			{
			}

			float fb;
			try
			{
				//fb = null == _name ? (float)b : (float)b[_name];
				fb = null == _name ? (float)b : (float)CoreReflector.GetValue(b, _name);
			}
			catch(Exception ex)
			{
			}

			//return ObjectUtil.numericCompare(fa, fb);
			return 0; // TEMP
		}

		private int DateCompare(object a, object b)
		{
			DateTime fa;
			try
			{
				//fa = _name == null ? (a as DateTime) : a[_name] as Date;
                fa = (DateTime)(_name == null ? a : CoreReflector.GetValue(a, _name));
			}
			catch(Exception ex)
			{
			}

			DateTime fb;
			try
			{
                //fb = _name == null ? (b as DateTime?) : b[_name] as Date;
                fb = (DateTime)(_name == null ? b : CoreReflector.GetValue(b, _name));
			}
			catch(Exception ex)
			{
			}

			//return ObjectUtil.dateCompare(fa, fb);
			return 0; //TEMP
		}

		private int StringCompare(object a, object b)
		{
			string fa;
			try
			{
				//fa = _name == null ? (string)a : (string)a[_name];
                fa = (string)(_name == null ? a : CoreReflector.GetValue(a, _name));
			}
			catch(Exception ex)
			{
			}

			string fb;
			try
			{
				//fb = _name == null ? (string)b : (string)b[_name];
                fa = (string)(_name == null ? b : CoreReflector.GetValue(b, _name));
			}
			catch(Exception ex)
			{
			}

			//return ObjectUtil.stringCompare(fa, fb, _caseInsensitive);
			return 0; // TEMP
		}

		#endregion

	}
}