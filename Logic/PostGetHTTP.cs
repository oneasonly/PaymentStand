﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logic
{
    public static class PostGetHTTP
    {        
        private static readonly string url = "http://public.softclub.by:3007/komplat/online.request"; //"http://public.softclub.by:3007/komplat/online.request"
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly Stopwatch timer = new Stopwatch();
        private static List<XDocument> ResponceHistory = new List<XDocument>();
        private static List<XDocument> RequestHistory = new List<XDocument>();


        public static event Action<string> WriteTextBox = (x) => { };
        public static event Action<XDocument> XmlReceived = (x) => { };

        private static async Task<XDocument> PostStringGetXML(string destinationUrl, string requestXml)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.UTF8.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                string timings = "";
                XDocument msg;
                timer.Restart();
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    timer.Stop();
                    timings += $"GetRequestStream={timer.ElapsedMilliseconds}; ";
                    WriteTextBox(requestXml);
                    timer.Restart();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var responseXml = await XmlLoadAsync(responseStream);
                            timer.Stop();
                            XmlReceived(responseXml);
                            timings = $"Responce={timer.ElapsedMilliseconds}; {timings}\n";
                            msg = responseXml;
                        }
                    }
                    else
                    {
                        timer.Stop();
                        timings = $"Responce={timer.ElapsedMilliseconds}; {timings}\n";
                        msg = null;

                    }
                }
                WriteTextBox(msg.ToStringFull());
                return msg;
            }
            catch (Exception ex)
            {
                var msg = $"1postXMLData():\n{ex.Message}";
                WriteTextBox(msg);
                return null;
            }
        }
        private static async Task<string> PostStringGetString(string destinationUrl, string requestXml)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.UTF8.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                string timings = "";
                string msg;
                timer.Restart();
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    timer.Stop();
                    timings += $"GetRequestStream={timer.ElapsedMilliseconds}; ";
                    WriteTextBox(requestXml);
                    timer.Restart();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var responseStream = new StreamReader(response.GetResponseStream()))
                        {
                            string responseStr = await responseStream.ReadToEndAsync();
                            timer.Stop();
                            timings = $"Responce={timer.ElapsedMilliseconds}; {timings}\n";
                            msg = $"{timings}{responseStr}";
                        }
                    }
                    else
                    {
                        timer.Stop();
                        timings = $"Responce={timer.ElapsedMilliseconds}; {timings}\n";
                        msg = $"{timings}response.StatusCode={response.StatusCode}\n{response.StatusDescription}";

                    }
                }
                WriteTextBox(msg);
                return msg;
            }
            catch (Exception ex)
            {
                var msg = $"1postXMLData():\n{ex.Message}";
                WriteTextBox(msg);
                return msg;
            }

        }

        public static async Task<XDocument> PostStringGetXML(string requestXml)
        {
            return await PostStringGetXML(url, $"<?xml version=\"1.0\" encoding=\"UTF-8\" standalone='yes'?>\n{requestXml}");
        }
        public static async Task<string> PostStringGetString(string requestXml)
        {
            return await PostStringGetString(url, $"<?xml version=\"1.0\" encoding=\"UTF-8\" standalone='yes'?>\n{requestXml}");
        }

        public static async Task<XDocument> XmlLoadAsync(Stream stream, LoadOptions loadOptions = LoadOptions.PreserveWhitespace)
        {
            return await Task.Run(() =>
            {
                return XDocument.Load(stream, loadOptions);
            });
        }

        public static async Task<XDocument> XmlLoadAsync(string str, LoadOptions loadOptions = LoadOptions.PreserveWhitespace)
        {
            return await Task.Run(() =>
            {
                return XDocument.Parse(str, loadOptions);
            });
        }

        private static async Task<string> GetResponseText(string address)
        {
            return await httpClient.GetStringAsync(address);
        }
        public static string ToStringFull(this XDocument d) => $"{d.Declaration}{Environment.NewLine}{d}";

    }
}
