using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Entities
{
    public class ErrorEntity : TableEntity
    {
        public ErrorEntity()
        {

        }

        public ErrorEntity(string url, string error)
        {
            this.PartitionKey = HttpUtility.UrlEncode(url);
            this.RowKey = Guid.NewGuid().ToString();
            this.Url = url;
            this.Error = error;
        }

        public string Url { get; set; }

        public string Error { get; set; }
    }
}
