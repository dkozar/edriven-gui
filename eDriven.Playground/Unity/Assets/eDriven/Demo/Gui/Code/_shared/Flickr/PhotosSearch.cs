using System.Collections.Generic;

namespace Assets.eDriven.Demo.Gui.Code.Flickr
{
    public class PhotosSearchResponse
    {
        [JsonFx.Json.JsonName("stat")]
        public string Stat;

        [JsonFx.Json.JsonName("code")]
        public int Code;

        [JsonFx.Json.JsonName("message")]
        public string Message;

        [JsonFx.Json.JsonName("photos")]
        public Photos Photos;
    }

    public class Photos
    {
        [JsonFx.Json.JsonName("page")]
        public int Page;

        [JsonFx.Json.JsonName("pages")]
        public int Pages;

        [JsonFx.Json.JsonName("perpage")]
        public int Perpage;

        [JsonFx.Json.JsonName("total")]
        public int Total;

        [JsonFx.Json.JsonName("photo")]
        public List<Photo> Photo;
    }

    public class Photo
    {
        [JsonFx.Json.JsonName("id")]
        public string Id;

        [JsonFx.Json.JsonName("owner")]
        public string Owner;

        [JsonFx.Json.JsonName("secret")]
        public string Secret;

        [JsonFx.Json.JsonName("server")]
        public string Server;

        [JsonFx.Json.JsonName("farm")]
        public string Farm;

        [JsonFx.Json.JsonName("title")]
        public string Title;

        [JsonFx.Json.JsonName("ispublic")]
        public int IsPublic;

        [JsonFx.Json.JsonName("isfriend")]
        public int IsFriend;

        [JsonFx.Json.JsonName("isfamily")]
        public int IsFamily;

        /// <summary>
        /// 
        /// </summary>
        /// <see>http://www.flickr.com/services/api/misc.urls.html</see>
        public string GetUrl(string size)
        {
            //http://farm{farm-id}.staticflickr.com/{server-id}/{id}_{o-secret}_o.(jpg|gif|png)
            return string.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}_{4}.jpg", Farm, Server, Id, Secret, size);
        }
    }
}