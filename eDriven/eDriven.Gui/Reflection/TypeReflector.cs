using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Gui.Util;
using UnityEngine;

#if DEBUG
using System.Text;
#endif

namespace eDriven.Gui.Reflection
{
	public static class TypeReflector
	{
#if DEBUG
		// ReSharper disable UnassignedField.Global
		/// <summary>
		/// Debug mode
		/// </summary>
		public static bool DebugMode;

		// ReSharper restore UnassignedField.Global
#endif

// ReSharper disable once InconsistentNaming
		public const string REFLECTION_FILE_PATH = "Reflection/reflection";

		/// <summary>
		/// The list of full names of assemblies to reflect
		/// </summary>
		private static readonly List<string> DefaultAssemblyList = new List<string>
		{
			/* NOTE: comma included */
			//"mscorlib,", // skipping for perfomance
			//"System,",
			//"System.Core,",
			"UnityEngine,",
			"Assembly-CSharp,",
			"Assembly-UnityScript,",
			"Assembly-Boo,",
			"eDriven.Animation,", // because of the effect styles (TODO: drop it)
			"eDriven.Gui,",
			"eDriven.Gui.Designer,",
			"eDriven.Gui.Editor,"// because of the StyleModuleBase
		};

		private static List<string> _loadedList;

		/// <summary>
		/// The list of full names of assemblies to reflect
		/// </summary>
		public static List<string> AssembliesToReflect
		{
			get
			{
				if (null != _loadedList)
					return _loadedList;
				
				/*string fileLocation = Application.persistentDataPath + "Resources/" + REFLECTION_FILE_PATH;
				
				if (!File.Exists(fileLocation))
				{
					throw new Exception("Cannot load the reflection.txt resource file");
				}*/

				try
				{
					TextAsset file = (TextAsset) Resources.Load(REFLECTION_FILE_PATH);
					if (null == file)
					{
#if DEBUG
						if (DebugMode)
						{
							Debug.Log("Cannot load assembly list file from the location: " + REFLECTION_FILE_PATH);
						}
#endif
						return DefaultAssemblyList;
					}

#if DEBUG
					if (DebugMode)
					{
						Debug.Log(@"File content:
" + file.text);
					}
#endif
					
					_loadedList = new List<string>();

					var parts = file.text.Split(new[] { Environment.NewLine, "\n", "\r", "\r\n" },
	StringSplitOptions.RemoveEmptyEntries);

					foreach (var part in parts)
					{
						_loadedList.Add(part);
					}

					/* or System.StringSplitOptions.None if you want empty results as well */
				}
				catch (Exception ex)
				{
					throw new Exception("Cannot load the reflection.txt resource file", ex);
					//return _defaultAssemblyList;
				}

				return _loadedList;
			}
		}

		private static List<Type> _allTypes;
		/// <summary>
		/// Gets all loaded types from all assemblies
		/// </summary>
		/// <returns></returns>
		public static List<Type> GetAllLoadedTypes() //TODO: Optimize using the type.IsClass (divide into classes, structs etc.)
		{
			/**
			 * 1. Get all types for all loaded assemblies
			 * This is done only when componet tab expanded, so is no performance issue
			 * */

			if (null == _allTypes)
			{
				//Debug.Log("GetAllLoadedTypes playMode: " + Application.isPlaying);

				/* Starting with types we are using from "mscorlib" */
				_allTypes = new List<Type>
				{
					typeof (int), typeof (int?),
					typeof (bool), typeof (bool?),
					typeof (float), typeof (float?),
					typeof (string),
					typeof(Type),
					typeof (Char), typeof (Char?)
					//typeof(Nullable)
				};

#if DEBUG
				var startTime = DateTime.Now;
				StringBuilder sb = new StringBuilder();
#endif

				// NOTE: Calling the preloader feedback from here messes the whole thing (stage never renders, eDriven framework object duplicated..)
				//PreloaderFeedback.Instance.StepStart(PreloaderFeedback.REFLECTING);

				Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				List<string> allAssemblies = new List<string>();
				foreach (var assembly in loadedAssemblies)
				{
					allAssemblies.Add(assembly.FullName);
				}

#if DEBUG
	if (DebugMode)
				{
					Debug.Log(String.Format(@"{0} allAssemblies
{1}", allAssemblies.Count, ListUtil<string>.Format(allAssemblies)));
				}
#endif

				var count = 0;
				foreach (Assembly assembly in loadedAssemblies)
				{
					//Debug.Log(assembly.FullName);
					var fullName = assembly.FullName;
					// ReSharper disable once ConvertClosureToMethodGroup
					var shouldReflect = AssembliesToReflect.Exists(delegate(string namePart)
					{
						return fullName.StartsWith(namePart);
					});

					if (!shouldReflect)
						continue;

#if DEBUG
					var currentTime = DateTime.Now;
					if (DebugMode)
					{
						sb.Append(assembly.FullName);
					}
#endif

					try
					{
						var types = assembly.GetTypes(); // avoid duplication!!!
						foreach (var type in types)
						{
							if (!_allTypes.Contains(type))
							{
								/*#if DEBUG
																if (DebugMode)
																{
																	sb.AppendLine("    " + type.FullName);
																}
								#endif*/
								_allTypes.Add(type);
							}
						}
					}
					catch (ReflectionTypeLoadException)
					{
						//throw new Exception("Cannot reflect assembly: " + assembly.FullName);
						Debug.LogError("Cannot reflect assembly: " + assembly.FullName);
#if DEBUG
	if (DebugMode)
	{
		sb.AppendLine(" -> Error!");
	}
#endif
					}

#if DEBUG
	if (DebugMode)
	{
		sb.AppendLine(String.Format(" ({0} ms)", DateTime.Now.Subtract(currentTime).TotalMilliseconds));
	}
#endif

					count++;
				}
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(String.Format(@"Reflected {0} assemblies ({1} ms)
{2}", count, Math.Round(DateTime.Now.Subtract(startTime).TotalMilliseconds), sb));
				}
#endif
			}

			return _allTypes;
		}
	}
}
