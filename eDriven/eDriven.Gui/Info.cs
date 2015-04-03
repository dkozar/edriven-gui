/*
 
eDriven Gui
Copyright (c) 2010-2014 Danko Kozar
 
*/

namespace eDriven.Gui
{
    /// <summary>
    /// The info class
    /// </summary>
    public sealed class Info
    {
#if !RELEASE
        public const string AssemblyName = "eDriven.Gui";
#endif

#if RELEASE
        public const string AssemblyName = "eDriven.Gui Free Edition";
#endif
        public const string AssemblyVersion = "2.4.0";
        public const string Author = "Danko Kozar";
        public const string Copyright = "Copyright (c) Danko Kozar 2010-2014. All rights reserved.";
        public const string Web = "edriven.dankokozar.com";
#if RELEASE
        public const string Note = "Use of this assembly must be limited to evaluation or educational purposes only, and it is not to be used for commercial purposes.";
#endif
        public override string ToString()
        {
            return string.Format(@"[{0} {1}]
[by {2}, (C) {3}, {4}]", AssemblyName, AssemblyVersion, Author, Copyright, Web);
        }

//        internal const string Name = "eDriven.Gui";
//        internal const string Version = "1.0.0";

//        /// <summary>
//        /// The author of eDriven framework
//        /// </summary>
//        public static string Author
//        {
//            get { return "Danko Kozar"; }
//        }

//        /// <summary>
//        /// Copyright
//        /// </summary>
//        public static string Copyright
//        {
//            get { return "2012 by Danko Kozar"; }
//        }

//        /// <summary>
//        /// eDriven web site
//        /// </summary>
//        public static string Web
//        {
//            get { return "edriven.dankokozar.com"; }
//        }

//        //Note: Assembly.GetExecutingAssembly().GetName().Name; caused a lot of problems with silent fails in web player!!!

//        /// <summary>
//        /// Assembly name
//        /// </summary>
//        public static string AssemblyName
//        {
//            get
//            {
//                return Name;
//                //try
//                //{
//                //    Assembly.GetExecutingAssembly().GetName().Name;
//                //}
//                ////catch (MethodAccessException)
//                //catch (Exception)
//                //{
//                //    return "eDriven.Core";
//                //}
//            }
//        }

//        /// <summary>
//        /// Assembly version
//        /// </summary>
//        public static string AssemblyVersion
//        {
//            get
//            {
//                return Version;
//                //try
//                //{
//                //    return "#Unknown Version#"; //Assembly.GetExecutingAssembly().GetName().Version.ToString();
//                //}
//                //catch (Exception)
//                //{
//                //    return "#Unknown Version#";
//                //}
//            }
//        }

//        public override string ToString()
//        {
//            return string.Format(@"[{0} {1}]
//[by {2}, (C) {3}, {4}]", AssemblyName, AssemblyVersion, Author, Copyright, Web);
//        }
    }
}