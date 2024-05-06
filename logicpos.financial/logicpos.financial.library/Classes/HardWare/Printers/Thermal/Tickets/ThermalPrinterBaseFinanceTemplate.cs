﻿using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public abstract class ThermalPrinterBaseFinanceTemplate : ThermalPrinterBaseTemplate
    {
        protected fin_documentfinancetype _documentType;
        protected List<int> _copyNames;
        protected string[] _copyNamesArray;
        protected bool _secondCopy;
        //Used in Child Documents
        protected string _copyName = string.Empty;
        protected int _ticketTablePaddingLeftLength = 2;

        public ThermalPrinterBaseFinanceTemplate(sys_configurationprinters pPrinter, fin_documentfinancetype pDocumentType, List<int> pCopyNames)
            : this(pPrinter, pDocumentType, pCopyNames, false)
        {
        }

        public ThermalPrinterBaseFinanceTemplate(sys_configurationprinters pPrinter, fin_documentfinancetype pDocumentType, List<int> pCopyNames, bool pSecondCopy)
            : base(pPrinter, SharedSettings.PrinterThermalImageCompanyLogo)
        {
            //Assign Parameter Properties
            _documentType = pDocumentType;
            _copyNames = pCopyNames;
            _secondCopy = pSecondCopy;

            //Generate CopyNamesArray (Original, Duplicate,...)
            if (_copyNames != null)
            {
                _copyNamesArray = CustomReport.CopyNames(pCopyNames);
            }
        }

        //Override Parent Template
        public override bool Print()
        {
            bool result;
            try
            {
                for (int i = 0; i < _copyNames.Count; i++)
                {
                    //Call Base Template PrintHeader
                    PrintHeader();
                    //PrintExtendedHeader
                    PrintExtendedHeader();

                    //Get CopyName Position, ex 0[Original], 4[Quadriplicate], we cant use I, else 0[Original], 1[Duplicate]
                    int copyNameIndex = _copyNames[i] + 1;
                    //Overrided by Child Classes
                    _copyName = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), string.Format("global_print_copy_title{0}", copyNameIndex));
                    if (_secondCopy && i < 1) _copyName = string.Format("{0}/{1}", _copyName, CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_print_second_print"));
                    //_logger.Debug(String.Format("copyName: [{0}], copyNameIndex: [{1}]", _copyName, copyNameIndex));

                    //Call Child Content (Overrided)
                    PrintContent();

                    //PrintFooterExtended
                    PrintFooterExtended();

                    //Call Base Template PrintFooter
                    PrintFooter();
                }

                //End Job
                PrintBuffer();

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Debug("override bool Print() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }

            return result;
        }

        //Finance Extended Header
        protected void PrintExtendedHeader()
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignLeft();/* IN009055 */

            //Extended Header
            _thermalPrinterGeneric.WriteLine(string.Format("{0}", _customVars["COMPANY_ADDRESS"]));
            _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} - {2}", _customVars["COMPANY_POSTALCODE"], _customVars["COMPANY_CITY"], _customVars["COMPANY_COUNTRY"]));
			/* IN009055 block */
            _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "prefparam_company_telephone"), _customVars["COMPANY_TELEPHONE"]);
            //_thermalPrinterGeneric.WriteLine(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_mobile_phone, _customVars["COMPANY_MOBILEPHONE"]);
            //_thermalPrinterGeneric.WriteLine(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_fax, _customVars["COMPANY_FAX"]);
            //_thermalPrinterGeneric.WriteLine(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_email"), _customVars["COMPANY_EMAIL"]);
            _thermalPrinterGeneric.WriteLine(_customVars["COMPANY_WEBSITE"], false); /* IN009211 */
            _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "prefparam_company_fiscalnumber"), _customVars["COMPANY_FISCALNUMBER"]);
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            //_thermalPrinterGeneric.SetAlignLeft(); /* IN009055 */
        }

        //Child Shared PrintDocumentMaster
        protected void PrintDocumentMaster(string pDocumentTypeResourceString, string pDocumentID, string pDocumentDateTime)
        {
            //Call Base PrintTitle()
            PrintTitles(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), pDocumentTypeResourceString), pDocumentID);

            //Set Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Copy Names + Document Date
            _thermalPrinterGeneric.WriteLine(_copyName);
            _thermalPrinterGeneric.WriteLine(pDocumentDateTime);
            _thermalPrinterGeneric.LineFeed();

            //Reset Align 
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //Child Shared PrintCustomer
        protected void PrintCustomer(string pName, string pAddress, string pZipCode, string pCity, string pCountry, string pFiscalNumber)
        {
			/* IN009055 block */
            string fiscalNumber = pFiscalNumber;
            string name = pName;

            /* IN009055: pFiscalNumber real value is overwritten by GetFRBOFinancePayment(Guid pDocumentFinancePaymentOid) method */
            if (SharedSettings.FinanceFinalConsumerFiscalNumberDisplay.Equals(pFiscalNumber) || SharedSettings.FinanceFinalConsumerFiscalNumber.Equals(pFiscalNumber))
            {
                //fiscalNumber = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                fiscalNumber = string.Empty; /* show the Fical Number display value is not necessary */
                name = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_final_consumer");
            }

			/* IN009055 block - begin */
            _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_customer"), name, false);
            _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_address"), pAddress, false);

            string addressDetails = pCountry;

            if (!string.IsNullOrEmpty(pZipCode) && !string.IsNullOrEmpty(pCity))
            {
                addressDetails = string.Format("{0} {1} - {2}", pZipCode, pCity, pCountry);
            }
            else if (!string.IsNullOrEmpty(pZipCode))
            {
                addressDetails = string.Format("{0} - {1}", pZipCode, pCountry);
            }
            else if (!string.IsNullOrEmpty(pCity))
            {
                addressDetails = string.Format("{0} - {1}", pCity, pCountry);
            }
            _thermalPrinterGeneric.WriteLine(addressDetails, false); /* When FS, no details */
            _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_fiscal_number"), fiscalNumber, false); /* Do not print Fiscal Number when empty */
            /* IN009055  block - end */
            _thermalPrinterGeneric.LineFeed();
        }

        public void PrintFooterExtended()
        {
            if (_customVars["TICKET_FOOTER_LINE1"] != string.Empty || _customVars["TICKET_FOOTER_LINE1"] != string.Empty)
            {
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                if (_customVars["TICKET_FOOTER_LINE1"] != string.Empty) _thermalPrinterGeneric.WriteLine(_customVars["TICKET_FOOTER_LINE1"]);
                if (_customVars["TICKET_FOOTER_LINE2"] != string.Empty) _thermalPrinterGeneric.WriteLine(_customVars["TICKET_FOOTER_LINE2"]);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();
            }
        }

        //Chield Shared PrintCustomer
        protected void PrintDocumentPaymentDetails(string pPaymentMethod, string pCurrency)
        {
            PrintDocumentPaymentDetails(string.Empty, pPaymentMethod, pCurrency);
        }

        protected void PrintDocumentPaymentDetails(string pPaymentCondition, string pPaymentMethod, string pCurrency)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

            //Init DataTable
            DataRow dataRow;
            //Add Row : PaymentConditions
            if (!string.IsNullOrEmpty(pPaymentCondition))
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_payment_conditions");
                dataRow[1] = pPaymentCondition;
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : PaymentMethod
            if (!string.IsNullOrEmpty(pPaymentMethod))
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_payment_method_field"); /* IN009055 */
                dataRow[1] = pPaymentMethod;
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : Currency
            dataRow = dataTable.NewRow();
            dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_currency_field"); /* IN009055 */
            dataRow[1] = pCurrency;
            dataTable.Rows.Add(dataRow);

            //Configure Ticket Column Properties
            List<TicketColumn> columns = new List<TicketColumn>
            {
                new TicketColumn("Label", "", _thermalPrinterGeneric.MaxCharsPerLineNormal / 2, TicketColumnsAlign.Right),
                new TicketColumn("Value", "", _thermalPrinterGeneric.MaxCharsPerLineNormal / 2, TicketColumnsAlign.Left)
            };

            //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
            TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormal);

            //Create Table Buffer, With Bigger TextMode
            ticketTable.Print(_thermalPrinterGeneric, true);

            //Line Feed
            _thermalPrinterGeneric.LineFeed();
        }

        //Used On Child Finance Documents - ResourceStringReport
        protected void PrintDocumentTypeFooterString(string pDocumentTypeMessage)
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Differ from Payments and Other Document Types
            string message = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), pDocumentTypeMessage);
            if (pDocumentTypeMessage != string.Empty && message != null) _thermalPrinterGeneric.WriteLine(message);

            //Line Feed
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();
        }
		//ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        //Print QRCode
        protected void PrintQRCode(string QRCode)
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Convert ASCII/Decimal
            string ESC = Convert.ToString((char)27);
            string GS = Convert.ToString((char)29);
            string center = ESC + "a" + (char)1; //align center
            //string left = ESC + "a" + (char)0; //align left
            string bold_on = ESC + "E" + (char)1; //turn on bold mode
            string bold_off = ESC + "E" + (char)0; //turn off bold mode
            string cut = ESC + "d" + (char)1 + GS + "V" + (char)66; //add 1 extra line before partial cut
            string initp = ESC + (char)64; //initialize printer
            string buffer = ""; //store all the data that want to be printed
            string QrData = QRCode; //data to be print in QR code
            Encoding m_encoding = Encoding.GetEncoding("UTF-8"); //set encoding for QRCode
            int store_len = (QrData).Length + 3;
            byte store_pL = (byte)(store_len % 256);
            byte store_pH = (byte)(store_len / 256);
            buffer += initp; //initialize printer
            buffer += center;
            buffer += m_encoding.GetString(new byte[] { 29, 40, 107, 4, 0, 49, 65, 50, 0 });
            buffer += m_encoding.GetString(new byte[] { 29, 40, 107, 3, 0, 49, 67, 8 });
            buffer += m_encoding.GetString(new byte[] { 29, 40, 107, 3, 0, 49, 69, 48 });
            buffer += m_encoding.GetString(new byte[] { 29, 40, 107, store_pL, store_pH, 49, 80, 48 });
            buffer += QrData;
            buffer += m_encoding.GetString(new byte[] { 29, 40, 107, 3, 0, 49, 81, 48 });
            //Cut Receipt
            //buffer += cut + initp;

            //Send to Printer

            _thermalPrinterGeneric.WriteLine(buffer);

            //Line Feed
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            //_thermalPrinterGeneric.SetAlignLeft();
        }

        protected void TestBarcode(string QrCode)
        {
            //ThermalPrinter.BarcodeType myType = ThermalPrinter.BarcodeType.ean13;
            string myData = QrCode;
            //_thermalPrinterGeneric.Reset();

            //printer.SetBarcodeLeftSpace(10);

            _thermalPrinterGeneric.SelectFontHRIBarcode(0);
            _thermalPrinterGeneric.SelectPrintingPositionHRIBarcode(2);

            _thermalPrinterGeneric.SetAlignCenter();

            _thermalPrinterGeneric.SetBarcodeWidth(4);
            _thermalPrinterGeneric.PrintBarcode(printer.generic.ThermalPrinter.BarcodeType.qrcode, myData);
            _thermalPrinterGeneric.SetBarcodeWidth(2);
            _thermalPrinterGeneric.LineFeed();
            _thermalPrinterGeneric.LineFeed();
            _thermalPrinterGeneric.LineFeed();
            _thermalPrinterGeneric.LineFeed();
            //_thermalPrinterGeneric.Cut(false);
        }

        protected void PrintQRCodeImage(System.Drawing.Bitmap bitmap)
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Convert ASCII/Decimal
            string ESC = Convert.ToString((char)27);
            string GS = Convert.ToString((char)29);
            string center = ESC + "a" + (char)1; //align center

            _thermalPrinterGeneric.PrintImage(@"temp/qrcode.Bmp", true);
       
            //Line Feed
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();

        }

        protected void PrintCertificationText()
        {
            PrintCertificationText(string.Empty);
        }

        //Used On Child Finance Documents
        protected void PrintCertificationText(string pHash4Chars)
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            /* IN009211 */
            string copyRightText = string.Format(
                CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_copyright") + " {0}",
                SharedSettings.SaftProductID
            );

            string certificationText;

            //Write Certification,CopyRight and License Text 
            if (SharedSettings.XpoOidConfigurationCountryPortugal.Equals(DataLayerSettings.ConfigurationSystemCountry.Oid))
            {
                //All Finance Documents use Processed, else Payments that use Emmited 
                string prefix = (_documentType.SaftDocumentType == SaftDocumentType.Payments)
                    ? CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_emitted")
                    : CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_processed")
                ;
                //Processed|Emitted with certified Software Nº {0}/AT
                certificationText = string.Format(
                    CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification"),
                    prefix,
                    SharedSettings.SaftSoftwareCertificateNumber
                );

                //Add Hash Validation if Defined (In DocumentFinance Only)
                if (pHash4Chars != string.Empty)
                {
                    certificationText = string.Format("{0} - {1}", pHash4Chars, certificationText);
                }
            }
            /* IN005975 and IN005979 for Mozambique deployment */
            else if (SharedSettings.XpoOidConfigurationCountryMozambique.Equals(DataLayerSettings.ConfigurationSystemCountry.Oid))
            {/* IN009055 - related to IN006047 */
				/* {Processado por computador} || Autorização da Autoridade Tributária: {DAFM1 - 0198 / 2018} */
                certificationText = string.Format(
                    CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_short"),
                    CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_moz_tax_authority_cert_number") + "\n" + 
                    CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_processed")
                 );
            }
			//TK016268 Angola - Certificação 
            else if (SharedSettings.XpoOidConfigurationCountryAngola.Equals(DataLayerSettings.ConfigurationSystemCountry.Oid))
            {
                //All Finance Documents use Processed, else Payments that use Emmited 
                string prefix = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_processed"); ;
                //Current Year
                string localDate = DateTime.Now.Year.ToString();
                //Processed|Emitted with certified Software Nº {0}/AT
                certificationText = string.Format(
                    CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_ao"),
                    prefix,
                    SharedSettings.SaftSoftwareCertificateNumberAO,
                    SharedSettings.SaftProductIDAO,
                    localDate);  
            }

            else {
				/* All other countries: "Processado por computador" */
                certificationText = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_report_overlay_software_certification_processed");
            }

            _thermalPrinterGeneric.WriteLine(certificationText, WriteLineTextMode.Small);

            _thermalPrinterGeneric.WriteLine(copyRightText, WriteLineTextMode.Small);

            /* IN009211 - it was printing empty label */
            if (!string.IsNullOrEmpty(LogicPOS.Settings.LicenseSettings.LicenseCompany))
            {
                string licenseText = string.Format(
                    CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_licensed_to"),
                    LogicPOS.Settings.LicenseSettings.LicenseCompany
            );
                _thermalPrinterGeneric.WriteLine(licenseText, WriteLineTextMode.Small);
            }

            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //UTILS
        public string GetQrCode(System.Drawing.Bitmap QRCode)
        {
            //int widthBMP = 113;
            //int heightBMP = 113;
            //var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            //var bmp = new System.Drawing.Bitmap((int)widthBMP, (int)heightBMP);
            //var graph = System.Drawing.Graphics.FromImage(bmp);

            //float scale = Math.Min(widthBMP / QRCode.Width, heightBMP / QRCode.Height);

            //var scaleWidth = (int)(QRCode.Width * scale);
            //var scaleHeight = (int)(QRCode.Height * scale);

            //graph.FillRectangle(brush, new System.Drawing.RectangleF(0, 0, widthBMP, heightBMP));
            //graph.DrawImage(QRCode, ((int)widthBMP - scaleWidth) / 2, ((int)heightBMP - scaleHeight) / 2, scaleWidth, scaleHeight);

            BitmapData data = GetBitmapData(QRCode);
            System.Collections.BitArray dots = data.Dots;
            byte[] width = BitConverter.GetBytes(data.Width);

            int offset = 0;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(stream);

            bw.Write((char)0x1B);
            bw.Write('@');

            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)24);

            while (offset < data.Height)
            {
                bw.Write((char)0x1B);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;
                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;
                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }
                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        bw.Write(slice);
                    }
                }
                offset += 24;
                bw.Write((char)0x0A);
            }
            // Restore the line spacing to the default of 30 dots.
            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)30);

            bw.Flush();
            byte[] bytes = stream.ToArray();
            return Encoding.Default.GetString(bytes);
        }

        public BitmapData GetBitmapData(System.Drawing.Bitmap QRCode)
        {
            using (var bitmap = QRCode)
            {
                var threshold = 127;
                var index = 0;
                var dimensions = bitmap.Width * bitmap.Height;
                var dots = new System.Collections.BitArray(dimensions);

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = bitmap.Height,
                    Width = bitmap.Width
                };
            }
        }

        public class BitmapData
        {
            public System.Collections.BitArray Dots
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public int Width
            {
                get;
                set;
            }
        }
    }
}
