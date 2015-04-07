namespace eDriven.Animation
{
    public class Info
    {
#if !TRIAL
        public const string AssemblyName = "eDriven.Animation";
#endif

#if TRIAL
        public const string AssemblyName = "eDriven.Animation Free Edition";
#endif

        public const string AssemblyVersion = "2.4.0";
        public const string Author = "Danko Kozar";
        public const string Copyright = "Copyright (c) Danko Kozar 2010-2014. All rights reserved.";
        public const string Web = "edriven.dankokozar.com";
#if TRIAL
        public const string Note = "Use of this assembly must be limited to evaluation or educational purposes only, and it is not to be used for commercial purposes.";
#endif
        public override string ToString()
        {
            return string.Format(@"[{0} {1}]
[by {2}, (C) {3}, {4}]", AssemblyName, AssemblyVersion, Author, Copyright, Web);
        }
    }
}