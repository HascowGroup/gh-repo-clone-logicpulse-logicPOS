﻿using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using logicpos.shared.App;
using System;
using System.Reflection;
using LogicPOS.Settings.Extensions;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Generic Collection Result for FRBODocumentFinanceMaster

    public class ResultFRBODocumentFinanceMaster
    {
        public FRBOGenericCollection<FRBODocumentFinanceMasterView> DocumentFinanceMaster { get; set; }

        public ResultFRBODocumentFinanceMaster()
        {
        }

        public ResultFRBODocumentFinanceMaster(FRBOGenericCollection<FRBODocumentFinanceMasterView> pFRBODocumentFinanceMaster)
        {
            DocumentFinanceMaster = pFRBODocumentFinanceMaster;
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Generic Collection Result for FRBODocumentFinancePayment 

    public class ResultFRBODocumentFinancePayment
    {
        public FRBOGenericCollection<FRBODocumentFinancePaymentView> DocumentFinancePayment { get; set; }

        public ResultFRBODocumentFinancePayment()
        {
        }

        public ResultFRBODocumentFinancePayment(FRBOGenericCollection<FRBODocumentFinancePaymentView> pFRBODocumentFinancePayment)
        {
            DocumentFinancePayment = pFRBODocumentFinancePayment;
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class FRBOHelper
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Generate Fast Report Business Objects for ProcessReportFinanceDocument
        /// </summary>
        public static ResultFRBODocumentFinanceMaster GetFRBOFinanceDocument(Guid pDocumentFinanceMasterOid)
        {
            ResultFRBODocumentFinanceMaster result = new ResultFRBODocumentFinanceMaster();

            try
            {
                fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)DataLayerUtils.GetXPGuidObject(XPOSettings.Session, typeof(fin_documentfinancemaster), pDocumentFinanceMasterOid);

                bool retificationDocuments = (
                     documentFinanceMaster.DocumentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeCreditNote
                );
                /* IN009173 */
                bool isTransportDocument = (
                    documentFinanceMaster.DocumentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeTransportationGuide ||
                    documentFinanceMaster.DocumentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeDeliveryNote
                );

                string sqlFilter = string.Format("fmOid = '{0}'", documentFinanceMaster.Oid.ToString());

                //Prepare and Declare FRBOGenericCollections : Limit 1, One Record (Document Master), else we get All Details to (View)
                FRBOGenericCollection<FRBODocumentFinanceMasterView> gcDocumentFinanceMaster = new FRBOGenericCollection<FRBODocumentFinanceMasterView>(sqlFilter, 1);
                FRBOGenericCollection<FRBODocumentFinanceDetail> gcDocumentFinanceDetail;
                FRBOGenericCollection<FRBODocumentFinanceMasterTotalView> gcDocumentFinanceMasterTotal;
                /* IN005986 - code refactoring */
                FRBODocumentFinanceMasterView documentFinanceMasterView = gcDocumentFinanceMaster.List[0];

                /* IN009075 - for decrypt phase */
                bool customerDataHasBeenCleaned = false;

                //Override Default Values
                //If Simplified Invoice - Remove Customer Details (If System Country Equal to PT)
                if (DataLayerSettings.ConfigurationSystemCountry.Oid.Equals(SharedSettings.XpoOidConfigurationCountryPortugal)
                    || DataLayerSettings.ConfigurationSystemCountry.Oid.Equals(SharedSettings.XpoOidConfigurationCountryMozambique) /* IN005986 */
                    || DataLayerSettings.ConfigurationSystemCountry.Oid.Equals(SharedSettings.XpoOidConfigurationCountryAngola)) /* IN009230 - Angola is now added to this rule */
                {
                    /* IN009230 - now, only when Final Customer we have data cleaned */
                    //if (SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice.Equals(new Guid(documentFinanceMasterView.DocumentType)) 
                    //    || FrameworkUtils.GetFinalConsumerEntity().Oid.ToString() == documentFinanceMasterView.EntityOid) //Added
                    if (FinancialLibraryUtils.GetFinalConsumerEntity().Oid.ToString() == documentFinanceMasterView.EntityOid) //Added
                    {
                        documentFinanceMasterView.EntityName = string.Empty;
                        documentFinanceMasterView.EntityAddress = string.Empty;
                        documentFinanceMasterView.EntityZipCode = string.Empty;
                        documentFinanceMasterView.EntityCity = string.Empty;
                        documentFinanceMasterView.EntityCountry = string.Empty;
                        documentFinanceMasterView.EntityLocality = string.Empty;
                        /* IN009230 */
                        documentFinanceMasterView.EntityFiscalNumber = SharedSettings.FinanceFinalConsumerFiscalNumberDisplay;

                        customerDataHasBeenCleaned = true;
                    }
                    /* IN009230 - "If" content removed from here to the just before block of code */
                    //Detect if is FinalConsumer with FiscalNumber 999999990 and Hide it
                    //erp_customer customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);
                    //if (documentFinanceMasterView.EntityFiscalNumber == customer.FiscalNumber)
                    //{
                    //    documentFinanceMasterView.EntityFiscalNumber = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                    //}
                }
				//IN009347 Documentos PT - Alteração do Layout dos dados do Cliente #Lindote 2020
                /* IN009075 - decrypt phase */
                if (!customerDataHasBeenCleaned)
                {
                    documentFinanceMasterView.EntityName = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityName);
                    documentFinanceMasterView.EntityAddress = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityAddress);
                    documentFinanceMasterView.EntityZipCode = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityZipCode);
                    documentFinanceMasterView.EntityCity = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityCity);
                    documentFinanceMasterView.EntityLocality = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityLocality);
                    // documentFinanceMasterView.EntityCountry = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityCountry);
                    // EntityLocality???
                    /* IN009230 */
                    documentFinanceMasterView.EntityFiscalNumber = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityFiscalNumber);
                }

                /* IN009173 - add Parent document number to Notes field */
                if (isTransportDocument && documentFinanceMaster.DocumentParent != null)
                {
                    string notes = string.Format("{0}: {1}", resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_source_document"), documentFinanceMaster.DocumentParent.DocumentNumber);
                    if (!string.IsNullOrEmpty(documentFinanceMasterView.Notes)) notes += " | ";
                    notes += documentFinanceMasterView.Notes;
                    documentFinanceMasterView.Notes = notes;
                }
				//ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                documentFinanceMasterView.ATDocQRCode = documentFinanceMaster.ATDocQRCode;
                /* Add ATDocCodeID to Notes field */
                if (!string.IsNullOrEmpty(documentFinanceMasterView.ATDocCodeID))
                {
                    string notes = string.Format("{0}: {1}", resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_at_atdoccodeid"), documentFinanceMasterView.ATDocCodeID);
                    if (! string.IsNullOrEmpty(documentFinanceMasterView.Notes)) notes += " | "/*Environment.NewLine*/;
                    notes += documentFinanceMasterView.Notes;
                    documentFinanceMasterView.Notes = notes;
                }

                //Detect if is Retification Document (ND/NC) and add SourceDocument to Show on Notes
                if (retificationDocuments)
                {
                    //TK016319 - Certificação Angola - Alterações para teste da AGT
					//Notas de Credito
                    string notes = string.Format("{0}: {1}", resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_source_document"), documentFinanceMaster.DocumentParent.DocumentNumber);
                    if (SharedSettings.XpoOidConfigurationCountryAngola.Equals(DataLayerSettings.ConfigurationSystemCountry.Oid))
                    {
                        notes = string.Format("{0}: {1}", resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_source_document_NC_ND"), documentFinanceMaster.DocumentParent.DocumentNumber);
                    }                        
                    /* IN009252 - "Reason" added to "fin_documentfinancemaster.Notes" */
                    if (! string.IsNullOrEmpty(documentFinanceMasterView.Notes)) notes += Environment.NewLine; /* " | " */
                    notes += string.Format("{0}: {1}", resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_reason"), documentFinanceMasterView.Notes);
                    documentFinanceMasterView.Notes = notes;
                }

                //Render Child Bussiness Objects
                foreach (FRBODocumentFinanceMasterView documentMaster in gcDocumentFinanceMaster)
                {
                    //Get FinanceDetail 
                    gcDocumentFinanceDetail = new FRBOGenericCollection<FRBODocumentFinanceDetail>(string.Format("DocumentMaster = '{0}'", documentMaster.Oid), "Ord");
                    documentMaster.DocumentFinanceDetail = gcDocumentFinanceDetail.List;

                    //Get FinanceMasterTotals
                    gcDocumentFinanceMasterTotal = new FRBOGenericCollection<FRBODocumentFinanceMasterTotalView>(string.Format("fmtDocumentMaster = '{0}'", documentMaster.Oid), "Value");
                    documentMaster.DocumentFinanceMasterTotal = gcDocumentFinanceMasterTotal.List;
                }

                result.DocumentFinanceMaster = gcDocumentFinanceMaster;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        /// <summary>
        /// Generate Fast Report Business Objects for ProcessReportFinanceDocumentPayment
        /// </summary>
        public static ResultFRBODocumentFinancePayment GetFRBOFinancePayment(Guid pDocumentFinancePaymentOid)
        {
            ResultFRBODocumentFinancePayment result = new ResultFRBODocumentFinancePayment();

            try
            {
                fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)DataLayerUtils.GetXPGuidObject(XPOSettings.Session, typeof(fin_documentfinancepayment), pDocumentFinancePaymentOid);

                string sqlFilter = string.Format("fpaOid = '{0}'", pDocumentFinancePaymentOid.ToString());

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBODocumentFinancePaymentView> gcDocumentFinancePayment = new FRBOGenericCollection<FRBODocumentFinancePaymentView>(sqlFilter);
                FRBOGenericCollection<FRBODocumentFinancePaymentDocumentView> gcDocumentFinancePaymentDocument;

                //Decrypt Values
                if (! string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityName)) 
                    gcDocumentFinancePayment.List[0].EntityName = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityName).ToString();
                if (! string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityAddress)) 
                    gcDocumentFinancePayment.List[0].EntityAddress = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityAddress).ToString();
                if (! string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityZipCode)) 
                    gcDocumentFinancePayment.List[0].EntityZipCode = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityZipCode).ToString();
                if (! string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityCity)) 
                    gcDocumentFinancePayment.List[0].EntityCity = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityCity).ToString();
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityLocality))
                    gcDocumentFinancePayment.List[0].EntityLocality = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityLocality).ToString();
                if (! string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityFiscalNumber)) 
                    gcDocumentFinancePayment.List[0].EntityFiscalNumber = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityFiscalNumber).ToString();

                //If FinalConsumer - Clean Output Data
                if (gcDocumentFinancePayment.List[0].EntityFiscalNumber == SharedSettings.FinanceFinalConsumerFiscalNumber)
                {
                    gcDocumentFinancePayment.List[0].EntityName = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityAddress = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityZipCode = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityCity = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityLocality = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityFiscalNumber = SharedSettings.FinanceFinalConsumerFiscalNumberDisplay;
                }

                //Render Child Bussiness Objects
                foreach (FRBODocumentFinancePaymentView documentFinanceMasterPayment in gcDocumentFinancePayment)
                {
                    //Get FinanceDetail 
                    gcDocumentFinancePaymentDocument = new FRBOGenericCollection<FRBODocumentFinancePaymentDocumentView>(string.Format("fpaOid = '{0}'", documentFinanceMasterPayment.Oid), "ftpCode, fmaDocumentNumber");
                    documentFinanceMasterPayment.DocumentFinancePaymentDocument = gcDocumentFinancePaymentDocument.List;
                }

                result.DocumentFinancePayment = gcDocumentFinancePayment;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
