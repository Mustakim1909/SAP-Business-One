using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using System.Globalization;
using EInvoice.Services;
using EInvoice.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.IO;
using EInvoice.Models;
using Serilog;
using Serilog.Events;
using Newtonsoft.Json;
//EInvoice SQL
namespace EInvoice
{
    static class Program
    {
        public static SAPbobsCOM.Company oCompany;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Appsettings AppConfig;
        [STAThread]

        static void Main(string[] args)
        {
            try
            {
               
                var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                var config = new ConfigurationBuilder()
                 .SetBasePath(projectDirectory)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .Build();
                Log.Information($"Appsetting Path : {projectDirectory}");
                AppConfig = config.GetSection("AppSettings").Get<Appsettings>();
                var json = JsonConvert.SerializeObject(AppConfig, Formatting.Indented);
                var deserialized = JsonConvert.DeserializeObject<Appsettings>(json);
                string logDirectory = deserialized.LogPath;
                // Ensure the log directory exists
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create a log file path dynamically based on the passed argument (e.g., file name)
                string logFilePath = Path.Combine(logDirectory, "TraceFile.txt");

                // Configure Serilog logger to use a dynamic log file path
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
                    .CreateLogger();
                // Bind configuration to model
                Application oApp = null;
                if (args.Length < 1)
                {
                    oApp = new Application();

                }
                else
                {
                    oApp = new Application(args[0]);
                }

                oCompany = Utility.clsCreations.getCompany();
                


                    if (oCompany == null)
                    return;


                clsMain objMain = new clsMain();
               objMain.fnExecuteAddOn();

                
                clsMenu objMenu = new clsMenu();

                SAPbouiCOM.MenuItem MenuItem = Application.SBO_Application.Menus.Item("EInvAddOn");



                Boolean bolIsDone;


                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);

                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(objMenu.SBO_Application_ItemEvent);
                


                Application.SBO_Application.FormDataEvent +=  objMenu.SBO_Application_FormDataEvent;

                oApp.RegisterMenuEventHandler(objMenu.SBO_Application_MenuEvent);


                Application.SBO_Application.StatusBar.SetSystemMessage("Advin EInvoice AddOn Connected", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                Log.Information("EInvoice AddOn Connected");

                oApp.Run();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                Log.Error("EInvoice AddOn not Connected");
            }
        }

		
		

		static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    //Exit Add-On
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    break;
                default:
                    break;
            }
        }

      




    }
}
