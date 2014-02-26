using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
// using HtmlAgilityPack;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text.RegularExpressions;
using Entities;
using System.Data.SqlClient;

namespace Crawler
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // #1 reads URL from queue
            // #2 crawls website
            // #3 if it finds any new URLs, add to queue
            // #4 add page title + url to table storage

            #region Object Initialization

            Trace.TraceInformation("Worker role initialization", "Information");
            PerformanceCounter memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //string connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            //CloudStorageAccount performanceCounterAccount = CloudStorageAccount.Parse(connectionString);
            //CloudTableClient performanceCounterTableClient = performanceCounterAccount.CreateCloudTableClient();
            //CloudTable performanceTable = performanceCounterTableClient.GetTableReference("WADPerformanceCountersTable");
            //performanceTable.CreateIfNotExists();


            WebClient client = new WebClient();
            HashSet<string> unique = new HashSet<string>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=info344blobstorage;AccountKey=zMaOQy6KX5uDrO11LDigfFWosUgs/Tg5btN7aLBjekPlrG4gE6bd0vKJIRFrIeJ/jXhzypmF2McaNsppQ6MGIQ==");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            
            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.CreateIfNotExists();
            
            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.CreateIfNotExists();

            CloudTable table = tableClient.GetTableReference("crawler");
            table.CreateIfNotExists();

            CloudTable errors = tableClient.GetTableReference("errors");
            errors.CreateIfNotExists();

            CloudTable stats = tableClient.GetTableReference("stats");
            stats.CreateIfNotExists();

            List<string> rules = new List<string>();

            #endregion

            #region Start loop

            while (true)
            {
                Thread.Sleep(1000);
                commandQueue.FetchAttributes();
                if (commandQueue.ApproximateMessageCount != 0)
                {
                    CloudQueueMessage commandMessage = commandQueue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                    if (commandMessage.AsString.StartsWith("Disallow: "))
                    {
                        rules.Add(commandMessage.AsString.Substring(10));
                        commandQueue.DeleteMessage(commandMessage);
                    }
                    else if (commandMessage.AsString.Equals("start"))
                    {
                        commandQueue.DeleteMessage(commandMessage);
                        break;
                    }
                }
            }

            #endregion

            while (true)
            {
                float ram1 = memoryCounter.NextValue();
                float processor1 = cpuCounter.NextValue();
                Thread.Sleep(100);

                #region Check CommandQueue

                commandQueue.FetchAttributes();
                if (commandQueue.ApproximateMessageCount != 0)
                {

                    CloudQueueMessage commandMessage = commandQueue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                    if (commandMessage.AsString.Equals("stop"))
                    {
                        commandQueue.DeleteMessage(commandMessage);
                        while (true)
                        {
                            Thread.Sleep(1000);
                            commandQueue.FetchAttributes();
                            if (commandQueue.ApproximateMessageCount != 0)
                            {
                                commandMessage = commandQueue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                                if (commandMessage.AsString.Equals("start"))
                                {
                                    commandQueue.DeleteMessage(commandMessage);
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                else
                {
                    CloudQueueMessage message = queue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                    string messageString = message.AsString;
                    try
                    {
                        string webpage = client.DownloadString(messageString);
                        List<string> parser = new List<string>(webpage.Split('\n'));

                        string url = message.AsString;
                        string lastModified = " ";
                        string title = " ";

                        foreach (var line in parser)
                        {

                            if (line.Contains("<title>") && line.Contains("</title>"))
                                title = line.Substring(7, line.Length - 15);
                            else if (line.Contains("meta") && line.Contains("last-modified"))
                            {
                                string pattern = @"[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}Z";
                                Match m = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
                                string date = m.ToString();
                                lastModified = date;
                            }
                            else
                            {
                                string pattern = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@\/$,]*";
                                Match m = Regex.Match(line, pattern,
                                  RegexOptions.IgnoreCase);
                                string link = m.ToString();

                                bool isValidURL = true;
                                foreach (string str in rules)
                                { 
                                    if (link.Contains(str))
                                    {
                                        isValidURL = false;
                                        break;
                                    }
                                }

                                if (!link.Equals("") && link.Contains("cnn.com/") && isValidURL)
                                {
                                    int test = link.IndexOf('>');
                                    if (test != -1)
                                    {
                                        link = link.Substring(0, test);
                                    }
                                    if (!unique.Contains(link) && !link.Equals(url))
                                    {
                                        unique.Add(link);
                                        queue.AddMessage(new CloudQueueMessage(link));
                                    }
                                }

                            }


                        }
                        queue.DeleteMessage(message);
                        /*HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                        document.Load(messageString);
                        HtmlNode title = document.DocumentNode.SelectSingleNode("title");
                        HtmlNode date = document.DocumentNode.SelectSingleNode("//meta[@http-equiv='last-modified']");
                        foreach (HtmlNode node in document.DocumentNode.SelectNodes("//a[@href] or //@src or //loc"))
                        {
                            if (node.Name.Equals("loc"))
                            {
                                AddToQueueIfValid(queue, rules, node.InnerText, rootDomain);
                            }
                            else
                            {
                                HtmlAttribute link = node.Attributes["href"];
                                HtmlAttribute source = node.Attributes["src"];
                                if (link != null)
                                    AddToQueueIfValid(queue, rules, link.Value, rootDomain);
                                else if (source != null)
                                    AddToQueueIfValid(queue, rules, source.Value, rootDomain);
                            }
                        }*/

                        if (!messageString.EndsWith(".xml"))
                        {
                            // URLWrapper entry = new URLWrapper(messageString, title.InnerHtml, date.Attributes["content"].Value);
                            float ram2 = memoryCounter.NextValue();
                            float processor2 = cpuCounter.NextValue();
                            URLWrapper entry = new URLWrapper(messageString, title, lastModified);
                            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
                            table.Execute(insertOperation);
                            using (SqlConnection connection = new SqlConnection("Data Source=klc7dpzke8.database.windows.net;Initial Catalog=WebCrawlerData;User ID=KevinTsang;Password=1-WingedAngel"))
                            {
                                //try
                                //{
                                    SqlCommand insert = new SqlCommand("INSERT INTO \"URLs\" VALUES (" + messageString + ", " + DateTime.Now + ")", connection);
                                    insert.Connection.Open();
                                    insert.ExecuteNonQuery();
                                    insert.Connection.Close();
                                    SqlCommand tableSize = new SqlCommand("SELECT TableSize FROM \"Statistics\" WHERE ID=1", connection);
                                    tableSize.Connection.Open();
                                    int size = (int)tableSize.ExecuteScalar();
                                    tableSize.Connection.Close();
                                    SqlCommand urlscrawled = new SqlCommand("SELECT URLsCrawled FROM \"Statistics\" WHERE ID=1", connection);
                                    urlscrawled.Connection.Open();
                                    int urls = (int)urlscrawled.ExecuteScalar();
                                    urlscrawled.Connection.Close();
                                    SqlCommand update = new SqlCommand("UPDATE \"Statistics\" SET RAMAvailable=" + (ram1 + ram2)/2 + ", CPUPercentage=" + (processor1+processor2)/2 + ", URLsCrawled=" + urls + ", TableSize=" + size + "WHERE ID=1", connection);
                                    update.Connection.Open();
                                    update.ExecuteNonQuery();
                                    update.Connection.Close();
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                        }
                        else
                        {
                            float ram2 = memoryCounter.NextValue();
                            float processor2 = cpuCounter.NextValue();
                            using (SqlConnection connection = new SqlConnection("Data Source=klc7dpzke8.database.windows.net;Initial Catalog=WebCrawlerData;User ID=KevinTsang;Password=1-WingedAngel"))
                            {
                                //try
                                //{
                                    SqlCommand command = new SqlCommand("SELECT URLsCrawled FROM \"Statistics\" WHERE ID=1", connection);
                                    command.Connection.Open();
                                    int urls = (int)command.ExecuteScalar();
                                    command.Connection.Close();
                                    SqlCommand update = new SqlCommand("UPDATE \"Statistics\" SET RAMAvailable=" + (ram1 + ram2)/2 + ", CPUPercentage=" + (processor1+processor2)/2 + ", URLsCrawled=" + urls + "WHERE ID=1", connection);
                                    update.Connection.Open();
                                    update.ExecuteNonQuery();
                                    update.Connection.Close();
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        string error = e.Message;
                        ErrorEntity errorentry = new ErrorEntity(messageString, error);
                        TableOperation insertOperation = TableOperation.Replace(errorentry);
                        queue.DeleteMessage(message);
                    }
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
