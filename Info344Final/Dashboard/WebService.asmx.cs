using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using WrapperEntities;

namespace Dashboard
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class WebService : System.Web.Services.WebService
    {

        public static Trie trie = new Trie();
        public static string status = "crawling";
        public static Dictionary<string, List<string>> cache = new Dictionary<string, List<string>>();
        public static int titlesprocessed = 0;
        public static string lastTitleInserted = String.Empty;
        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ClearIndex()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.AddMessage(new CloudQueueMessage("stop"));

            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.Clear();

            CloudQueue size = queueClient.GetQueueReference("size");
            size.Clear();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("crawler");
            table.DeleteIfExists();

            status = "idle";
            List<string> success = new List<string>();
            string s = "All worker roles are now idle.  Successfully cleared the queue/table.";
            success.Add(s);

            return new JavaScriptSerializer().Serialize(success);
        }

        [WebMethod]
        public void StartCrawl()
        {
            status = "crawling";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.CreateIfNotExists();
            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.CreateIfNotExists();


            WebClient client = new WebClient();
            string robotText = client.DownloadString("http://www.cnn.com/robots.txt");
            string[] lines = robotText.Split('\n');


            foreach (string line in lines)
            {
                if (line.Contains("Sitemap:"))
                {
                    queue.AddMessage(new CloudQueueMessage(line.Substring(9))); 
                    Thread.Sleep(100);
                }
                else if (line.Contains("Disallow:"))
                {
                    commandQueue.AddMessage(new CloudQueueMessage(line));
                }

            }
            commandQueue.AddMessage(new CloudQueueMessage("start"));
        }

        [WebMethod]
        public void StopCrawl()
        {
            status = "idle";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.CreateIfNotExists();
            commandQueue.AddMessage(new CloudQueueMessage("stop"));
        }

        [WebMethod]
        public List<string> getUrlFromTitle(string titlePart)
        {
            if (cache.Keys.Contains(titlePart))
                return cache[titlePart];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("urls");
            table.CreateIfNotExists();
            string[] parts = titlePart.Split(' ');
            List<string> urls = new List<string>();

            foreach (string s in parts)
            {
                TableQuery<URLWrapper> query = new TableQuery<URLWrapper>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, s));

                foreach (URLWrapper entity in table.ExecuteQuery(query))
                {
                    if (!urls.Contains(entity.Url))
                    {
                        urls.Add(entity.Title + ": " + entity.Url);
                    }
                }
            }

            if (urls.Count > 0)
            {
                if (cache.Count == 100)
                    cache.Remove(cache.ElementAt(0).Key);
                if (!cache.Keys.Contains(titlePart))
                    cache.Add(titlePart, urls);
                else cache[titlePart].AddRange(urls);
                return urls;
            }
            else
            {
                urls.Add("Nothing found");
                return urls;
            }
        }

        [WebMethod]
        public string GetStatus()
        {
            Thread.Sleep(500);
            return status;
        }

        [WebMethod]
        public int? QueueSize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.CreateIfNotExists();
            queue.FetchAttributes();
            return queue.ApproximateMessageCount;
        }

        [WebMethod]
        public int? NumberCrawled()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue urlscrawled = queueClient.GetQueueReference("urlscrawled");
            urlscrawled.CreateIfNotExists();
            urlscrawled.FetchAttributes();
            int? length = urlscrawled.ApproximateMessageCount;

            if (QueueSize() == 0)
                length = 0;
            return length;
        }

        [WebMethod]
        public int? IndexSize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("size");
            queue.CreateIfNotExists();
            queue.FetchAttributes();
            int? size = queue.ApproximateMessageCount;

            if (QueueSize() == 0)
                size = 0;
            
            return size;
        }

        [WebMethod]
        public List<string> Errors()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("errors");
            table.CreateIfNotExists();

            TableQuery<URLWrapper> rangeQuery = new TableQuery<URLWrapper>()
            .Where(TableQuery.GenerateFilterCondition("LastModified", QueryComparisons.Equal, ""));
            List<string> errors = new List<string>();

            foreach (URLWrapper entity in table.ExecuteQuery(rangeQuery))
            {
                errors.Add(entity.Url);
            }
            return errors;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetLastTenURLs()
        {
            List<string> urls = new List<string>();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("lastten");
            table.CreateIfNotExists();

            TableQuery<URLWrapper> rangeQuery = new TableQuery<URLWrapper>()
            .Where(TableQuery.GenerateFilterCondition("Url", QueryComparisons.NotEqual, ""));

            foreach (URLWrapper entity in table.ExecuteQuery(rangeQuery))
            {
                urls.Add(entity.Title + ": " + entity.Url);
            }
            return new JavaScriptSerializer().Serialize(urls);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRam()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("stats");
            table.CreateIfNotExists();

            TableOperation tablequery = TableOperation.Retrieve<Performance>("0", "0");
            TableResult retrieved = table.Execute(tablequery);

            return new JavaScriptSerializer().Serialize(((Performance)retrieved.Result).Ram);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCPU()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("stats");
            table.CreateIfNotExists();

            TableOperation tablequery = TableOperation.Retrieve<Performance>("0", "0");
            TableResult retrieved = table.Execute(tablequery);

            return new JavaScriptSerializer().Serialize(((Performance)retrieved.Result).CPU);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSuggestions(string word)
        {
            List<string> queries = null;
            queries = trie.SearchPhrasesForPrefix(word);
            return new JavaScriptSerializer().Serialize(queries);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCache(string word)
        {
            List<string> cachedresults = new List<string>();
            foreach (string saved in cache.Keys)
            {
                if (saved.StartsWith(word))
                {
                    cachedresults.Add(saved);
                }
            }
            return new JavaScriptSerializer().Serialize(cachedresults);
        }

        [WebMethod]
        public string getBlob()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("info344db");

            if (container.Exists())
            {
                string file = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\processedfile.txt";
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        using (var fileStream = new FileStream(file, FileMode.OpenOrCreate))
                        {
                            blob.DownloadToStream(fileStream);
                        }

                    }
                }

                return file;
            }

            return "Error";
        }



        [WebMethod]
        public void createTrie()
        {
            trie = new Trie();
            string file = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\processedfile.txt";
            int cutOff = 0;
            FileStream fs = File.OpenRead(file);
            BufferedStream buffer = new BufferedStream(fs);
            using (StreamReader sr = new StreamReader(buffer))
            {
                while ((lastTitleInserted = sr.ReadLine()) != null && cutOff < 100000)
                {
                    trie.InsertIntoTrie(lastTitleInserted);
                    titlesprocessed++;
                    cutOff++;
                }
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string titleInserted()
        {
            return new JavaScriptSerializer().Serialize(lastTitleInserted);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string trieCount()
        {
            return new JavaScriptSerializer().Serialize(titlesprocessed);
        }
    }
}
