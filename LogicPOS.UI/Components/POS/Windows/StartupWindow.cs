using Gdk;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using LogicPOS.Settings;
using LogicPOS.UI;
using System;
using Image = Gtk.Image;

namespace logicpos
{
    public partial class StartupWindow : PosBaseWindow
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NumberPadPin _numberPadPin;
        public TablePad TablePadUser { get; set; }
        private sys_userdetail _selectedUserDetail;

        public StartupWindow(
            string backgroundImage,
            bool needToUpdate)
            : base(backgroundImage)
        {
            InitializeUI();

            //InitPlataform
            InitPlataformParameters();

            //show changelog
            if (needToUpdate)
            {
                Utils.ShowChangeLog(this);
            }

            //Show Notifications to all users after Show UI, here we dont have a logged user Yet
            Utils.ShowNotifications(this);

            //Assign to member UserDetail reference, after InitUi, this way ChangePassword Message appears after StartupWindow
            AssignUserDetail();

            //Events
            this.KeyReleaseEvent += StartupWindow_KeyReleaseEvent;
        }

        private void InitPlataformParameters()
        {
            try
            {
                //Get ConfigurationPreferenceParameter Values to Check if Plataform is Inited
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCountryOid = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_OID")) as cfg_configurationpreferenceparameter);
                cfg_configurationpreferenceparameter configurationPreferenceParameterSystemCurrencyOid = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "SYSTEM_CURRENCY_OID")) as cfg_configurationpreferenceparameter);
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCountryCode2 = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_CODE2")) as cfg_configurationpreferenceparameter);
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyFiscalNumber = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_FISCALNUMBER")) as cfg_configurationpreferenceparameter);

                if (
                    string.IsNullOrEmpty(configurationPreferenceParameterCompanyCountryOid.Value) ||
                    string.IsNullOrEmpty(configurationPreferenceParameterCompanyCountryCode2.Value) ||
                    string.IsNullOrEmpty(configurationPreferenceParameterCompanyFiscalNumber.Value) ||
                    string.IsNullOrEmpty(configurationPreferenceParameterSystemCurrencyOid.Value)
                )
                {
                    PosEditCompanyDetails dialog = new PosEditCompanyDetails(this, DialogFlags.DestroyWithParent | DialogFlags.Modal, false);
                    ResponseType response = (ResponseType)dialog.Run();
                    dialog.Destroy();
                }

                //Always Get Objects from Prefs to Singleton : with and without PosEditCompanyDetails
                configurationPreferenceParameterCompanyCountryOid = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_OID")) as cfg_configurationpreferenceparameter);
                configurationPreferenceParameterSystemCurrencyOid = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "SYSTEM_CURRENCY_OID")) as cfg_configurationpreferenceparameter);
                XPOSettings.ConfigurationSystemCountry = (cfg_configurationcountry)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcountry), new Guid(configurationPreferenceParameterCompanyCountryOid.Value));
                XPOSettings.ConfigurationSystemCurrency = (cfg_configurationcurrency)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcurrency), new Guid(configurationPreferenceParameterSystemCurrencyOid.Value));

                _logger.Debug(string.Format("Using System Country: [{0}], Currency: [{1}]", XPOSettings.ConfigurationSystemCountry.Designation, XPOSettings.ConfigurationSystemCurrency.Designation));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private dynamic GetTheme()
        {
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
            var theme = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
            return theme;
        }

        private void InitializeUI()
        {

            dynamic theme = GetTheme();

            string errorMessage = "Node: <Window ID=\"StartupWindow\">";

            //Assign Theme Vars + UI
            if (theme != null)
            {
                try
                {
                    //Globals
                    Title = Convert.ToString(theme.Globals.Name);
                    //Objects:LabelVersion
                    System.Drawing.Point labelVersionPosition = Utils.StringToPosition(theme.Objects.LabelVersion.Position);
                    string labelVersionFont = theme.Objects.LabelVersion.Font;
                    Color labelVersionFontColor = (theme.Objects.LabelVersion.FontColor as string).StringToGdkColor();
                    //Objects:NumberPadPin
                    System.Drawing.Point numberPadPinPosition = Utils.StringToPosition(theme.Objects.NumberPadPin.Position);
                    System.Drawing.Size numberPadPinButtonSize = Utils.StringToSize(theme.Objects.NumberPadPin.ButtonSize);
                    string numberPadPinFont = theme.Objects.NumberPadPin.Font;
                    System.Drawing.Color numberPadPinFontColor = (theme.Objects.NumberPadPin.FontColor as string).StringToColor();
                    uint numberPadPinRowSpacingSystemButtons = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingSystemButtons);
                    uint numberPadPinRowSpacingLabelStatus = Convert.ToUInt16(theme.Objects.NumberPadPin.RowSpacingLabelStatus);
                    //Objects:NumberPadPin:LabelStatus
                    string numberPadPinLabelStatusFont = theme.Objects.NumberPadPin.LabelStatus.Font;
                    System.Drawing.Color numberPadPinLabelStatusFontColor = (theme.Objects.NumberPadPin.LabelStatus.FontColor as string).StringToColor();
                    //Objects:NumberPadPin:Size (EventBox)
                    bool NumberPadPinVisibleWindow = Convert.ToBoolean(theme.Objects.NumberPadPin.VisibleWindow);
                    System.Drawing.Size numberPadPinSize = Utils.StringToSize(theme.Objects.NumberPadPin.Size);

                    //Objects:NumberPadPin:ButtonPasswordReset
                    //Position numberPadPinButtonPasswordResetPosition = Utils.StringToPosition(themeWindow.Objects.NumberPadPin.ButtonPasswordReset.Position);
                    //System.Drawing.Size numberPadPinButtonPasswordResetSize = Utils.StringToSize(themeWindow.Objects.NumberPadPin.ButtonPasswordReset.Size);
                    //System.Drawing.Size numberPadPinButtonPasswordResetIconSize = new System.Drawing.Size(numberPadPinButtonPasswordResetSize.Width - 10, numberPadPinButtonPasswordResetSize.Height - 10);
                    //string numberPadPinButtonPasswordResetImageFileName = themeWindow.Objects.NumberPadPin.ButtonPasswordReset.ImageFileName;

                    //Objects:TablePadUserButtonPrev
                    System.Drawing.Point tablePadUserButtonPrevPosition = Utils.StringToPosition(theme.Objects.TablePadUser.TablePadUserButtonPrev.Position);
                    System.Drawing.Size tablePadUserButtonPrevSize = Utils.StringToSize(theme.Objects.TablePadUser.TablePadUserButtonPrev.Size);
                    string tablePadUserButtonPrevImageFileName = theme.Objects.TablePadUser.TablePadUserButtonPrev.ImageFileName;
                    //Objects:TablePadUserButtonNext
                    System.Drawing.Point tablePadUserButtonNextPosition = Utils.StringToPosition(theme.Objects.TablePadUser.TablePadUserButtonNext.Position);
                    System.Drawing.Size tablePadUserButtonNextSize = Utils.StringToSize(theme.Objects.TablePadUser.TablePadUserButtonNext.Size);
                    string tablePadUserButtonNextImageFileName = theme.Objects.TablePadUser.TablePadUserButtonNext.ImageFileName;
                    //Objects:TablePadUser
                    System.Drawing.Point tablePadUserPosition = Utils.StringToPosition(theme.Objects.TablePadUser.Position);
                    System.Drawing.Size tablePadUserButtonSize = Utils.StringToSize(theme.Objects.TablePadUser.ButtonSize);
                    TableConfig tablePadUserTableConfig = Utils.StringToTableConfig(theme.Objects.TablePadUser.TableConfig);
                    bool tablePadUserVisible = Convert.ToBoolean(theme.Objects.TablePadUser.Visible);

                    //Init UI
                    Fixed fix = new Fixed();


                    if (GtkSettings.ShowMinimizeButton)
                    {
                        EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                        eventBoxMinimize.ButtonReleaseEvent += delegate { Iconify(); };
                        fix.Put(eventBoxMinimize, GlobalApp.ScreenSize.Width - 27 - 10, 10);
                    }

                    //NumberPadPin
                    _numberPadPin = new NumberPadPin(this, "numberPadPin", System.Drawing.Color.Transparent, numberPadPinFont, numberPadPinLabelStatusFont, numberPadPinFontColor, numberPadPinLabelStatusFontColor, Convert.ToByte(numberPadPinButtonSize.Width), Convert.ToByte(numberPadPinButtonSize.Height), false, true, NumberPadPinVisibleWindow, numberPadPinRowSpacingLabelStatus, numberPadPinRowSpacingSystemButtons);
                    //Create and Assign local touchButtonKeyPasswordReset Reference to numberPadPin.ButtonKeyResetPassword
                    //TouchButtonIcon touchButtonKeyPasswordReset = new TouchButtonIcon("touchButtonKeyPasswordReset_Green", System.Drawing.Color.Transparent, numberPadPinButtonPasswordResetImageFileName, numberPadPinButtonPasswordResetIconSize, numberPadPinButtonPasswordResetSize.Width, numberPadPinButtonPasswordResetSize.Height) { Sensitive = false };
                    //_numberPadPin.ButtonKeyResetPassword = touchButtonKeyPasswordReset;
                    // Apply Size to Inner EventBox
                    if (numberPadPinSize.Width > 0 || numberPadPinSize.Height > 0)
                    {
                        _numberPadPin.Eventbox.WidthRequest = numberPadPinSize.Width;
                        _numberPadPin.Eventbox.HeightRequest = numberPadPinSize.Height;
                    }

                    //Put in Fix
                    fix.Put(_numberPadPin, numberPadPinPosition.X, numberPadPinPosition.Y);
                    //Over NumberPadPin
                    //fix.Put(touchButtonKeyPasswordReset, numberPadPinButtonPasswordResetPosition.X, numberPadPinButtonPasswordResetPosition.Y);
                    //Events
                    _numberPadPin.ButtonKeyOK.Clicked += ButtonKeyOK_Clicked;
                    _numberPadPin.ButtonKeyResetPassword.Clicked += ButtonKeyResetPassword_Clicked;
                    _numberPadPin.ButtonKeyFrontOffice.Clicked += ButtonKeyFrontOffice_Clicked;
                    _numberPadPin.ButtonKeyBackOffice.Clicked += ButtonKeyBackOffice_Clicked;
                    _numberPadPin.ButtonKeyQuit.Clicked += ButtonKeyQuit_Clicked;

                    //Objects:TablePadUserButtonPrev
                    TouchButtonIcon tablePadUserButtonPrev = new TouchButtonIcon("TablePadUserButtonPrev", System.Drawing.Color.Transparent, tablePadUserButtonPrevImageFileName, new System.Drawing.Size(tablePadUserButtonPrevSize.Width - 2, tablePadUserButtonPrevSize.Height - 2), tablePadUserButtonPrevSize.Width, tablePadUserButtonPrevSize.Height);
                    tablePadUserButtonPrev.Relief = ReliefStyle.None;
                    tablePadUserButtonPrev.BorderWidth = 0;
                    tablePadUserButtonPrev.CanFocus = false;
                    //Objects:TablePadUserButtonNext
                    TouchButtonIcon tablePadUserButtonNext = new TouchButtonIcon("TablePadUserButtonNext", System.Drawing.Color.Transparent, tablePadUserButtonNextImageFileName, new System.Drawing.Size(tablePadUserButtonNextSize.Width - 2, tablePadUserButtonNextSize.Height - 2), tablePadUserButtonNextSize.Width, tablePadUserButtonNextSize.Height);
                    tablePadUserButtonNext.Relief = ReliefStyle.None;
                    tablePadUserButtonNext.BorderWidth = 0;
                    tablePadUserButtonNext.CanFocus = false;
                    //Objects:TablePadUser
                    string sqlTablePadUser = @"
                        SELECT 
                            Oid as id, Name as name, Name as label, ButtonImage as image
                        FROM 
                            sys_userdetail
                        WHERE 
                            (Disabled IS NULL or Disabled <> 1)
                    ";

                    TablePadUser = new TablePad(
                        sqlTablePadUser,
                        "ORDER BY Ord",
                        "",
                        Guid.Empty,
                        true,
                        tablePadUserTableConfig.Rows,
                        tablePadUserTableConfig.Columns,
                        "buttonUserId",
                        System.Drawing.Color.Transparent,
                        tablePadUserButtonSize.Width,
                        tablePadUserButtonSize.Height,
                        tablePadUserButtonPrev,
                        tablePadUserButtonNext
                    );
                    TablePadUser.SourceWindow = this;
                    TablePadUser.Clicked += TablePadUser_Clicked;

                    //Put in Fix
                    if (tablePadUserVisible)
                    {
                        fix.Put(tablePadUserButtonPrev, tablePadUserButtonPrevPosition.X, tablePadUserButtonPrevPosition.Y);
                        fix.Put(tablePadUserButtonNext, tablePadUserButtonNextPosition.X, tablePadUserButtonNextPosition.Y);
                        fix.Put(TablePadUser, tablePadUserPosition.X, tablePadUserPosition.Y);
                    }

                    //Label Version
                    string appVersion = "";
                    if (LicenseSettings.LicenseReseller != null && LicenseSettings.LicenseReseller.ToString().ToLower() != "Logicpulse" && LicenseSettings.LicenseReseller.ToString().ToLower() != "")
                    {
                        //appVersion = string.Format("Brough by {1}\n{0}",appVersion, GlobalFramework.LicenceReseller);
                        appVersion = string.Format("Powered by {0}� Vers. {1}", LicenseSettings.LicenseReseller, GeneralSettings.ProductVersion);
                    }
                    else
                    {
                        appVersion = string.Format(PluginSettings.AppSoftwareVersionFormat, GeneralSettings.ProductVersion);
                    }
                    Label labelVersion = new Label(appVersion);
                    Pango.FontDescription fontDescLabelVersion = Pango.FontDescription.FromString(labelVersionFont);
                    labelVersion.ModifyFg(StateType.Normal, labelVersionFontColor);
                    labelVersion.ModifyFont(fontDescLabelVersion);
                    labelVersion.Justify = Justification.Center;
                    labelVersion.WidthRequest = 307;
                    labelVersion.HeightRequest = 50;
                    labelVersion.SetAlignment(0.5F, 0.5F);

                    //Put in Fix
                    fix.Put(labelVersion, labelVersionPosition.X, labelVersionPosition.Y);

                    //Developer Dialog - Enabled / Disable Developer Button
#if (DEBUG)
                    bool developerButtonEnabled = true;
#else
                   bool developerButtonEnabled = false;
#endif

                    if (developerButtonEnabled)
                    {
                        Button buttonDeveloper = new Button("Developer");
                        fix.Put(buttonDeveloper, 10, 100);
                        buttonDeveloper.Clicked += ButtonDeveloper_Clicked;
                    }
                    //LOGO
                    if (PluginSettings.LicenceManager != null)
                    {
                        string fileImageBackOfficeLogo = string.Format(PathsSettings.Paths["themes"] + @"Default\Images\logicPOS_loggericpulse_loggerin.png");

                        if (!string.IsNullOrEmpty(LicenseSettings.LicenseReseller) && LicenseSettings.LicenseReseller == "NewTech")
                        {
                            fileImageBackOfficeLogo = string.Format(PathsSettings.Paths["themes"] + @"Default\Images\Branding\{0}\logicPOS_loggericpulse_loggerin.png", "NT");
                        }

                        // var bitmapImage = GlobalFramework.PluginLicenceManager.DecodeImage(fileImageBackOfficeLogo, (GlobalApp.ScreenSize.Width - 550), (GlobalApp.ScreenSize.Height - 550));
                        // Gdk.Pixbuf pixbufImageLogo = Utils.ImageToPixbuf(bitmapImage);
                        //Image imageLogo = new Image(pixbufImageLogo);

                        //fix.Put(imageLogo, GlobalApp.ScreenSize.Width - 430, 80);
                    }
                    else
                    {
                        Image imageLogo = new Image(Utils.GetThemeFileLocation(GeneralSettings.Settings["fileImageBackOfficeLogo"]));
                        fix.Put(imageLogo, GlobalApp.ScreenSize.Width - 430, 80);
                    }
                    //string fileImageBackOfficeLogo = Utils.GetThemeFileLocation(LogicPOS.Settings.GeneralSettings.Settings["fileImageBackOfficeLogo"]);
                    ScreenArea.Add(fix);

                    //Force EntryPin to be the Entry with Focus
                    _numberPadPin.EntryPin.GrabFocus();

                    ShowAll();

                    //Events
                    _numberPadPin.ExposeEvent += delegate
                    {
                        _numberPadPin.ButtonKeyFrontOffice.Hide();
                        _numberPadPin.ButtonKeyBackOffice.Hide();
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    Utils.ShowMessageTouchErrorRenderTheme(this, $"{errorMessage}{Environment.NewLine}{Environment.NewLine}{ex.Message}");
                }
            }
            else
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
            }
        }

        private void ButtonDeveloper_Clicked(object sender, EventArgs e)
        {
           
            PosDeveloperTestDialog dialog = new PosDeveloperTestDialog(this, DialogFlags.DestroyWithParent | DialogFlags.Modal);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();
            

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Company Details Dialog

            //PosEditCompanyDetails dialog = new PosEditCompanyDetails(this, DialogFlags.DestroyWithParent | DialogFlags.Modal);
            //ResponseType response = (ResponseType) dialog.Run();
            //dialog.Destroy();

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test DocumentPaymentCreatePDF
            //DocumentFinancePayment documentFinancePayment = (DocumentFinancePayment) XPOSettings.Session.GetObjectByKey(typeof(DocumentFinancePayment), new Guid("e53e779d-4af4-4323-9b2b-48f4ebf6b0c6"));
            //if (documentFinancePayment != null) {
            //    string fileName = CustomReport.DocumentPaymentCreatePDF(documentFinancePayment);
            //}

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test Backups
            //DataBaseBackup.Backup(this);

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test Unpack In Linux
            //Utils.ZipUnPack("/media/lpdev/LogicPosBackups/mysql_loggericposdb29_450_20151203_125525.bak", "/media/lpdev/LogicPosBackups");

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //#1 SelectRecord : Test SelectRecord
            //UsingSelectRecordTreeViewWithCheckBox usingSelectRecordTreeViewWithCheckBox = new UsingSelectRecordTreeViewWithCheckBox(this);
            //DataTable dataTable;
            //DataTable dataTable = usingSelectRecordTreeViewWithCheckBox.SelectRecordDialog();
            //_logger.Debug(string.Format("dataTable.Rows.Count: [{0}]", dataTable.Rows.Count));

            //#2 SelectRecord : Test SelectRecord With Helper and Custom Sql :)
            //dataTable = PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries>.GetSelected(this);
            //if (dataTable != null) _logger.Debug(string.Format("dataTable.Rows.Count: [{0}]", dataTable.Rows.Count));

            //#3 SelectRecord : Test SelectRecord With Helper and Custom Sql :) - Dont have records to Show
            //dataTable = PosSelectRecordDialog<DataTable, DataRow, TreeViewDocumentFinanceArticle>.GetSelected(this);
            //if (dataTable != null) _logger.Debug(string.Format("dataTable.Rows.Count: [{0}]", dataTable.Rows.Count));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Create FiscalYear
            //DocumentFinanceYears documentFinanceYears = (DocumentFinanceYears) XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceYears), new Guid("42204635-1c7b-4a13-a206-c997726009da"));
            //ProcessFinanceDocumentSeries.DisableFiscalYear(documentFinanceYears);

            //DocumentFinanceYears documentFinanceYears = new DocumentFinanceYears();
            //documentFinanceYears.Save();
            //ProcessFinanceDocumentSeries.CreateDocumentFinanceYearSeries(documentFinanceYears);

            //logicpos.Utils.GetInputTextResponse getInputTextResponse = Utils.GetInputText(this, DialogFlags.Modal, "Acronym", "Acronym", "2015001", SettingsApp.RegexDocumentSeriesAcronym, true);
            //if (getInputTextResponse.ResponseType == ResponseType.Ok)
            //{
            //    _logger.Debug(string.Format("Acronym: [{0}]", getInputTextResponse.InputText));
            //}

            //Document Series Get And Confirm Acronym Dialog
            //logicpos.Utils.ResponseText result = TreeViewDocumentFinanceSeries.PosConfirmAcronymSeriesDialog(this, "2015S");
            //if (result.ResponseType == ResponseType.Ok)
            //{
            //    _logger.Debug(string.Format("Acronym: [{0}]", result.Text));
            //}

            //Get Current Active FinanceYear
            //DocumentFinanceYears currentDocumentFinanceYear = ProcessFinanceDocumentSeries.GetCurrentDocumentFinanceYear();
            //if (currentDocumentFinanceYear != null)
            //{
            //    bool result = TreeViewDocumentFinanceSeries.UICreateDocumentFinanceYearSeriesTerminal(this, currentDocumentFinanceYear);
            //    _logger.Debug(string.Format("Result: [{0}]", result));
            //}

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test PosDocumentFinancePrintDialog : Used to test StandAlone Dialog
            //Guid guidDocument = new Guid("7d08282c-d705-4f01-9047-36d6d65c15d7");
            //DocumentFinanceMaster documentFinanceMaster = (DocumentFinanceMaster)XPOHelper.GetXPGuidObjectFromSession(XPOSettings.Session, typeof(DocumentFinanceMaster), guidDocument);
            //if (documentFinanceMaster != null)
            //{
            //    documentFinanceMaster.DocumentType.PrintRequestMotive = true;
            //    PosDocumentFinancePrintDialog.PrintDialogResponse response = PosDocumentFinancePrintDialog.GetDocumentFinancePrintProperties(this, documentFinanceMaster);
            //    //_logger.Debug(string.Format("response: [{0}]", response));
            //}

            //DONT FORGET ToggleAction TEST DocumentMasterCreatePDF
            //Guid guidOid = new Guid("70c9cf6d-c212-4e3a-9c3a-641bb81da85c");
            //DocumentFinanceMaster documentFinanceMaster = (DocumentFinanceMaster)XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceMaster), guidOid);
            //string fileName = CustomReport.DocumentMasterCreatePDF(documentFinanceMaster);
            //_logger.Debug(string.Format("fileName: [{0}]", fileName));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //UtilsDays with Holidays

            /*
            List<DateTime> utilDays = FrameworkUtils.GetUtilDays(DateTime.Now.AddDays(-15));
            _logger.Debug(string.Format("GetUtilDays: [{0}]", utilDays.Count));
            UtilsDays Without Holidays
            utilDays = FrameworkUtils.GetUtilDays(DateTime.Now.AddDays(-15), true);
            _logger.Debug(string.Format("GetUtilDays: [{0}]", utilDays.Count));
            Count Holidays
            Dictionary<DateTime,bool> holidays = FrameworkUtils.GetHolidays();
            _logger.Debug(string.Format("Holidays: [{0}]", holidays.Count));
            Test Is Holiday
            _logger.Debug(string.Format("Message: [{0}]", FrameworkUtils.IsHoliday(new DateTime(2016, 4, 25))));
            _logger.Debug(string.Format("Message: [{0}]", FrameworkUtils.IsHoliday(new DateTime(2016, 7, 11))));
            _logger.Debug(string.Format("Message: [{0}]", FrameworkUtils.IsHoliday(new DateTime(2016, 8, 11))));

            DateTime result = FrameworkUtils.GetDateTimeBackUtilDays(DateTime.Now, 15, true);
            _logger.Debug(string.Format("GetDateTimeBackUtilDays: [{0}]", result.ToString(SettingsApp.DateFormat)));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Other Test to Get Values from GetDateTimeBackUtilDays and Count days since Day

            /*
            DateTime dateFilterFrom = FrameworkUtils.GetDateTimeBackUtilDays(FrameworkUtils.CurrentDateTimeAtomicMidnight(), 5, true);
            _logger.Debug(string.Format("dateFilterFrom: [{0}]", dateFilterFrom));

            int documentBackUtilDays = FrameworkUtils.GetUtilDays(new DateTime(2016, 1, 4), true).Count;
            _logger.Debug(string.Format("documentBackUtilDays: [{0}]", documentBackUtilDays));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test Permissions

            /*
            UserDetail userDetail = (UserDetail)XPOSettings.Session.GetObjectByKey(typeof(UserDetail), new Guid("090c5684-52ba-4d7a-8bc3-a00320ef503d"));
            userDetail.Profile.Permissions.Reload();
            GeneralSettings.LoggedUserPermissions = SharedUtils.GetUserPermissions(userDetail);
            bool BACKOFFICE_ACCESS = FrameworkUtils.HasPermissionTo("BACKOFFICE_ACCESS");
            _logger.Debug(string.Format("HasPermissionTo(BACKOFFICE_ACCESS) : [{0}]", BACKOFFICE_ACCESS));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test IsDocumentMasterChildOfDocumentType Source Document ::::

            /*
            //Guid documentFinanceMasterGuid = new Guid("2a9b69f4-b37c-4b69-8007-8ba368932842");//OR ORPVVEH17012016S001/1 > GR GRPVVEH17012016S001/1 > FT FTPVVEH17012016S001/4 > NC NCPVVEH17012016S001/1
            Guid documentFinanceMasterGuid = new Guid("d487fd9b-ca37-4e1e-9cd4-92b98d4a7cb2");
            //DocumentType
            List<Guid> documentFinanceTypeList = new List<Guid>();
            Guid documentFinanceTypeGuid1 = new Guid("95f6a472-1b12-43aa-a215-a4b406b18924");//GR
            Guid documentFinanceTypeGuid2 = new Guid("005ac531-31a1-44bb-9346-058f9c9ad01a");//OR
            Guid documentFinanceTypeGuid3 = new Guid("b8554d36-642a-4083-b608-8f1da35f0fec");//FC
            //documentFinanceTypeList.Add(documentFinanceTypeGuid1);
            documentFinanceTypeList.Add(documentFinanceTypeGuid2);
            //documentFinanceTypeList.Add(documentFinanceTypeGuid3);
            DocumentFinanceMaster documentFinanceMaster = (DocumentFinanceMaster)XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceMaster), documentFinanceMasterGuid);
            //From DocumentFinanceTypeList
            //bool result = FrameworkUtils.IsDocumentMasterChildOfDocumentType(documentFinanceMaster, documentFinanceTypeList);
            //_logger.Debug(string.Format("IsDocumentMasterChildOfDocumentType: [{0}]", result));
            //From SaftDocumentType
            bool wayBillMode = 
                true && 
                //SaftDocumentType
                (documentFinanceMaster.DocumentType.SaftDocumentType != SaftDocumentType.MovementOfGoods &&
                //Child Of SaftDocumentType
                ! FrameworkUtils.IsDocumentMasterChildOfDocumentType(documentFinanceMaster, SaftDocumentType.MovementOfGoods)
            );

            _logger.Debug(string.Format("wayBillMode: [{0}]", wayBillMode));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test ProcessFinanceDocumentToInvoice with SaftDocumentType.MovementOfGoods Parameters

            /*
            string notificationTypeSaftDocumentTypeMovementOfGoods = "80a03838-0937-4ae3-921f-75a1e358f7bf";
            int defaultBackDaysForInvoice = 5;
            SystemNotification systemNotification = FrameworkUtils.ProcessFinanceDocumentToInvoice(XPOSettings.Session, new Guid(notificationTypeSaftDocumentTypeMovementOfGoods), SaftDocumentType.MovementOfGoods, "(DocumentStatusStatus = 'N')", defaultBackDaysForInvoice);
            _logger.Debug(string.Format("Message: [{0}]", systemNotification.Message));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test CSVFileToDictionary

            //string filePath = SettingsApp.ProtectedFilesFileName;
            //Dictionary<string, string> files = FrameworkUtils.CSVFileToDictionary(filePath);
            //_logger.Debug(string.Format("Message: [{0}]", files));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test Protected files after BootStrap, Boot and Try to Change Or Delete File

            //string file = "Resources/Reports/UserReports/ReportDocumentFinance.frx";
            //bool result = GlobalApp.ProtectedFiles.IsValidFile(file);
            //_logger.Debug(string.Format("[{0}] IsValidFile: [{1}]", file, result));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test ProcessFinanceDocument.GetLastDocumentDateTime

            /*
            DateTime result = ProcessFinanceDocument.GetLastDocumentDateTime();
            _logger.Debug(string.Format("result: [{0}]", result));
            _logger.Debug(string.Format("result: [{0}]", (DateTime.Now < result)));

            DocumentFinanceYearSerieTerminal documentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(SettingsApp.XpoOidDocumentFinanceTypeInvoice);
            _logger.Debug(string.Format("Terminal: [{0}], Serie: [{1}], SerieTerminal: [{2}]", documentFinanceYearSerieTerminal.Terminal.Designation, documentFinanceYearSerieTerminal.Serie.Designation, documentFinanceYearSerieTerminal.Designation));
            _logger.Debug(string.Format("Terminal: [{0}], Serie: [{1}]", documentFinanceYearSerieTerminal.Terminal.Designation, documentFinanceYearSerieTerminal.Designation));
            _logger.Debug(string.Format("result: [{0}]", result.Date));
            _logger.Debug(string.Format("result: [{0}]", (DateTime.Now < result)));

            documentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(SettingsApp.XpoOidDocumentFinanceTypeCreditNote);
            result = ProcessFinanceDocument.GetLastDocumentDateTime(string.Format("DocumentSerie = '{0}'", documentFinanceYearSerieTerminal.Serie.Oid));
            _logger.Debug(string.Format("Terminal: [{0}], Serie: [{1}], SerieTerminal: [{2}]", documentFinanceYearSerieTerminal.Terminal.Designation, documentFinanceYearSerieTerminal.Serie.Designation, documentFinanceYearSerieTerminal.Designation));
            _logger.Debug(string.Format("result: [{0}]", result.Date));
            _logger.Debug(string.Format("result: [{0}]", (DateTime.Now < result)));
            
            documentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(SettingsApp.XpoOidDocumentFinanceTypeDebitNote);
            result = ProcessFinanceDocument.GetLastDocumentDateTime(string.Format("DocumentSerie = '{0}'", documentFinanceYearSerieTerminal.Serie.Oid));
            _logger.Debug(string.Format("Terminal: [{0}], Serie: [{1}], SerieTerminal: [{2}]", documentFinanceYearSerieTerminal.Terminal.Designation, documentFinanceYearSerieTerminal.Serie.Designation, documentFinanceYearSerieTerminal.Designation));
            _logger.Debug(string.Format("Date: [{0}]", result.Date));
            _logger.Debug(string.Format("result: [{0}]", (DateTime.Now < result)));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test ProcessFinanceDocumentValidation.ValidatePersistFinanceDocument
            //Required a Valid LoggedUser

            /*
            XPOSettings.LoggedUser = (UserDetail)XPOSettings.Session.GetObjectByKey(typeof(UserDetail), new Guid("090c5684-52ba-4d7a-8bc3-a00320ef503d"));
            //Get DocumentMaster Oid
            //SELECT DocumentMaster,COUNT(*) AS Count FROM fin_documentfinancedetail GROUP BY DocumentMaster ORDER BY COUNT(*) DESC;
            DocumentFinanceMaster documentFinanceMaster = (DocumentFinanceMaster)XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceMaster), new Guid("814e8065-bcc1-49f2-86c0-bdbaeaf40e41"));
            ArticleBag articleBag = ArticleBag.DocumentFinanceMasterToArticleBag(documentFinanceMaster);
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(SettingsApp.XpoOidDocumentFinanceTypeCreditNote, articleBag);
            //Change default DocumentDateTime
            processFinanceDocumentParameter.DocumentDateTime = FrameworkUtils.CurrentDateTimeAtomic().AddDays(-5);
            //Simulate ProcessFinanceDocument Test
            DateTime documentDateTime = (processFinanceDocumentParameter.DocumentDateTime != DateTime.MinValue) ? processFinanceDocumentParameter.DocumentDateTime : FrameworkUtils.CurrentDateTimeAtomic();
            _logger.Debug(string.Format("documentDateTime: [{0}]", documentDateTime));
            //Get ResponseType Result, Silence is dont have Errors
            ResponseType responseType = Utils.ShowMessageTouchCheckIfFinanceDocumentHasValiDocumentDate(this, processFinanceDocumentParameter);
            _logger.Debug(string.Format("responseType: [{0}]", responseType));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test ProcessFinanceDocumentValidation.FilterValidateMessages

            /*
            string search = "INFO_";
            List<string> searchList = new List<string> { "ERROR_", "WARNING_" };
            List<string> messageList = new List<string> 
            { 
                "ERROR_#1", "ERROR_#2", "ERROR_#3", 
                "WARNING_#1", "WARNING_#2", "WARNING_#3",
                "INFO_#1", "INFO_#2", "INFO_#3",
                "FIELD_#1", "FIELD_#2", "FIELD_#3",
                "DEBUG_#1", "DEBUG_#2", "DEBUG_#3"
            };
            _logger.Debug("INFO_");
            List<string> result = FrameworkUtils.FilterList(messageList, search);
            _logger.Debug("ERROR_, WARNING_");
            result = FrameworkUtils.FilterList(messageList, searchList);
            _logger.Debug("DEBUG_, WARNING_");
            result = FrameworkUtils.FilterList(messageList, new List<string> { "DEBUG_", "WARNING_" });
            _logger.Debug("FIELD_");
            result = FrameworkUtils.FilterList(messageList, "FIELD_");
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test PosSendMessageDialog
            /*
            //Prepare Dialog
            PosSendMessageDialog dialog = new PosSendMessageDialog(this, DialogFlags.DestroyWithParent | DialogFlags.Modal);

            //Call Dialog and Return Result
            ResponseSendMessageText resultResponse = new ResponseSendMessageText();
            try
            {
                resultResponse.ResponseType = (ResponseType)dialog.Run();
                resultResponse.Text = dialog.Value;
                resultResponse.Terminal = dialog.ValueTerminal;
                resultResponse.User = dialog.ValueUser;
                //return resultResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                //return resultResponse;
            }
            finally
            {
                dialog.Destroy();
            }
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test XPO Cache

            /*
            //Start UnitOfWork
            Guid documentFinanceMasterGuid = new Guid("c76b45b3-b56d-4dc0-91cd-48bad33cbe31");
            Guid documentFinanceMasterNewGuid = new Guid();

            using (UnitOfWork uowSession = new UnitOfWork())
            {
                try
                {
                    //INSERT
                    DocumentFinanceMaster documentFinanceMasterNew = new DocumentFinanceMaster(uowSession);
                    documentFinanceMasterNew.EntityName = "NEW";
                    //UPDATE
                    DocumentFinanceMaster documentFinanceMaster = (DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(DocumentFinanceMaster), documentFinanceMasterGuid);
                    _logger.Debug(string.Format("DataLayer: [{0}]", documentFinanceMaster.Session.DataLayer));
                    documentFinanceMaster.Printed = false;
                    documentFinanceMaster.EntityName = "false";
                    uowSession.CommitChanges();

                    documentFinanceMasterNewGuid = documentFinanceMasterNew.Oid;
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                    _logger.Error(ex.Message, ex);
                }
            }

            //documentFinanceMasterGuid = documentFinanceMasterNewGuid;

            //On Save Modify Right Values with Bad Values
            DocumentFinanceMaster documentFinanceMasterChange = (DocumentFinanceMaster)XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceMaster), documentFinanceMasterGuid);
            _logger.Debug(string.Format("DataLayer: [{0}]", documentFinanceMasterChange.Session.DataLayer));
            documentFinanceMasterChange.EntityLocality = "true";
            _logger.Debug(string.Format("Printed: [{0}], EntityName: [{1}]", 
                documentFinanceMasterChange.Printed, 
                documentFinanceMasterChange.EntityName
            ));
            //documentFinanceMasterChange.Save();

            DocumentFinanceMaster documentFinanceMasterCheck = (DocumentFinanceMaster)XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceMaster), documentFinanceMasterGuid);
            _logger.Debug(string.Format("DataLayer: [{0}]", documentFinanceMasterCheck.Session.DataLayer));
            _logger.Debug(string.Format("Printed: [{0}], EntityName: [{1}], EntityLocality: [{2}]", 
                documentFinanceMasterCheck.Printed, 
                documentFinanceMasterCheck.EntityName,
                documentFinanceMasterCheck.EntityLocality
            ));
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            // PosArticleStock Window : Input/Output Stock Window

            /*
            ProcessArticleStockParameter response =  PosArticleStockDialog.GetProcessArticleStockParameter(this);
            if (response != null)
            {
                _logger.Debug(string.Format("Message: [{0}]", response.Article.Designation));
                ProcessArticleStock.Add(ProcessArticleStockMode.In, response);
            }
            */

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Get BarCode Input

            //logicpos.Utils.ResponseText dialogResponse;
            //dialogResponse = Utils.GetInputText(this, DialogFlags.Modal, CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_barcode, string.Empty, SettingsApp.RegexInteger, true);
            //if (dialogResponse.ResponseType == ResponseType.Ok)
            //{
            //    _logger.Debug(String.Format("BarCode: [{0}]", dialogResponse.Text));
            //}

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test Country Code2

            //cfg_configurationcountry countryPT = (cfg_configurationcountry)XPOHelper.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcountry), new Guid("e7e8c325-a0d4-4908-b148-508ed750676a"));
            //cfg_configurationcountry countryAO = (cfg_configurationcountry)XPOHelper.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcountry), new Guid("9655510a-ff58-461e-9719-c037058f10ed"));
            //_logger.Debug(String.Format("countryPT: [{0}], [{1}]", countryPT.Designation, countryPT.Code2));
            //_logger.Debug(String.Format("countryAO: [{0}], [{1}]", countryAO.Designation, countryAO.Code2));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test PosLicenceDialog 

            //LicenceDetails licenceDetails = PosLicenceDialog.GetLicenseDetails(this, "92A4-67CE-A9C0-7503-B245-2228");
            //_logger.Debug(String.Format("Message: [{0}]", licenceDetails.LicenseName));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            // Test Thread Dialog 

            //Without Utils.ThreadStart
            //Thread thread = new Thread(new ThreadStart(Utils.ThreadRoutine));
            //GlobalApp.DialogThreadNotify = new ThreadNotify (new ReadyEvent (Utils.ThreadDialogReadyEvent));
            //thread.Start();
            //GlobalApp.DialogThreadWork = Utils.GetThreadDialog(this);
            //GlobalApp.DialogThreadWork.Run();

            //Without Utils.ThreadStart and Parameter
            //Thread thread = new Thread(() => Utils.ThreadRoutine(5000));
            //Utils.ThreadStart(this, thread);

            //Non Thread Call
            //DataBaseBackup.Backup(this); 
            //Thread Call
            //Thread thread = new Thread(() => DataBaseBackup.Backup(this));
            //GlobalApp.DialogThreadNotify = new ThreadNotify (new ReadyEvent (Utils.ThreadDialogReadyEvent));
            //thread.Start();
            //GlobalApp.DialogThreadWork = Utils.GetThreadDialog(this);
            //GlobalApp.DialogThreadWork.Run();

            //Thread Call
            //object value = null;
            //Window sourceWindow = this;
            //Thread thread = new Thread(() => value = DataBaseBackup.Backup(this));
            //Utils.ThreadStart(this, thread);

            //DataBaseBackup.Backup(this);

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test WebService

            //string endpointAddress = FrameworkUtils.GetEndpointAddress();
            //if (FrameworkUtils.IsWebServiceOnline(endpointAddress))
            //{
            //    try
            //    {
            //        //Init Client
            //        Service1Client serviceClient = new Service1Client();
            //        //GetData
            //        var result = serviceClient.GetData(14);
            //        _logger.Debug(string.Format("Test GetData Service Result: {0}", result));
            //        //SendDocuments
            //        var resultResultObject = serviceClient.SendDocument(new Guid("c708ad97-a5fb-46ee-b241-a73cb13242ac"));
            //        if (resultResultObject != null)
            //        {
            //            _logger.Debug("Test AT WebService ResultObject:");
            //            _logger.Debug(string.Format("ReturnCode: {0}", resultResultObject.ReturnCode));
            //            _logger.Debug(string.Format("ReturnMessage: {0}", resultResultObject.ReturnMessage));
            //            _logger.Debug(string.Format("ReturnRaw: {0}", resultResultObject.ReturnRaw));
            //        }
            //        else
            //        {
            //            _logger.Error(String.Format("Error null resultResultObject: [{0}]", resultResultObject));
            //        }

            //        // Always Close Client
            //        serviceClient.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.Error(ex.Message, ex);
            //    }
            //}
            //else
            //{
            //    _logger.Debug(string.Format("EndpointAddress OffLine, Please check URI: {0}", endpointAddress));
            //}

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test GetFields and Properties from Settings

            //Field
            //try
            //{
            //    object o1 = FrameworkUtils.GetFieldValueFromType(typeof(SettingsApp), "SaftCurrencyCode");
            //    object o2 = FrameworkUtils.GetFieldValueFromType(typeof(SettingsApp), "AppCompanyName");
            //    //Property
            //    object o3 = FrameworkUtils.GetPropertyValueFromType(typeof(SettingsApp), "FinanceRuleSimplifiedInvoiceMaxTotal");
            //    //Output
            //    _logger.Debug(string.Format("Message: [{0}]:[{1}]:[{2}]", o1.ToString(), o2.ToString(), o3.ToString()));
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error(ex.Message, ex);
            //}

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test InitPlataformParameters

            //PosEditCompanyDetails dialog = new PosEditCompanyDetails(this, DialogFlags.DestroyWithParent | DialogFlags.Modal);
            //ResponseType response = (ResponseType)dialog.Run();
            //dialog.Destroy();

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Test Encrypted Creating Objects outside of BO

            //try
            //{
            //    int size = 10;

            //    erp_customer customer = new erp_customer(XPOSettings.Session)
            //    {
            //        Name = Utils.GenerateRandomStringAlphaUpper(size),
            //        Address = Utils.GenerateRandomStringAlphaUpper(size),
            //        Locality = Utils.GenerateRandomStringAlphaUpper(size),
            //        ZipCode = Utils.GenerateRandomStringAlphaUpper(size),
            //        City = Utils.GenerateRandomStringAlphaUpper(size),
            //        DateOfBirth = Utils.GenerateRandomStringAlphaUpper(size),
            //        Phone = Utils.GenerateRandomStringAlphaUpper(size),
            //        Fax = Utils.GenerateRandomStringAlphaUpper(size),
            //        MobilePhone = Utils.GenerateRandomStringAlphaUpper(size),
            //        Email = Utils.GenerateRandomStringAlphaUpper(size),
            //        WebSite = Utils.GenerateRandomStringAlphaUpper(size),
            //        FiscalNumber = Utils.GenerateRandomStringAlphaUpper(size),
            //        CardNumber = Utils.GenerateRandomStringAlphaUpper(size)
            //    };

            //    // Required to Encrypt Properties Before Save DEPRECTATED now we have all inside XPGuidObject
            //    //customer.EncryptProperties();
            //    customer.Save();
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error(ex.Message, ex);
            //}

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            // Test XPGuidObjectAttribute[]

            //XPGuidObjectAttribute[] attr = Utils.GetXPGuidObjectAttributes(typeof(erp_customer));

            //Dictionary<string, PropertyInfo> attributes = XPGuidObject.GetXPGuidObjectAttributes(typeof(erp_customer), false);
            //foreach (var item in attributes)
            //{
            //    _logger.Debug($"[{item.Key}]=[{item.Value}]");
            //}
        }
    }
}
