﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPoleDisplay : BOBaseDialog
    {
        public DialogConfigurationPoleDisplay(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_edit_configurationpoledisplay"));

            if (logicpos.Utils.IsLinux) SetSizeRequest(500, 410);
            else SetSizeRequest(500, 490);
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

                // HBox For Vid and Pid
                HBox hbox1 = new HBox(true, _boxSpacing);
                // HBox For Vid and Pid
                HBox hbox2 = new HBox(true, _boxSpacing);

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SharedSettings.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SharedSettings.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SharedSettings.RegexAlfaNumericExtended, true));

                //VID
                Entry entryVID = new Entry();
                BOWidgetBox boxVID = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_vid"), entryVID);
                hbox1.PackStart(boxVID, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVID, _dataSourceRow, "VID", SharedSettings.RegexHardwareVidAndPid, true));

                //PID
                Entry entryPID = new Entry();
                BOWidgetBox boxPID = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_pid"), entryPID);
                hbox1.PackStart(boxPID, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPID, _dataSourceRow, "PID", SharedSettings.RegexHardwareVidAndPid, true));

                //EndPoint
                Entry entryEndPoint = new Entry();
                BOWidgetBox boxEndPoint = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_endpoint"), entryEndPoint);
                hbox1.PackStart(boxEndPoint, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxEndPoint, _dataSourceRow, "EndPoint", SharedSettings.RegexHardwareEndpoint, true));

                //COM
                Entry entryCOM = new Entry();
                BOWidgetBox boxCOM = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_com_port"), entryCOM);
                hbox1.PackStart(boxCOM, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCOM, _dataSourceRow, "COM", SharedSettings.RegexHardwarePortName, true));

                // Pack hboxVIDAndPid
                vboxTab1.PackStart(hbox1, false, false, 0);

                //CodeTable
                Entry entryCodeTable = new Entry();
                BOWidgetBox boxCodeTable = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_codetable"), entryCodeTable);
                hbox2.PackStart(boxCodeTable, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCodeTable, _dataSourceRow, "CodeTable", SharedSettings.RegexHardwareCodeTable, true));

                //DisplayCharactersPerLine
                Entry entryDisplayCharactersPerLine = new Entry();
                BOWidgetBox boxDisplayCharactersPerLine = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_number_of_characters_per_line"), entryDisplayCharactersPerLine);
                hbox2.PackStart(boxDisplayCharactersPerLine, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDisplayCharactersPerLine, _dataSourceRow, "DisplayCharactersPerLine", SharedSettings.RegexInteger, true));


                //GoToStandByInSeconds
                Entry entryGoToStandByInSeconds = new Entry();
                BOWidgetBox boxGoToStandByInSeconds = new BOWidgetBox(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_goto_stand_by_in_seconds"), entryGoToStandByInSeconds);
                hbox2.PackStart(boxGoToStandByInSeconds, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxGoToStandByInSeconds, _dataSourceRow, "GoToStandByInSeconds", SharedSettings.RegexInteger, true));

                // Pack hboxEndpointAndCodeTableAndGoToStandByInSeconds
                vboxTab1.PackStart(hbox2, false, false, 0);

                //StandByLine1
                Entry entryStandByLine1 = new Entry();
                BOWidgetBox boxStandByLine1 = new BOWidgetBox(string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_stand_by_line_no"), 1), entryStandByLine1);
                vboxTab1.PackStart(boxStandByLine1, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxStandByLine1, _dataSourceRow, "StandByLine1", SharedSettings.RegexAlfaNumericExtended, false));

                //StandByLine2
                Entry entryStandByLine2 = new Entry();
                BOWidgetBox boxStandByLine2 = new BOWidgetBox(string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_pole_display_stand_by_line_no"), 2), entryStandByLine2);
                vboxTab1.PackStart(boxStandByLine2, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxStandByLine2, _dataSourceRow, "StandByLine2", SharedSettings.RegexAlfaNumericExtended, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
