using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.XPath;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using HtmlAgilityPack;
using WrapperEntities;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace WebCrawler
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            PerformanceCounter memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            WebClient client = new WebClient();
            HashSet<string> unique = new HashSet<string>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=info344blobstorage;AccountKey=szmTcQtg4w4CLwDg2FK3Frcq2M+Qy0Y66rYsuAQ0o6MQUI14GrFAvocYXAyglPRYp5b08lNQlD5TSRXgc3gzhA==");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue commandQueue = queueClient.GetQueueReference("commands");
            commandQueue.CreateIfNotExists();

            CloudQueue queue = queueClient.GetQueueReference("urls");
            queue.CreateIfNotExists();

            CloudQueue urlscrawled = queueClient.GetQueueReference("urlscrawled");
            urlscrawled.CreateIfNotExists();

            CloudQueue size = queueClient.GetQueueReference("size");
            size.CreateIfNotExists();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("crawler");
            table.CreateIfNotExists();

            CloudTableClient errorTableClient = storageAccount.CreateCloudTableClient();
            CloudTable errors = errorTableClient.GetTableReference("errors");
            errors.CreateIfNotExists();

            CloudTableClient statsTableClient = storageAccount.CreateCloudTableClient();
            CloudTable statsTable = statsTableClient.GetTableReference("stats");
            statsTable.CreateIfNotExists();

            CloudTableClient lasttenTableClient = storageAccount.CreateCloudTableClient();
            CloudTable lastten = lasttenTableClient.GetTableReference("lastten");

            List<string> rules = new List<string>();

            
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
                    else commandQueue.DeleteMessage(commandMessage);
                }
            }

            while (true)
            {
                float ram1 = memoryCounter.NextValue();
                float processor1 = cpuCounter.NextValue();
                int counter = 0;

                Thread.Sleep(100);
                commandQueue.FetchAttributes();
                if (commandQueue.ApproximateMessageCount > 0)
                {
                    CloudQueueMessage commandMessage = commandQueue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                    if (commandMessage.AsString.Equals("stop"))
                    {
                        commandQueue.DeleteMessage(commandMessage);
                        queue.FetchAttributes();
                        if (queue.ApproximateMessageCount > 0)
                        {
                            queue.Clear();
                            unique.Clear();
                        }
                        while (true)
                        {
                            commandQueue.FetchAttributes();
                            if (commandQueue.ApproximateMessageCount > 0)
                            {
                                Thread.Sleep(500);
                                CloudQueueMessage resume = commandQueue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                                if (resume.AsString.Equals("start"))
                                {
                                    commandQueue.DeleteMessage(resume);
                                    table.CreateIfNotExists();
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    CloudQueueMessage message = queue.GetMessage(TimeSpan.FromMilliseconds(100.0));
                    string messageString = message.AsString;
                    unique.Add(messageString);
                    urlscrawled.AddMessage(new CloudQueueMessage(messageString));
                    if (messageString.EndsWith(".xml"))
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(messageString);
                        foreach (XmlNode n in document.LastChild.ChildNodes)
                        {
                            XmlNode date = n["lastmod"];
                            if (date != null && DateTime.Parse(date.InnerText).CompareTo(new DateTime(2013, 12, 1)) > 0)
                            {
                                XmlNode location = n["loc"];
                                queue.AddMessage(new CloudQueueMessage(location.InnerText));
                            }
                        }
                    }
                    else
                    {
                        HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                        try
                        {
                            document.Load(messageString);
                            HtmlNode title = document.DocumentNode.SelectSingleNode("title");
                            HtmlNode date = document.DocumentNode.SelectSingleNode("//meta[@http-equiv='last-modified']");
                            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//a[@href] or //@src"))
                            {
                                HtmlAttribute link = node.Attributes["href"];
                                HtmlAttribute source = node.Attributes["src"];
                                string url = "";
                                if (link != null && !unique.Contains(link.Value))
                                    url = link.Value;
                                else if (source != null && !unique.Contains(source.Value))
                                    url = source.Value;
                                if (!String.IsNullOrEmpty(url))
                                {
                                    bool isValidURL = true;
                                    foreach (string rule in rules)
                                    {
                                        if (url.Contains(rule))
                                            isValidURL = false;
                                    }
                                    if (isValidURL && url.Contains("cnn.com"))
                                    {
                                        queue.AddMessage(new CloudQueueMessage(url));
                                        size.AddMessage(new CloudQueueMessage(url));
                                        int end = title.InnerText.IndexOf(" - CNN.com");
                                        string[] titleParts = title.InnerText.Substring(0, end).Split(' ');
                                        foreach (string word in titleParts)
                                        {
                                            URLWrapper thing = new URLWrapper("0", counter % 10 + "", date.Attributes["content"].Value, title.InnerText.Substring(0, end));
                                            counter++;
                                            TableOperation replace = TableOperation.InsertOrReplace(thing);
                                            lastten.Execute(replace);
                                            URLWrapper entity = new URLWrapper(url, word, date.Attributes["content"].Value, title.InnerText.Substring(0, end));
                                            TableOperation insert = TableOperation.Insert(entity);
                                            table.Execute(insert);
                                        }
                                    }
                                }   
                            }
                            queue.DeleteMessage(message);
                        }
                        catch (Exception e)
                        {
                            string error = e.Message;
                            URLWrapper errorentry = new URLWrapper(messageString, error, "", "");
                            TableOperation insertOperation = TableOperation.Insert(errorentry);
                            errors.Execute(insertOperation);
                            queue.DeleteMessage(message);
                        }
                    }
                    float ram2 = memoryCounter.NextValue();
                    float processor2 = cpuCounter.NextValue();
                    Performance stat = new Performance((ram1 + ram2) / 2, (processor1 + processor2) / 2);
                    TableOperation recordStat = TableOperation.InsertOrReplace(stat);
                    statsTable.Execute(recordStat);
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
