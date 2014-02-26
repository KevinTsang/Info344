using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
// using HtmlAgilityPack;
using Crawler;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Data.SqlClient;
using System.Web.Script.Services;
using Entities;

namespace Dashboard
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class Admin : System.Web.Services.WebService
    {
        private static string status = "Loading";

        [WebMethod]
        public void ClearIndex()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.CreateIfNotExists();
            commandQueue.AddMessage(new CloudQueueMessage("stop"));
            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.Clear();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("crawler");
            table.DeleteIfExists();
            CloudTable errors = tableClient.GetTableReference("errors");
            errors.DeleteIfExists();
            using (SqlConnection connection = new SqlConnection("Data Source=klc7dpzke8.database.windows.net;Initial Catalog=WebCrawlerData;User ID=KevinTsang;Password=1-WingedAngel"))
            {
                try
                {
                    SqlCommand deleteFromLastTenURLs = new SqlCommand("DELETE * FROM \"URLs\" WHERE 1=1");
                    deleteFromLastTenURLs.Connection.Open();
                    deleteFromLastTenURLs.ExecuteNonQuery();
                    deleteFromLastTenURLs.Connection.Close();

                    SqlCommand statistics = new SqlCommand("DELETE * FROM \"Statistics\" WHERE 1=1");
                    statistics.Connection.Open();
                    statistics.ExecuteNonQuery();
                    statistics.Connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            status = "Idle";
        }

        [WebMethod]
        public void StartCrawl(string baseURL)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=info344blobstorage;AccountKey=zMaOQy6KX5uDrO11LDigfFWosUgs/Tg5btN7aLBjekPlrG4gE6bd0vKJIRFrIeJ/jXhzypmF2McaNsppQ6MGIQ==");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.CreateIfNotExists();
            commandQueue.CreateIfNotExists();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("crawler");
            table.CreateIfNotExists();
            CloudTable errors = tableClient.GetTableReference("errors");
            errors.CreateIfNotExists();
            CloudTable stats = tableClient.GetTableReference("stats");
            stats.CreateIfNotExists();

            WebClient client = new WebClient();
            string robotString = client.DownloadString(baseURL + "/robots.txt");
            string[] lines = robotString.Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("Sitemap: "))
                    queue.AddMessage(new CloudQueueMessage(line.Substring(9)));
                else if (line.StartsWith("Disallow: "))
                    commandQueue.AddMessage(new CloudQueueMessage(line));
            }
            using (SqlConnection connection = new SqlConnection("Data Source=klc7dpzke8.database.windows.net;Initial Catalog=WebCrawlerData;User ID=KevinTsang;Password=1-WingedAngel"))
            {
                try
                {
                    SqlCommand statistics = new SqlCommand("INSERT INTO \"Statistics\" VALUES (1, 0, 0, 0, 0)");
                    statistics.Connection.Open();
                    statistics.ExecuteNonQuery();
                    statistics.Connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            commandQueue.AddMessage(new CloudQueueMessage("start"));
            status = "Crawling";
        }

        [WebMethod]
        public void StopCrawl()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.CreateIfNotExists();
            commandQueue.AddMessage(new CloudQueueMessage("stop"));
        }

        [WebMethod]
        public string GetPageTitle(string url)
        {
            // Retrieves data from Azure Tables (gets page title for specific URL)
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("crawler");
            table.CreateIfNotExists();
            TableOperation pageTitle = TableOperation.Retrieve<URLWrapper>(HttpUtility.UrlEncode(url), HttpUtility.UrlEncode(url));
            TableResult result = table.Execute(pageTitle);
            if (result.Result != null)
                return ((URLWrapper)result.Result).Title;
            else return "No title exists for this URL.";
        }

        [WebMethod]
        public int QueueSize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.FetchAttributes();
            return (int)queue.ApproximateMessageCount;
        }

        [WebMethod]
        public List<string> GetStatistics()
        {
            List<string> sqldata = new List<string>();
            using (SqlConnection connection = new SqlConnection("Data Source=klc7dpzke8.database.windows.net;Initial Catalog=WebCrawlerData;User ID=KevinTsang;Password=1-WingedAngel"))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM \"Statistics\"", connection);
                command.Connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            sqldata.Add(reader[i].ToString());
                        }
                    }
                }
                command.Connection.Close();
            }
            return sqldata;
        }

        [WebMethod]
        public string GetStatus()
        {
            return status;
        }

        [WebMethod]
        public List<string> Errors()
        {
            List<string> errors = new List<string>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("errors");
            table.CreateIfNotExists();
            TableQuery<ErrorEntity> query = new TableQuery<ErrorEntity>()
            .Where(TableQuery.GenerateFilterCondition("Error", QueryComparisons.NotEqual, ""));
            foreach (var entity in table.ExecuteQuery(query))
            {
                errors.Add(entity.Error + ": " + entity.Url);
            }
            return errors;
        }

        [WebMethod]
        public List<string> GetLastTenURLs()
        {
            List<string> sqldata = new List<string>();
            using (SqlConnection connection = new SqlConnection("Data Source=klc7dpzke8.database.windows.net;Initial Catalog=WebCrawlerData;User ID=KevinTsang;Password=1-WingedAngel"))
            {
                SqlCommand command = new SqlCommand("SELECT TOP 10 * FROM \"URLs\" ORDER BY Timestamp DESC", connection);
                command.Connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sqldata.Add(reader["URL"].ToString());
                    }
                }
                command.Connection.Close();
            }
            return sqldata;
        }
    }
 
}
