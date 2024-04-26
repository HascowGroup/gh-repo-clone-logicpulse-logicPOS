﻿using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.service.App;
using logicpos.financial.service.Objects;
using logicpos.financial.service.Objects.Service;
using logicpos.financial.service.Test.Modules.AT;
using logicpos.financial.servicewcf;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using logicpos.shared.App;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace logicpos.financial.service
{
    internal class Program
    {
        //Log4Net basics with a Console Application (c#)
        //http://geekswithblogs.net/MarkPearl/archive/2012/01/30/log4net-basics-with-a-console-application-c.aspx
        //Add the assembly for the log4net.config to Properties/AssemblyInfo.cs
        //[assembly: log4net.Config.XmlConfigurator(Watch = true)]

        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedList<string, Action> _testActions;
        private static readonly string _line = string.Empty;

        //Service Private Members
        public static string SERVICE_NAME = "LogicPulse LogicPos Financial Service";
        private static ServiceHost _serviceHost;
        private static Uri _baseAddress;

        public static int ServicePort { get; } = 50391;
        //Timer
        private static System.Timers.Timer _timer = null;
        private static bool _timerRunningTasks = false;

        private static void Main(string[] args)
        {
            //Init Settings Main Config Settings
            DataLayerFramework.Settings = ConfigurationManager.AppSettings;

            //Base Bootstrap Init from LogicPos
            Init();

            //Service Initialization
            string uri = string.Format("http://localhost:{0}/Service1.svc", ServicePort);
            _logger.Debug(string.Format("Service URI: {0}", uri));
            _baseAddress = new Uri(uri);

            //Service Mode
            if (!Environment.UserInteractive)
            {
                // Running as service
                using (var service = new Service())
                {
                    _logger.Debug("Service.Run(service)");
                    System.ServiceProcess.ServiceBase.Run(service);
                }
            }
            //Console Mode
            else
            {
                Utils.Log("Launch service? [Y or Enter] or any other key to run in interactive develop/debug mode");
                ConsoleKeyInfo cki = Console.ReadKey();

                //Service Mode
                if (cki.Key.ToString().ToUpper() == "y".ToUpper() || cki.Key.ToString() == "Enter")
                {
                    // Running as console app
                    Start(args);

                    Console.Clear();
                    Utils.Log(string.Format("The service is ready at {0}", _baseAddress));
                    Utils.Log("Press any key to stop the service and exit");
                    Console.WriteLine();
                    Console.ReadKey();
                    Stop();
                }
                //Interactive develop mode
                else
                {
                    //Init Test Actions
                    InitTestActions();
                    //Init Main
                    InitMain();
                }
            }
        }

        //LogicPos BootStrap
        private static void Init()
        {
            try
            {
                //After Construct Settings (ex Required path["certificates"])
                //Utils.Log(string.Format("BootStrap {0}....", SettingsApp.AppName));

                // Init Paths
                DataLayerFramework.Path = new Hashtable
                {
                    { "temp", SharedUtils.OSSlash(DataLayerFramework.Settings["pathTemp"]) },
                    { "certificates", SharedUtils.OSSlash(DataLayerFramework.Settings["pathCertificates"]) },
                    { "plugins", SharedUtils.OSSlash(DataLayerFramework.Settings["pathPlugins"]) }
                };
                //Create Directories
                SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["temp"])));
                SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["certificates"])));

                // Protection for plugins Path
                if (DataLayerFramework.Path["plugins"] == null || !Directory.Exists(DataLayerFramework.Path["plugins"].ToString()))
                {
                    Utils.Log($"Missing pathPlugins: {DataLayerFramework.Settings["pathPlugins"]}. Please correct path in config! ex \"c:\\Program Files (x86)\\Logicpulse\"");
                    //Output only if in Console Mode
                    if (Environment.UserInteractive)
                    {
                        Utils.Log("Press any key...");
                        Console.ReadKey();
                    }
                    Environment.Exit(0);
                }

                // VendorPlugin
                InitPlugins();

                //Prepare AutoCreateOption
                AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;

                //Get DataBase Details
               DataLayerFramework.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), DataLayerFramework.Settings["databaseType"]);
                SharedFramework.DatabaseName = FinancialServiceSettings.DatabaseName;
                //Override default Database name with parameter from config
                string configDatabaseName = DataLayerFramework.Settings["databaseName"];
                SharedFramework.DatabaseName = (string.IsNullOrEmpty(configDatabaseName)) ? FinancialServiceSettings.DatabaseName : configDatabaseName;
                //Xpo Connection String
                string xpoConnectionString = string.Format(DataLayerFramework.Settings["xpoConnectionString"], SharedFramework.DatabaseName.ToLower());

                //Init XPO Connector DataLayer
                try
                {
                    _logger.Debug(string.Format("Init XpoDefault.DataLayer: [{0}]", xpoConnectionString));


                    var connectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder()
                    { ConnectionString = xpoConnectionString };
                    if (connectionStringBuilder.ContainsKey("password")) { connectionStringBuilder["password"] = "*****"; };
                    _logger.Debug(string.Format("void Init() :: Init XpoDefault.DataLayer: [{0}]", connectionStringBuilder.ToString()));

                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                    DataLayerFramework.SessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };

                    //if (XpoDefault.DataLayer != null)
                    ////Utils.Log("DataLayer...");
                    ////DataLayerFramework.SessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                    //if (DataLayerFramework.SessionXpo != null)
                    //    //Utils.Log("SessionXpo...");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    //Output only if in Console Mode
                    if (Environment.UserInteractive)
                    {
                        Utils.Log(ex.Message);
                        Utils.Log("Press any key...");
                        Console.ReadKey();
                    }
                    Environment.Exit(0);
                }

                //PreferenceParameters
                SharedFramework.PreferenceParameters = SharedUtils.GetPreferencesParameters();

                //Check parameters in debug
                //try
                //{
                //    foreach (var pref inSharedFramework.PreferenceParameters)
                //    {
                //        _logger.Debug(string.Format(pref.Key + ": " + pref.Value));
                //    }
                //}catch(Exception Ex) { _logger.Debug(Ex.Message); }

                //CultureInfo/Localization
                string culture = SharedFramework.PreferenceParameters["CULTURE"];
                if (!string.IsNullOrEmpty(culture))
                {
                    /* IN006018 and IN007009 */
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                }

                SharedFramework.CurrentCulture = new CultureInfo(DataLayerFramework.Settings["customCultureResourceDefinition"]);
                if (SharedFramework.CurrentCulture == null)
                {
                    //Get Culture from DB
                    string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
                    string getCultureFromDB = DataLayerFramework.SessionXpo.ExecuteScalar(sql).ToString();
                    SharedFramework.CurrentCulture = new System.Globalization.CultureInfo(getCultureFromDB);
                    if (SharedFramework.CurrentCulture != null)
                        _logger.Debug(SharedFramework.CurrentCulture.DisplayName);
                    else
                    {
                        _logger.Debug("No culture loaded");
                    }
                }
                //Always use en-US NumberFormat because of MySql Requirements
                SharedFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(FinancialServiceSettings.CultureNumberFormat);

                //SettingsApp
                string companyCountryOid = SharedFramework.PreferenceParameters["COMPANY_COUNTRY_OID"];
                string systemCurrencyOid = SharedFramework.PreferenceParameters["SYSTEM_CURRENCY_OID"];
                DataLayerSettings.ConfigurationSystemCountry = (cfg_configurationcountry)DataLayerUtils.GetXPGuidObject(DataLayerFramework.SessionXpo, typeof(cfg_configurationcountry), new Guid(companyCountryOid));
                SharedSettings.ConfigurationSystemCurrency = (cfg_configurationcurrency)DataLayerUtils.GetXPGuidObject(DataLayerFramework.SessionXpo, typeof(cfg_configurationcurrency), new Guid(systemCurrencyOid));

                //After Construct Settings (ex Required path["certificates"])
                Utils.Log(string.Format("BootStrap {0}....", FinancialServiceSettings.AppName));

                //Show WS Mode
                Utils.Log(string.Format("ServiceATEnableTestMode: [{0}]", FinancialServiceSettings.ServiceATEnableTestMode));

                // Protection to Check if all Required values are met
                if (!HasAllRequiredValues())
                {
                    throw new Exception($"Error! Invalid Parameters Met! Required parameters missing! Check parameters: AccountFiscalNumber: [{FinancialServiceSettings.ServicesATAccountFiscalNumber}], ATAccountPassword: [{FinancialServiceSettings.ServicesATAccountPassword}], TaxRegistrationNumber: [{FinancialServiceSettings.ServicesATTaxRegistrationNumber}]");
                }
                if (!Utils.IsLinux)
                {
                    SystemEvents.PowerModeChanged += OnPowerChange;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static void InitPlugins()
        {
            // Check Path before launch error found Plugin
            string pathPlugins = DataLayerFramework.Path["plugins"].ToString();
            if (string.IsNullOrEmpty(pathPlugins))
            {
                // Error Missing pluginPath
                string errorMessage = "Error! missing plugin path in config! Please fix config and try again!";
                _logger.Error(errorMessage);
                Console.WriteLine(errorMessage);
                Console.ReadKey();
                Environment.Exit(0);
            }
            // Init PluginContainer
            SharedFramework.PluginContainer = new PluginContainer(DataLayerFramework.Path["plugins"].ToString());

            // PluginSoftwareVendor
            SharedFramework.PluginSoftwareVendor = (SharedFramework.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>());
            if (SharedFramework.PluginSoftwareVendor != null)
            {
                // Show Loaded Plugin
                _logger.Debug(string.Format("Registered plugin: [{0}] Name : [{1}]", typeof(ISoftwareVendor), SharedFramework.PluginSoftwareVendor.Name));
                // Init Plugin
                SharedSettings.InitSoftwareVendorPluginSettings();
            }
            else
            {
                // Error Loading Required Plugin
                string errorMessage = string.Format("Error! missing required plugin: [{0}]. Install required plugin and try again!", typeof(ISoftwareVendor));
                _logger.Error(errorMessage);
                Console.WriteLine(errorMessage);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static bool HasAllRequiredValues()
        {
            return !(string.IsNullOrEmpty(FinancialServiceSettings.ServicesATAccountFiscalNumber)
                || string.IsNullOrEmpty(FinancialServiceSettings.ServicesATAccountPassword)
                || string.IsNullOrEmpty(FinancialServiceSettings.ServicesATTaxRegistrationNumber)
            );
        }

        private static void InitTestActions()
        {
            // Init Dictionary
            _testActions = new SortedList<string, Action>
            {
                {
                    "1) TestSendDocument.SendFinanceDocument()",
                    () =>
                TestSendDocument.SendDocumentNonWayBill()
                },
                {
                    "2) TestSendDocument.SendWayBillDocument()",
                    () =>
                    TestSendDocument.SendDocumentWayBill()
                }
            };
        }

        private static void InitMain()
        {
            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            //Utils.Log("Press any combination of CTL, ALT, and SHIFT, and a console key.");
            //Utils.Log("Press the Escape (Esc) key to quit: \n");
            ListTestActions();

            do
            {
                Utils.Log("INP> ");
                cki = Console.ReadKey();
                Utils.Log("\n");

                //Pressed Number Key
                if (cki.Key.ToString().Length == 2 && cki.Key.ToString().Substring(0, 1) == "D")
                {
                    int keyNumber = Convert.ToInt16(cki.Key.ToString().Substring(1, 1)) - 1;
                    try
                    {
                        if (keyNumber >= 0 && keyNumber <= _testActions.Count)
                        {
                            Console.Clear();
                            //Utils.Log(string.Format("Pressed keyNumber: {0}", keyNumber, _testActions.Keys[0]));
                            Utils.Log(string.Format("Test: {0}", _testActions.Keys[keyNumber]));
                            Utils.Log(_line);

                            try
                            {
                                _testActions.Values[keyNumber].Invoke();
                            }
                            catch (Exception ex)
                            {
                                Utils.Log(ex.Message);
                            }

                            Utils.Log(_line);
                            Utils.Log("Press any key...");
                            Console.ReadKey();
                            ListTestActions();
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.Log(ex.Message);
                        Utils.Log("Press any key...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    ListTestActions();
                }
            } while (cki.Key != ConsoleKey.Escape);

            //Quit
            //Utils.Log("Press any key....");
            //Console.ReadKey();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper

        private static void ListTestActions()
        {
            Console.Clear();

            Utils.Log(string.Format("{0} : Database: [{1}]", FinancialServiceSettings.AppName, FinancialServiceSettings.DatabaseName));
            Utils.Log(_line);

            int i = 0;
            foreach (var testAction in _testActions)
            {
                i++;
                //Utils.Log(string.Format("{0}) {1}", i.ToString("00"), testAction.Key));
                Utils.Log(testAction.Key);
            }
            Utils.Log(_line);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Service Methods

        public static void Start(string[] args)
        {
            _logger.Debug("Service Started");

            //Call ModifyHttpSettings
            Utils.ModifyHttpSettings();

            //Init ServiceHost
            _serviceHost = new ServiceHost(typeof(Service1), _baseAddress);

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            _serviceHost.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            _serviceHost.Open();

            //StartTimer
            StartTimer();
        }

        public static void Stop()
        {
            // onstop code here
            _logger.Debug("Service Stoped");
            // Close the ServiceHost.
            _serviceHost.Close();

            //StopTimer
            StopTimer();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Timer

        public static void StartTimer()
        {
            if (FinancialServiceSettings.ServiceTimerEnabled)
            {
                _logger.Debug("Service StartTimer to " + FinancialServiceSettings.ServiceTimer.Hour + ":" + FinancialServiceSettings.ServiceTimer.Minute);
                DateTime nowTime = DateTime.Now;
                DateTime oneAmTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, FinancialServiceSettings.ServiceTimer.Hour, FinancialServiceSettings.ServiceTimer.Minute, 0, 0);
                if (nowTime > oneAmTime)
                    oneAmTime = oneAmTime.AddDays(1);

                double tickTime = (oneAmTime - nowTime).TotalMilliseconds;
                _timer = new System.Timers.Timer(tickTime);
                _timer.Elapsed += TimerElapsedEvent;
                _timer.Start();
            }
        }

        public static void StopTimer()
        {
            if (FinancialServiceSettings.ServiceTimerEnabled && _timer != null)
            {
                //_logger.Debug("Service StopTimer");
                _timer.Stop();
                _timer = null;
            }
        }

        public static void TimerElapsedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_timerRunningTasks)
            {

                StopTimer();
                //Stop();
                //Started Running Tasks
                _timerRunningTasks = true;

                _logger.Debug(string.Format("Send Documents to AT"));
                //Financial.service - Correções no envio de documentos AT [IN:014494]
                //Now only works in prodution
                if (Convert.ToBoolean(DataLayerFramework.Settings["ServiceATSendDocuments"]) || Convert.ToBoolean(DataLayerFramework.Settings["ServiceATSendDocumentsWayBill"]))
                {
                    _logger.Debug(string.Format("ServiceATSendDocuments True"));
                    Utils.ServiceSendPendentDocuments();
                }

                //Finished Running Tasks
                _timerRunningTasks = false;
                StartTimer();
            }
        }

        private static void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            try
            {
                switch (e.Mode)
                {
                    case PowerModes.Resume:
                        StartTimer();
                        _logger.Debug("Windows Resume");
                        break;
                    case PowerModes.Suspend:
                        StopTimer();
                        _logger.Debug("Windows Suspend");
                        break;
                }
            }
            catch (Exception Ex)
            {
                _logger.Error("Erro ao suspender/retomar sessão no Windows" + Ex.Message);
            }

        }
    }
}
