using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.WindowsAzure;

namespace Entities
{
    public class URLWrapper : TableEntity
    {
        public URLWrapper()
        {

        }

        public URLWrapper(string url, string title, string lastModified)
        {
            this.PartitionKey = HttpUtility.UrlEncode(url);
            this.RowKey = HttpUtility.UrlEncode(url);

            this.Url = url;
            this.Title = title;
            this.LastModified = DateTime.Parse(lastModified);
        }

        public string Url { get; set; }

        public string Title { get; set; }

        public DateTime LastModified { get; set; }
    }
}
