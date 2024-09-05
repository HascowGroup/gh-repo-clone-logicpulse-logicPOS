﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationVatExemptionReason : EditDialog
    {
        public DialogConfigurationVatExemptionReason(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_edit_configurationvatrate"));

            SetSizeRequest(500, 398);
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
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_acronym"), entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAcronym, Entity, "Acronym", LogicPOS.Utility.RegexUtils.RegexAcronym3Chars, true));

                //StandardApplicable
                Entry entryStandardApplicable = new Entry();
                BOWidgetBox boxStandardApplicable = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_description"), entryStandardApplicable);
                vboxTab1.PackStart(boxStandardApplicable, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxStandardApplicable, Entity, "StandardApplicable", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable/Enable Components
                bool enableEdit = (Entity.Oid != InvoiceSettings.XpoOidConfigurationVatExemptionReasonM99);
                entryOrd.Sensitive = enableEdit;
                entryCode.Sensitive = enableEdit;
                entryDesignation.Sensitive = enableEdit;
                entryAcronym.Sensitive = enableEdit;
                entryStandardApplicable.Sensitive = enableEdit;
                checkButtonDisabled.Sensitive = enableEdit;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}