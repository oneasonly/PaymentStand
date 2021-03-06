﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XmlStructureComplat;
using ExceptionManager;
using System.IO;
using System.Runtime.ExceptionServices;

namespace Logic
{
    public static class StaticMain
    {
        //private static Lazy<PagesManager> manager = new Lazy<PagesManager>();
        private static XmlTransactionsManager manager = new XmlTransactionsManager();      
        private static string settingsPath1 = $@"{Environment.CurrentDirectory}\settings.xml";
        private static string settingsPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\settings.xml";
        static StaticMain()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }
        [HandleProcessCorruptedStateExceptions]
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            (e.ExceptionObject as Exception).Show("UnhandledException(AppDomain.CurrentDomain)");
        }

        public static Settings Settings { get; private set; } = new Settings();
        public static async Task<PS_ERIP> NextPage(object select)
        {
            return await manager.NextRequest(select);
        }

        public static async Task<PS_ERIP> BackPage()
        {
            return await manager.PrevRequest();
        }

        public static bool IsBackPossible()
        {
            try
            {
                return manager.IsPrevRequestPossible();
            }
            catch (Exception ex)
            {
                ex.Log();
                return true;
            }

        }

        public static async Task<PS_ERIP> HomePage()
        {
            await LoadSettings();
            return await manager.HomeRequest();
        }

        public static async Task LoadSettings()
        {
            Ex.Log($"StaticMain.LoadSettings() path={settingsPath}");
            if (File.Exists(settingsPath))
            {
                try
                {
                    XDocument fileXml = await Task.Run(() => XDocument.Load(settingsPath));
                    Settings = await SerializationUtil.Deserialize<Settings>(fileXml);
                    Task.Run(async () =>
                    {
                        try
                        {
                            XDocument memory = await SerializationUtil.Serialize(Settings);
                            var inMemParts = memory.Descendants();
                            var fileParts = fileXml.Descendants();
                            if (inMemParts.Count() != fileParts.Count())
                            { SaveSettings().RunAsync(); Ex.Log("inMemParts != fileParts"); }
                        }
                        catch (Exception ex)
                        {
                            ex.Log("Error at saving settings to file, when loading.");
                        }

                    }).RunAsync();
                }
                catch (Exception ex)
                {
                    ex.Show("Error at loading settings.");
                }
            }
            else
            {
                SaveSettings().RunAsync();
            }
        }

        public static async Task SaveSettings()
        {
            try
            {
                XDocument xml = await SerializationUtil.Serialize(Settings);
                await Task.Run(() => xml.Save(settingsPath));
            }
            catch (Exception ex)
            {
                ex.Log("Error at SaveSettings()");
            }

        }
    }
}
