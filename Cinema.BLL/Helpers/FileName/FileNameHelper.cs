using System;
using System.Linq;

namespace Cinema.BLL.Helpers.Url
{
    public static class FileNameHelper
    {
        public static string GetFileNameFromUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Split('?')[0];
                url = url.Split('/').Last();
                string extension = url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";

                return Guid.NewGuid().ToString() + extension;
            }

            return String.Empty;
        }
    }
}
