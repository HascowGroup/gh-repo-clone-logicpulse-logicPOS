﻿using logicpos.datalayer.DataLayer.Xpo;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace logicpos.shared.App
{
    public static class SharedFramework
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Localization
        public static CultureInfo CurrentCulture;
        public static CultureInfo CurrentCultureNumberFormat;
        //Licence
        public static bool LicenceRegistered = false;
        public static string LicenseVersion;
        public static string LicenseDate;
        public static string LicenseName;
        public static string LicenseCompany;
        public static string LicenseNif;
        public static string LicenseAddress;
        public static string LicenseEmail;
        public static string LicenseTelephone;
        public static string LicenseHardwareId;
        public static string LicenseReseller;
        public static bool LicenseModuleStocks;
        public static DateTime LicenceUpdateDate;
        public static DataTable DtLicenseKeys;
        //TK016248 - BackOffice - Check New Version 
        public static string ServerVersion;
        //AT - Only Used in logicerp.Modules.FINANCIAL | LogicposHelper
        public static Hashtable AT;
        //Database
        public static string DatabaseServer;
        public static string DatabaseName;
        public static string DatabaseUser;
        public static string DatabasePassword;
        public static string DatabaseVersion;
        //WorkSession
        public static pos_worksessionperiod WorkSessionPeriodDay;
        public static pos_worksessionperiod WorkSessionPeriodTerminal;
        //Session
        public static GlobalFrameworkSession SessionApp;
        // Plugins
        public static PluginContainer PluginContainer;
        // Plugins
        public static ISoftwareVendor PluginSoftwareVendor;
        public static ILicenceManager PluginLicenceManager;

        //User/Terminal/Permissions
        public static Dictionary<string, bool> LoggedUserPermissions;
        //PreferenceParameters
        public static Dictionary<string, string> PreferenceParameters;
        //FastReport
        public static Dictionary<string, string> FastReportSystemVars;
        public static Dictionary<string, string> FastReportCustomVars;
        //TK013134: HardCoded Modules
        public static bool AppUseParkingTicketModule = false;
        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        public static bool PrintQRCode = true;
        //Gestão de Stocks : Janela de Gestão de Stocks [IN:016534]
        public static bool CheckStocks = true;
        public static bool CheckStockMessage = true;
        //TK016235 BackOffice - Mode
        public static bool AppUseBackOfficeMode = false;
        public static Dictionary<string, Guid> PendentPayedParkingTickets = new Dictionary<string, Guid>();
        public static Dictionary<string, Guid> PendentPayedParkingCards = new Dictionary<string, Guid>();
        //TK016249 - Impressoras - Diferenciação entre Tipos
        public static bool UsingThermalPrinter;

        //Get Screen Size to use in shared
        public static System.Drawing.Size ScreenSize { get; set; }

        public static string AppRootFolder
        {
            get
            {
                //Log4Net
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                string result = Environment.CurrentDirectory + "/";
                try
                {
                    if (datalayer.App.DataLayerFramework.Settings["AppRootFolder"] != null)
                    {
                        result = datalayer.App.DataLayerFramework.Settings["AppRootFolder"];
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
                return (result);
            }
        }

        private static bool _canOpenFiles = true;
        public static bool CanOpenFiles
        {
            get
            {
                return (_canOpenFiles);
            }
            set
            {
                _canOpenFiles = value;
            }
        }
    }
}