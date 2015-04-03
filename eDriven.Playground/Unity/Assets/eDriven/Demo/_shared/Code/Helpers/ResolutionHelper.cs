using System.Collections.Generic;
using eDriven.Core.Data.Collections;
using UnityEngine;

namespace Assets.eDriven.Demo.Helpers
{
    public static class ResolutionHelper
    {
        private static List<object> _resolutionList;

        public static List<object> GetResolutionList()
        {
            if (null == _resolutionList) // lazy
            {
                Resolution[] resolutions = Screen.resolutions;

                _resolutionList = new List<object>();

                int count = resolutions.Length;

                for (int i = count - 1; i >= 0; i--) // bigger on top
                {
                    Resolution resolution = resolutions[i];
                    ListItem li = new ListItem(
                        new ResolutionDescriptor(i, resolution), 
                        string.Format("{0}x{1}", resolution.width, resolution.height)
                    );
                    _resolutionList.Add(li);
                }
            }

            return _resolutionList;
        }

        /// <summary>
        /// Returns dummy resolution list (when in editor)
        /// </summary>
        /// <returns></returns>
        public static List<object> GetDummyResolutionList()
        {
            if (null == _resolutionList) // lazy
            {
                List<ResolutionDescriptor> resolutions = new List<ResolutionDescriptor>
                                                   {
                                                       new ResolutionDescriptor(0, new Resolution {width = 100, height = 100}),
                                                       new ResolutionDescriptor(1, new Resolution {width = 200, height = 200}),
                                                       new ResolutionDescriptor(2, new Resolution {width = 300, height = 300}),
                                                       new ResolutionDescriptor(3, new Resolution {width = 400, height = 400}),
                                                       new ResolutionDescriptor(4, new Resolution {width = 500, height = 500}),
                                                   };

                _resolutionList = new List<object>();

                foreach (ResolutionDescriptor resolution in resolutions)
                {
                    ListItem li = new ListItem(resolution, string.Format("{0}x{1}", resolution.Resolution.width, resolution.Resolution.height));
                    _resolutionList.Add(li);
                }
            }

            return _resolutionList;
        }
    }

    public class ResolutionDescriptor
    {
        public int Id;
        public Resolution Resolution;

        public ResolutionDescriptor(int id, Resolution resolution)
        {
            Id = id;
            Resolution = resolution;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Resolution: {1}, {2}", Id, Resolution.width, Resolution.height);
        }

        #region Equals

        public bool Equals(ResolutionDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ResolutionDescriptor)) return false;
            return Equals((ResolutionDescriptor) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion

    }
}