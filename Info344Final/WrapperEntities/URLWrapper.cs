using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WrapperEntities
{
    public class URLWrapper : TableEntity
    {
        public URLWrapper()
        {

        }

        public URLWrapper(string url, string titlePart, string lastModified, string title)
        {
            this.PartitionKey = titlePart;
            this.RowKey = HttpUtility.UrlEncode(url);
            this.Title = title;
            this.Url = url;
            this.Timestamps = DateTime.Now;
            if (!String.IsNullOrEmpty(lastModified))
                this.LastModified = DateTime.Parse(lastModified);
        }

        public string Title { get; set; }

        public string Url { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime Timestamps { get; set; }
    }
}
