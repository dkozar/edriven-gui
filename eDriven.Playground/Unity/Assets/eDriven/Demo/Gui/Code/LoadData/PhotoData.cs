using UnityEngine;

namespace Assets.eDriven.Demo.Gui.Code.LoadData
{
    internal class PhotoData
    {
        public string Title;
        public Texture Thumbnail;

        // the URL of the actual (big) image, loaded on request
        public string ImageUrl;

        public PhotoData()
        {
        }

        public PhotoData(string title)
        {
            Title = title;
        }
    }
}