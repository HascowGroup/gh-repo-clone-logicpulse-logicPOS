﻿using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports.BOs;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterFinanceDocumentMaster : ThermalPrinterBaseFinanceTemplate
    {
        //Parameters Properties
        private FIN_DocumentFinanceMaster _documentMaster;
        //Business Objects
        private List<FRBODocumentFinanceMaster> _documentFinanceMasterList;
        private List<FRBODocumentFinanceDetail> _documentFinanceDetailList;
        private List<FRBODocumentFinanceMasterTotal> _documentFinanceMasterTotalList;

        public ThermalPrinterFinanceDocumentMaster(SYS_ConfigurationPrinters pPrinter, FIN_DocumentFinanceMaster pDocumentMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
            : base(pPrinter, pDocumentMaster.DocumentType, pCopyNames, pSecondCopy)
        {
            try
            {
                //Parameters
                _documentMaster = pDocumentMaster;

                //Init Fast Reports Business Objects (From FRBOHelper)
                ResultFRBODocumentFinanceMaster fRBOHelperResponseProcessReportFinanceDocument = FRBOHelper.GetFRBOFinanceDocument(_documentMaster.Oid);
                //Get FRBOs Lists 
                _documentFinanceMasterList = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List;
                _documentFinanceDetailList = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List[0].DocumentFinanceDetail;
                _documentFinanceMasterTotalList = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List[0].DocumentFinanceMasterTotal; ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call base PrintDocumentMaster();
                base.PrintDocumentMaster(_documentFinanceMasterList[0].DocumentTypeResourceString, _documentFinanceMasterList[0].DocumentNumber, _documentFinanceMasterList[0].DocumentDate);

                //Call 
                PrintDocumentMasterDocumentType();

                //Call base PrintCustomer();
                base.PrintCustomer(
                    _documentFinanceMasterList[0].EntityName,
                    _documentFinanceMasterList[0].EntityAddress,
                    _documentFinanceMasterList[0].EntityZipCode,
                    _documentFinanceMasterList[0].EntityCity,
                    _documentFinanceMasterList[0].EntityCountry,
                    _documentFinanceMasterList[0].EntityFiscalNumber
                );

                PrintDocumentDetails();
                PrintMasterTotals();
                PrintMasterTotalTax();
                PrintDocumenWayBillDetails();

                //Call base PrintDocumentPaymentDetails();
                base.PrintDocumentPaymentDetails(_documentFinanceMasterList[0].PaymentConditionDesignation, _documentFinanceMasterList[0].PaymentMethodDesignation, _documentFinanceMasterList[0].CurrencyAcronym);

                //Call base PrintNotes();
                if (_documentFinanceMasterList[0].Notes != null) base.PrintNotes(_documentFinanceMasterList[0].Notes.ToString());

                //Only Print if is in Portugal ex "Os artigos faturados...."
                //Call base PrintDocumentTypeFooterString();
                if (SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal)
                { 
                    base.PrintDocumentTypeFooterString(_documentFinanceMasterList[0].DocumentTypeResourceStringReport);
                }

                //Get Hash4Chars from Hash
                string hash4Chars = ProcessFinanceDocument.GenDocumentHash4Chars(_documentMaster.Hash);
                //Call Base CertificationText 
                base.PrintCertificationText(hash4Chars);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Print Specific DocumentType Details
        public void PrintDocumentMasterDocumentType()
        {
            //Set Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            string appOperationModeToken = GlobalFramework.Settings["appOperationModeToken"].ToLower();

            //ConferenceDocument : Show Table if in ConferenceDocument and in default AppMode
            if (_documentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument && appOperationModeToken == "default")
            {
                //Table|Order #2|Name/Zone
                string tableZone = string.Format("{0} : #{1}/{2}"
                    , Resx.ResourceManager.GetString(string.Format("global_table_appmode_{0}", appOperationModeToken))
                    , _documentMaster.SourceOrderMain.PlaceTable.Designation
                    , _documentMaster.SourceOrderMain.PlaceTable.Place.Designation
                );
                _thermalPrinterGeneric.WriteLine(tableZone);
                _thermalPrinterGeneric.LineFeed();
            }

            //Reset Align 
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //Loop Details
        public void PrintDocumentDetails()
        {
            try
            {
                List<TicketColumn> columns = new List<TicketColumn>();
                //columns.Add(new TicketColumn("Article", Resx.global_article_acronym, 0, TicketColumnsAlign.Right, typeof(string), "{0:0.00}"));
                columns.Add(new TicketColumn("VatRate", Resx.global_vat_rate, 6, TicketColumnsAlign.Right, typeof(decimal), "{0:00.00}"));
                columns.Add(new TicketColumn("Quantity", Resx.global_quantity_acronym, 8, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("UnitMeasure", Resx.global_unit_measure_acronym, 3, TicketColumnsAlign.Right));
                columns.Add(new TicketColumn("Price", Resx.global_price, 11, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("Discount", Resx.global_discount_acronym, 6, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                //columns.Add(new TicketColumn("TotalNet", Resx.global_totalnet_acronym, 9, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("TotalFinal", Resx.global_totalfinal_acronym, 0, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));//Dynamic

                //Prepare Table with Padding
                DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
                TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                //Print Table Headers
                ticketTable.Print(_thermalPrinterGeneric, paddingLeftFormat);

                //Print Items
                foreach (FRBODocumentFinanceDetail item in _documentFinanceDetailList)
                {
                    //Recreate/Reset Table for Item Details Loop
                    ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                    PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
                }

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Detail Row Block
        public void PrintDocumentDetail(TicketTable pTicketTable, FRBODocumentFinanceDetail pFinanceDetail, string pPaddingLeftFormat)
        {
            try
            {
                //Trim Data
                string designation = (pFinanceDetail.Designation.Length <= _maxCharsPerLineNormalBold)
                    ? pFinanceDetail.Designation
                    : pFinanceDetail.Designation.Substring(0, _maxCharsPerLineNormalBold)
                ;
                //Print Item Designation : Bold
                _thermalPrinterGeneric.WriteLine(designation, WriteLineTextMode.Bold);

                //Prepare ExemptionReason
                string exemptionReason = string.Empty;
                if (pFinanceDetail.VatExemptionReasonDesignation != null && pFinanceDetail.VatExemptionReasonDesignation != string.Empty)
                {
                    //Replaced Code : Remove Cut Exception Reason......its better not to cut VatExemptionReason.....
                    //exemptionReason = (pFinanceDetail.VatExemptionReasonDesignation.Length <= pTicketTable.TableWidth)
                    //    ? pFinanceDetail.VatExemptionReasonDesignation
                    //    : pFinanceDetail.VatExemptionReasonDesignation.Substring(0, pTicketTable.TableWidth)
                    //;
                    //Replace Code
                    exemptionReason = pFinanceDetail.VatExemptionReasonDesignation;
                    //Always Format Data, its appens in first line only
                    exemptionReason = string.Format(pPaddingLeftFormat, exemptionReason);
                }

                //Item Details
                DataRow dataRow = pTicketTable.NewRow();
                dataRow[0] = pFinanceDetail.Vat;
                dataRow[1] = pFinanceDetail.Quantity;
                dataRow[2] = pFinanceDetail.UnitMeasure;
                dataRow[3] = pFinanceDetail.Price * _documentFinanceMasterList[0].ExchangeRate;
                dataRow[4] = pFinanceDetail.Discount;
                //dataRow[5] = pFinanceDetail.TotalNet * _documentFinanceMasterList[0].ExchangeRate;
                dataRow[5] = pFinanceDetail.TotalFinal * _documentFinanceMasterList[0].ExchangeRate;

                //Add DataRow to Table, Ready for Print
                pTicketTable.Rows.Add(dataRow);
                //Print Table Rows
                pTicketTable.Print(_thermalPrinterGeneric, WriteLineTextMode.Normal, true, pPaddingLeftFormat);

                //VatExemptionReason
                if (!string.IsNullOrEmpty(exemptionReason)) _thermalPrinterGeneric.WriteLine(exemptionReason);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Totals, with TotalDelivery and TotalChange
        private void PrintMasterTotals()
        {
            try
            {
                //Init DataTable
                DataRow dataRow = null;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

                //Add Row : Discount
                if (_documentFinanceMasterList[0].Discount > 0.0m)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0} (%)", Resx.global_discount);
                    dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].Discount);
                    dataTable.Rows.Add(dataRow);
                }
                //Add Row : TotalGross
                dataRow = dataTable.NewRow();
                dataRow[0] = Resx.global_documentfinance_totalgross;
                //dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalGross * _documentFinanceMasterList[0].ExchangeRate);
                dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalNet * _documentFinanceMasterList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);
                //Add Row : TotalDiscount
                if (_documentFinanceMasterList[0].TotalDiscount > 0.0m)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = Resx.global_documentfinance_discount_customer;
                    dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalDiscount * _documentFinanceMasterList[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                }
                //Add Row : Discount PaymentCondition
                //WIP Always 0
                //Add Row : TotalTax
                dataRow = dataTable.NewRow();
                dataRow[0] = Resx.global_documentfinance_totaltax;
                dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalTax * _documentFinanceMasterList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);
                //Add Row : TotalFinal
                dataRow = dataTable.NewRow();
                dataRow[0] = Resx.global_documentfinance_totalfinal;
                dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalFinal * _documentFinanceMasterList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);

                //If Simplified Invoice, Payment Method MONEY and has Total Change, add it
                if (new Guid(_documentFinanceMasterList[0].DocumentType) == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    && _documentFinanceMasterList[0].PaymentMethodToken == "MONEY"
                    && _documentFinanceMasterList[0].TotalChange > 0
                    )
                {
                    //Blank Row, to separate from Main Totals
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Empty;
                    dataRow[1] = string.Empty;
                    dataTable.Rows.Add(dataRow);
                    //TotalDelivery
                    dataRow = dataTable.NewRow();
                    dataRow[0] = Resx.global_total_deliver;
                    dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalDelivery * _documentFinanceMasterList[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                    //TotalChange
                    dataRow = dataTable.NewRow();
                    dataRow[0] = Resx.global_total_change;
                    dataRow[1] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].TotalChange * _documentFinanceMasterList[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                }

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>();
                columns.Add(new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Left));
                columns.Add(new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Right));

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormalBold);

                //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (TotalChange(4) and Total(7))
                List<string> table = ticketTable.GetTable();
                WriteLineTextMode rowTextMode;
                //Dynamic Print All, some Rows ommited/bold(Table Header, TotalDocument,TotalChange)
                for (int i = 1; i < table.Count; i++)
                {
                    //Prepare TextMode Based on Row
                    rowTextMode = (i == 5 || i == 8) ? WriteLineTextMode.DoubleHeightBold : WriteLineTextMode.Bold;
                    //Print Row
                    _thermalPrinterGeneric.WriteLine(table[i], rowTextMode);
                }

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintMasterTotalTax()
        {
            try
            {
                //Init DataTable
                DataRow dataRow = null;
                DataTable dataTable = new DataTable();
                DataColumn dcDesignation = new DataColumn("Designation", typeof(String));
                DataColumn dcTax = new DataColumn("Tax", typeof(String));
                DataColumn dcTaxBase = new DataColumn("TaxBase", typeof(String));
                DataColumn dcTotal = new DataColumn("Total", typeof(String));
                dataTable.Columns.Add(dcDesignation);
                dataTable.Columns.Add(dcTax);
                dataTable.Columns.Add(dcTaxBase);
                dataTable.Columns.Add(dcTotal);

                foreach (FRBODocumentFinanceMasterTotal item in _documentFinanceMasterTotalList)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = item.Designation;
                    dataRow[1] = string.Format("{0}%", FrameworkUtils.DecimalToString(item.Value));
                    dataRow[2] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].ExchangeRate * item.TotalBase);
                    dataRow[3] = FrameworkUtils.DecimalToString(_documentFinanceMasterList[0].ExchangeRate * item.Total);
                    dataTable.Rows.Add(dataRow);
                }

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>();
                columns.Add(new TicketColumn("Designation", Resx.global_designation, 0, TicketColumnsAlign.Left));
                columns.Add(new TicketColumn("Tax", Resx.global_tax, 8, TicketColumnsAlign.Right));
                columns.Add(new TicketColumn("TotalBase", Resx.global_total_tax_base, 12, TicketColumnsAlign.Right));
                columns.Add(new TicketColumn("Total", Resx.global_total, 10, TicketColumnsAlign.Right));

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormal);
                //Print Table Buffer
                ticketTable.Print(_thermalPrinterGeneric);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PrintDocumenWayBillDetails()
        {
            try
            {
                //If Simplified Invoice, Payment Method MONEY and has Total Change, add it
                if (_documentFinanceMasterList[0].DocumentTypeWayBill)
                {
                    //WayBill Local Load
                    _thermalPrinterGeneric.WriteLine(Resx.global_documentfinance_waybill_local_load, WriteLineTextMode.Bold);
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1}", _documentFinanceMasterList[0].ShipFromAddressDetail, _documentFinanceMasterList[0].ShipFromCity));
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} [{2}]", _documentFinanceMasterList[0].ShipFromPostalCode, _documentFinanceMasterList[0].ShipFromRegion, _documentFinanceMasterList[0].ShipFromCountry));
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_from_delivery_date_report, FrameworkUtils.DateTimeToString(_documentFinanceMasterList[0].ShipFromDeliveryDate));
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_from_delivery_id_report, _documentFinanceMasterList[0].ShipFromDeliveryID);
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_from_warehouse_id_report, _documentFinanceMasterList[0].ShipFromWarehouseID);
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_from_location_id_report, _documentFinanceMasterList[0].ShipFromLocationID);

                    //Separate with Blank Line
                    _thermalPrinterGeneric.LineFeed();

                    //WayBill Local Download
                    _thermalPrinterGeneric.WriteLine(Resx.global_documentfinance_waybill_local_download, WriteLineTextMode.Bold);
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1}", _documentFinanceMasterList[0].ShipToAddressDetail, _documentFinanceMasterList[0].ShipToCity));
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} [{2}]", _documentFinanceMasterList[0].ShipToPostalCode, _documentFinanceMasterList[0].ShipToRegion, _documentFinanceMasterList[0].ShipToCountry));
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_to_delivery_date_report, FrameworkUtils.DateTimeToString(_documentFinanceMasterList[0].ShipToDeliveryDate));
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_to_delivery_id_report, _documentFinanceMasterList[0].ShipToDeliveryID);
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_to_warehouse_id_report, _documentFinanceMasterList[0].ShipToWarehouseID);
                    _thermalPrinterGeneric.WriteLine(Resx.global_ship_to_location_id_report, _documentFinanceMasterList[0].ShipToLocationID);

                    //Line Feed
                    _thermalPrinterGeneric.LineFeed();

                    //ATDocCodeID
                    if (!string.IsNullOrEmpty(_documentFinanceMasterList[0].ATDocCodeID))
                    {
                        //Set Align Center
                        _thermalPrinterGeneric.SetAlignCenter();

                        //WayBill Local Load
                        _thermalPrinterGeneric.WriteLine(Resx.global_at_atdoccodeid, WriteLineTextMode.Bold);
                        _thermalPrinterGeneric.WriteLine(_documentFinanceMasterList[0].ATDocCodeID, WriteLineTextMode.DoubleHeight);

                        //Reset Align 
                        _thermalPrinterGeneric.SetAlignLeft();

                        //Line Feed
                        _thermalPrinterGeneric.LineFeed();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
