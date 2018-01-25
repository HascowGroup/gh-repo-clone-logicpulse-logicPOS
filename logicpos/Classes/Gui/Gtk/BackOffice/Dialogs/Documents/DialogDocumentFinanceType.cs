﻿using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogDocumentFinanceType : BOBaseDialog
    {
        public DialogDocumentFinanceType(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_template);
            SetSizeRequest(400, 514);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(Resx.global_record_order, entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(Resx.global_record_code, entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(Resx.global_designation, entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(Resx.global_acronym, entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAcronym, _dataSourceRow, "Acronym", SettingsApp.RegexAlfa, true));

                //PrintCopies
                Entry entryPrintCopies = new Entry();
                BOWidgetBox boxPrintCopies = new BOWidgetBox(Resx.global_print_copies, entryPrintCopies);
                vboxTab1.PackStart(boxPrintCopies, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrintCopies, _dataSourceRow, "PrintCopies", SettingsApp.RegexPrintCopies, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrinters), (DataSourceRow as FIN_DocumentFinanceType).Printer, "Designation");
                BOWidgetBox boxPrinter = new BOWidgetBox(Resx.global_device_printer, xpoComboBoxPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, false));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrintersTemplates), (DataSourceRow as FIN_DocumentFinanceType).Template, "Designation");
                BOWidgetBox boxTemplate = new BOWidgetBox(Resx.global_configurationprintersTemplate, xpoComboBoxTemplate);
                vboxTab1.PackStart(boxTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplate, DataSourceRow, "Template", SettingsApp.RegexGuid, false));

                //PrintRequestConfirmation
                CheckButton checkButtonPrintRequestConfirmation = new CheckButton(Resx.global_print_request_confirmation);
                vboxTab1.PackStart(checkButtonPrintRequestConfirmation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonPrintRequestConfirmation, _dataSourceRow, "PrintRequestConfirmation"));

                //PrintOpenDrawer
                CheckButton checkButtonPrintOpenDrawer = new CheckButton(Resx.global_open_drawer);
                vboxTab1.PackStart(checkButtonPrintOpenDrawer, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonPrintOpenDrawer, _dataSourceRow, "PrintOpenDrawer"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(Resx.global_record_main_detail));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryDesignation.Sensitive = (_dialogMode == DialogMode.Insert);
                entryAcronym.Sensitive = (_dialogMode == DialogMode.Insert);
                xpoComboBoxTemplate.Sensitive = (_dialogMode == DialogMode.Insert);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}

