﻿using DevExpress.Xpo;
using System;
using System.Data;
using System.Text;

namespace logicpos.datalayer.DataLayer.Xpo
{
    //Todo : Change File to Work with Encrypted Attributed
    // 1. change string fEntityCountry; > to [Size(50)] / EntityCountry varchar(50) DEFAULT NULL (All Dbs)
    // 2. uncomment InitEncryptedAttributes<fin_documentfinancemaster>(); in Constructor and OnAfterConstruction
    // 3. uncomment //[XPGuidObject(Encrypted = true)]
    // 4. saft and print documents are with encrypted values from db, require extra work

    [DeferredDeletion(false)]
    public class fin_documentfinancemaster : XPGuidObject
    {
        public fin_documentfinancemaster() : base() { }
        public fin_documentfinancemaster(Session session) : base(session)
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist
            InitEncryptedAttributes<fin_documentfinancemaster>();
        }

        protected override void OnAfterConstruction()
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist - Required for New Records to have InitEncryptedAttributes else it Triggers Exception on Save
            InitEncryptedAttributes<fin_documentfinancemaster>();
        }

        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        /*BOF: SAF-T(PT)*/
        //used for InvoiceNo (SalesInvoices) DocumentNumber(MovementOfGoods), DocumentNumber(WorkingDocuments)
        private string fDocumentNumber;
        [Size(50)]
        [Indexed(Unique = true)]
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        //N - Normal
        //S - Auto Faturação
        //A - Documento Anulado
        //R - Documento de Resumo doutros documentos criados noutras aplicações e gerado nesta aplicaçãoi
        //F - Documento Faturado
        private string fDocumentStatusStatus;
        [Size(1)]
        public string DocumentStatusStatus
        {
            get { return fDocumentStatusStatus; }
            set { SetPropertyValue<string>("DocumentStatusStatus", ref fDocumentStatusStatus, value); }
        }

        private string fDocumentStatusDate;
        [Size(19)]
        public string DocumentStatusDate
        {
            get { return fDocumentStatusDate; }
            set { SetPropertyValue<string>("DocumentStatusDate", ref fDocumentStatusDate, value); }
        }

        private string fDocumentStatusReason;
        [Size(50)]
        public string DocumentStatusReason
        {
            get { return fDocumentStatusReason; }
            set { SetPropertyValue<string>("DocumentStatusReason", ref fDocumentStatusReason, value); }
        }

        //Used to Store 30Chars Codes for SAF-T
        private string fDocumentStatusUser;
        [Size(30)]
        public string DocumentStatusUser
        {
            get { return fDocumentStatusUser; }
            set { SetPropertyValue<string>("DocumentStatusUser", ref fDocumentStatusUser, value); }
        }

        //P - Documento Produzido na Aplicação
        //I - Documento Integrado e produzido noutra aplicação 
        //M - Documento proveniente de recuperação ou de emissão manual
        private string fSourceBilling;
        [Size(1)]
        public string SourceBilling
        {
            get { return fSourceBilling; }
            set { SetPropertyValue<string>("SourceBilling", ref fSourceBilling, value); }
        }

        private string fHash;
        [Size(172)]
        public string Hash
        {
            get { return fHash; }
            set { SetPropertyValue<string>("Hash", ref fHash, value); }
        }

        private string fHashControl;
        [Size(40)]
        public string HashControl
        {
            get { return fHashControl; }
            set { SetPropertyValue<string>("HashControl", ref fHashControl, value); }
        }

        //used for InvoiceDate (SalesInvoices) MovementDate(MovementOfGoods), WorkDate(WorkingDocuments)
        private string fDocumentDate;
        [Size(19)]
        public string DocumentDate
        {
            get { return fDocumentDate; }
            set { SetPropertyValue<string>("DocumentDate", ref fDocumentDate, value); }
        }

        private int fSelfBillingIndicator;
        public int SelfBillingIndicator
        {
            get { return fSelfBillingIndicator; }
            set { SetPropertyValue<int>("SelfBillingIndicator", ref fSelfBillingIndicator, value); }
        }

        private int fCashVATSchemeIndicator;
        public int CashVatSchemeIndicator
        {
            get { return fCashVATSchemeIndicator; }
            set { SetPropertyValue<int>("CashVATSchemeIndicator", ref fCashVATSchemeIndicator, value); }
        }

        private int fThirdPartiesBillingIndicator;
        public int ThirdPartiesBillingIndicator
        {
            get { return fThirdPartiesBillingIndicator; }
            set { SetPropertyValue<int>("ThirdPartiesBillingIndicator", ref fThirdPartiesBillingIndicator, value); }
        }

        //Used to Store 30Chars Codes for SAF-T
        private string fDocumentCreatorUser;
        [Size(30)]
        public string DocumentCreatorUser
        {
            get { return fDocumentCreatorUser; }
            set { SetPropertyValue<string>("DocumentCreatorUser", ref fDocumentCreatorUser, value); }
        }

        private string fEACCode;
        [Size(5)]
        public string EACCode
        {
            get { return fEACCode; }
            set { SetPropertyValue<string>("EACCode", ref fEACCode, value); }
        }

        private string fSystemEntryDate;
        [Size(50)]
        public string SystemEntryDate
        {
            get { return fSystemEntryDate; }
            set { SetPropertyValue<string>("SystemEntryDate", ref fSystemEntryDate, value); }
        }

        private string fTransactionID;
        [Size(70)]
        public string TransactionID
        {
            get { return fTransactionID; }
            set { SetPropertyValue<string>("TransactionID", ref fTransactionID, value); }
        }

        //MovementOfGoods: ShipTo
        private string fShipToDeliveryID;
        [Size(255)]
        public string ShipToDeliveryID
        {
            get { return fShipToDeliveryID; }
            set { SetPropertyValue<string>("ShipToDeliveryID", ref fShipToDeliveryID, value); }
        }

        private DateTime fShipToDeliveryDate;
        public DateTime ShipToDeliveryDate
        {
            get { return fShipToDeliveryDate; }
            set { SetPropertyValue<DateTime>("ShipToDeliveryDate", ref fShipToDeliveryDate, value); }
        }

        private string fShipToWarehouseID;
        [Size(50)]
        public string ShipToWarehouseID
        {
            get { return fShipToWarehouseID; }
            set { SetPropertyValue<string>("ShipToWarehouseID", ref fShipToWarehouseID, value); }
        }

        private string fShipToLocationID;
        [Size(30)]
        public string ShipToLocationID
        {
            get { return fShipToLocationID; }
            set { SetPropertyValue<string>("ShipToLocationID", ref fShipToLocationID, value); }
        }

        private string fShipToBuildingNumber;
        [Size(10)]
        public string ShipToBuildingNumber
        {
            get { return fShipToBuildingNumber; }
            set { SetPropertyValue<string>("ShipToBuildingNumber", ref fShipToBuildingNumber, value); }
        }

        private string fShipToStreetName;
        [Size(90)]
        public string ShipToStreetName
        {
            get { return fShipToStreetName; }
            set { SetPropertyValue<string>("ShipToStreetName", ref fShipToStreetName, value); }
        }

        private string fShipToAddressDetail;
        [Size(100)]
        public string ShipToAddressDetail
        {
            get { return fShipToAddressDetail; }
            set { SetPropertyValue<string>("ShipToAddressDetail", ref fShipToAddressDetail, value); }
        }

        private string fShipToCity;
        [Size(50)]
        public string ShipToCity
        {
            get { return fShipToCity; }
            set { SetPropertyValue<string>("ShipToCity", ref fShipToCity, value); }
        }

        private string fShipToPostalCode;
        [Size(20)]
        public string ShipToPostalCode
        {
            get { return fShipToPostalCode; }
            set { SetPropertyValue<string>("ShipToPostalCode", ref fShipToPostalCode, value); }
        }

        private string fShipToRegion;
        [Size(50)]
        public string ShipToRegion
        {
            get { return fShipToRegion; }
            set { SetPropertyValue<string>("ShipToRegion", ref fShipToRegion, value); }
        }

        private string fShipToCountry;
        [Size(5)]
        public string ShipToCountry
        {
            get { return fShipToCountry; }
            set { SetPropertyValue<string>("ShipToCountry", ref fShipToCountry, value); }
        }

        //MovementOfGoods: ShipFrom
        private string fShipFromDeliveryID;
        [Size(255)]
        public string ShipFromDeliveryID
        {
            get { return fShipFromDeliveryID; }
            set { SetPropertyValue<string>("ShipFromDeliveryID", ref fShipFromDeliveryID, value); }
        }

        private DateTime fShipFromDeliveryDate;
        public DateTime ShipFromDeliveryDate
        {
            get { return fShipFromDeliveryDate; }
            set { SetPropertyValue<DateTime>("ShipFromDeliveryDate", ref fShipFromDeliveryDate, value); }
        }

        private string fShipFromWarehouseID;
        [Size(50)]
        public string ShipFromWarehouseID
        {
            get { return fShipFromWarehouseID; }
            set { SetPropertyValue<string>("ShipFromWarehouseID", ref fShipFromWarehouseID, value); }
        }

        private string fShipFromLocationID;
        [Size(30)]
        public string ShipFromLocationID
        {
            get { return fShipFromLocationID; }
            set { SetPropertyValue<string>("ShipFromLocationID", ref fShipFromLocationID, value); }
        }

        private string fShipFromBuildingNumber;
        [Size(10)]
        public string ShipFromBuildingNumber
        {
            get { return fShipFromBuildingNumber; }
            set { SetPropertyValue<string>("ShipFromBuildingNumber", ref fShipFromBuildingNumber, value); }
        }

        private string fShipFromStreetName;
        [Size(90)]
        public string ShipFromStreetName
        {
            get { return fShipFromStreetName; }
            set { SetPropertyValue<string>("ShipFromStreetName", ref fShipFromStreetName, value); }
        }

        private string fShipFromAddressDetail;
        [Size(100)]
        public string ShipFromAddressDetail
        {
            get { return fShipFromAddressDetail; }
            set { SetPropertyValue<string>("ShipFromAddressDetail", ref fShipFromAddressDetail, value); }
        }

        private string fShipFromCity;
        [Size(50)]
        public string ShipFromCity
        {
            get { return fShipFromCity; }
            set { SetPropertyValue<string>("ShipFromCity", ref fShipFromCity, value); }
        }

        private string fShipFromPostalCode;
        [Size(20)]
        public string ShipFromPostalCode
        {
            get { return fShipFromPostalCode; }
            set { SetPropertyValue<string>("ShipFromPostalCode", ref fShipFromPostalCode, value); }
        }

        private string fShipFromRegion;
        [Size(50)]
        public string ShipFromRegion
        {
            get { return fShipFromRegion; }
            set { SetPropertyValue<string>("ShipFromRegion", ref fShipFromRegion, value); }
        }

        private string fShipFromCountry;
        [Size(5)]
        public string ShipFromCountry
        {
            get { return fShipFromCountry; }
            set { SetPropertyValue<string>("ShipFromCountry", ref fShipFromCountry, value); }
        }

        //MovementOfGoods: Common to ShipTo/ShipFrom
        private DateTime fMovementStartTime;
        public DateTime MovementStartTime
        {
            get { return fMovementStartTime; }
            set { SetPropertyValue<DateTime>("MovementStartTime", ref fMovementStartTime, value); }
        }

        private DateTime fMovementEndTime;
        public DateTime MovementEndTime
        {
            get { return fMovementEndTime; }
            set { SetPropertyValue<DateTime>("MovementEndTime", ref fMovementEndTime, value); }
        }

        /*EOF: SAF-T(PT)*/

        private decimal fTotalNet;
        public decimal TotalNet
        {
            get { return fTotalNet; }
            set { SetPropertyValue<decimal>("TotalNet", ref fTotalNet, value); }
        }

        private decimal fTotalGross;
        public decimal TotalGross
        {
            get { return fTotalGross; }
            set { SetPropertyValue<decimal>("TotalGross", ref fTotalGross, value); }
        }

        private decimal fTotalDiscount;
        public decimal TotalDiscount
        {
            get { return fTotalDiscount; }
            set { SetPropertyValue<decimal>("TotalDiscount", ref fTotalDiscount, value); }
        }

        private decimal fTotalTax;
        public decimal TotalTax
        {
            get { return fTotalTax; }
            set { SetPropertyValue<decimal>("TotalTax", ref fTotalTax, value); }
        }

        private decimal fTotalFinal;
        public decimal TotalFinal
        {
            get { return fTotalFinal; }
            set { SetPropertyValue<decimal>("TotalFinal", ref fTotalFinal, value); }
        }

        private decimal fTotalFinalRound;
        public decimal TotalFinalRound
        {
            get { return fTotalFinalRound; }
            set { SetPropertyValue<decimal>("TotalFinalRound", ref fTotalFinalRound, value); }
        }

        private decimal fTotalDelivery;
        public decimal TotalDelivery
        {
            get { return fTotalDelivery; }
            set { SetPropertyValue<decimal>("TotalDelivery", ref fTotalDelivery, value); }
        }

        private decimal fTotalChange;
        public decimal TotalChange
        {
            get { return fTotalChange; }
            set { SetPropertyValue<decimal>("TotalChange", ref fTotalChange, value); }
        }

        private string fExternalDocument;
        [Size(50)]
        public string ExternalDocument
        {
            get { return fExternalDocument; }
            set { SetPropertyValue<string>("ExternalDocument", ref fExternalDocument, value); }
        }

        private decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        private decimal fDiscountFinancial;
        public decimal DiscountFinancial
        {
            get { return fDiscountFinancial; }
            set { SetPropertyValue<decimal>("DiscountFinancial", ref fDiscountFinancial, value); }
        }

        private decimal fExchangeRate;
        public decimal ExchangeRate
        {
            get { return fExchangeRate; }
            set { SetPropertyValue<decimal>("ExchangeRate", ref fExchangeRate, value); }
        }

        private Guid fEntityOid;
        public Guid EntityOid
        {
            get { return fEntityOid; }
            set { SetPropertyValue<Guid>("EntityOid", ref fEntityOid, value); }
        }

        //Used to Store 30Chars Codes for SAF-T
        private string fEntityInternalCode;
        [Size(30)]
        public string EntityInternalCode
        {
            get { return fEntityInternalCode; }
            set { SetPropertyValue<string>("EntityInternalCode", ref fEntityInternalCode, value); }
        }

        private string fEntityName;
        //[XPGuidObject(Encrypted = true)]
        public string EntityName
        {
            get { return fEntityName; }
            set { SetPropertyValue<string>("EntityName", ref fEntityName, value); }
        }

        private string fEntityAddress;
        //[XPGuidObject(Encrypted = true)]
        public string EntityAddress
        {
            get { return fEntityAddress; }
            set { SetPropertyValue<string>("EntityAddress", ref fEntityAddress, value); }
        }

        private string fEntityLocality;
        //[XPGuidObject(Encrypted = true)]
        public string EntityLocality
        {
            get { return fEntityLocality; }
            set { SetPropertyValue<string>("EntityLocality", ref fEntityLocality, value); }
        }

        private string fEntityZipCode;
        [Size(10)]
        //[XPGuidObject(Encrypted = true)]
        public string EntityZipCode
        {
            get { return fEntityZipCode; }
            set { SetPropertyValue<string>("EntityZipCode", ref fEntityZipCode, value); }
        }

        private string fEntityCity;
        //[XPGuidObject(Encrypted = true)]
        public string EntityCity
        {
            get { return fEntityCity; }
            set { SetPropertyValue<string>("EntityCity", ref fEntityCity, value); }
        }

        private string fEntityCountry;
        [Size(5)]
        //[XPGuidObject(Encrypted = true)]
        public string EntityCountry
        {
            get { return fEntityCountry; }
            set { SetPropertyValue<string>("EntityCountry", ref fEntityCountry, value); }
        }

        private Guid fEntityCountryOid;
        public Guid EntityCountryOid
        {
            get { return fEntityCountryOid; }
            set { SetPropertyValue<Guid>("EntityCountryOid", ref fEntityCountryOid, value); }
        }

        private string fEntityFiscalNumber;
        //[XPGuidObject(Encrypted = true)]
        public string EntityFiscalNumber
        {
            get { return fEntityFiscalNumber; }
            set { SetPropertyValue<string>("FiscalNumber", ref fEntityFiscalNumber, value); }
        }

        private bool fPayed;
        public bool Payed
        {
            get { return fPayed; }
            set { SetPropertyValue<bool>("Payed", ref fPayed, value); }
        }

        private DateTime fPayedDate;
        public DateTime PayedDate
        {
            get { return fPayedDate; }
            set { SetPropertyValue<DateTime>("PayedDate", ref fPayedDate, value); }
        }

        private bool fPrinted;
        public bool Printed
        {
            get { return fPrinted; }
            set { SetPropertyValue<bool>("Printed", ref fPrinted, value); }
        }

        private fin_documentordermain fSourceOrderMain;
        public fin_documentordermain SourceOrderMain
        {
            get { return fSourceOrderMain; }
            set { SetPropertyValue<fin_documentordermain>("SourceOrderMain", ref fSourceOrderMain, value); }
        }

        private fin_documentfinancemaster fDocumentParent;
        public fin_documentfinancemaster DocumentParent
        {
            get { return fDocumentParent; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentParent", ref fDocumentParent, value); }
        }

        private fin_documentfinancemaster fDocumentChild;
        public fin_documentfinancemaster DocumentChild
        {
            get { return fDocumentChild; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentChild", ref fDocumentChild, value); }
        }

        //WayBill Code
        private string fATDocCodeID;
        [Size(200)]
        public string ATDocCodeID
        {
            get { return fATDocCodeID; }
            set { SetPropertyValue<string>("ATDocCodeID", ref fATDocCodeID, value); }
        }

        //
        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        private string fATCUD;
        [Size(200)]
        public string ATCUD
        {
            get { return fATCUD; }
            set { SetPropertyValue<string>("ATCUD", ref fATCUD, value); }
        }

        //QRCode image
        private string fATDocQRCode;
        [Size(200)]
        public string ATDocQRCode
        {
            get { return fATDocQRCode; }
            set { SetPropertyValue<string>("ATDocQRCode", ref fATDocQRCode, value); }
        }

        //AT WebServices
        private sys_systemauditat fATValidAuditResult;
        public sys_systemauditat ATValidAuditResult
        {
            get { return fATValidAuditResult; }
            set { SetPropertyValue<sys_systemauditat>("ATValidAuditResult", ref fATValidAuditResult, value); }
        }

        private bool fATResendDocument;
        public bool ATResendDocument
        {
            get { return fATResendDocument; }
            set { SetPropertyValue<bool>("ATResendDocument", ref fATResendDocument, value); }
        }

        //DocumentFinanceMaster One <> Many SystemAuditATWS
        [Association(@"DocumentFinanceMasterReferencesSystemAuditAT", typeof(sys_systemauditat))]
        public XPCollection<sys_systemauditat> ATAudit
        {
            get { return GetCollection<sys_systemauditat>("ATAudit"); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceDetail
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceDetail", typeof(fin_documentfinancedetail))]
        public XPCollection<fin_documentfinancedetail> DocumentDetail
        {
            get { return GetCollection<fin_documentfinancedetail>("DocumentDetail"); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceMasterTotal
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceMasterTotal", typeof(fin_documentfinancemastertotal))]
        public XPCollection<fin_documentfinancemastertotal> Totals
        {
            get { return GetCollection<fin_documentfinancemastertotal>("Totals"); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceMaster
        private fin_documentfinancetype fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceMaster")]
        public fin_documentfinancetype DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<fin_documentfinancetype>("DocumentType", ref fDocumentType, value); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinanceMaster
        private fin_documentfinanceseries fDocumentSerie;
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinanceMaster")]
        public fin_documentfinanceseries DocumentSerie
        {
            get { return fDocumentSerie; }
            set { SetPropertyValue<fin_documentfinanceseries>("DocumentSerie", ref fDocumentSerie, value); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinanceMaster
        private fin_configurationpaymentmethod fPaymentMethod;
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinanceMaster")]
        public fin_configurationpaymentmethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue<fin_configurationpaymentmethod>("PaymentMethod", ref fPaymentMethod, value); }
        }

        //ConfigurationPaymentCondition One <> Many DocumentFinanceMaster
        private fin_configurationpaymentcondition fPaymentCondition;
        [Association(@"ConfigurationPaymentConditionReferencesDocumentFinanceMaster")]
        public fin_configurationpaymentcondition PaymentCondition
        {
            get { return fPaymentCondition; }
            set { SetPropertyValue<fin_configurationpaymentcondition>("PaymentCondition", ref fPaymentCondition, value); }
        }

        //ConfigurationCurrency One <> Many DocumentFinanceMaster
        private cfg_configurationcurrency fCurrency;
        [Association(@"ConfigurationCurrencyReferencesDocumentFinanceMaster")]
        public cfg_configurationcurrency Currency
        {
            get { return fCurrency; }
            set { SetPropertyValue<cfg_configurationcurrency>("Currency", ref fCurrency, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMaster", typeof(fin_documentfinancemasterpayment))]
        public XPCollection<fin_documentfinancemasterpayment> DocumentPayment
        {
            get { return GetCollection<fin_documentfinancemasterpayment>("DocumentPayment"); }
        }

        //DocumentFinanceMaster One <> Many SystemPrint
        [Association(@"DocumentFinanceMasterReferencesSystemPrint", typeof(sys_systemprint))]
        public XPCollection<sys_systemprint> SystemPrint
        {
            get { return GetCollection<sys_systemprint>("SystemPrint"); }
        }

        ////SystemNotification One <> Many DocumentFinanceMaster
        //sys_systemnotification fNotification;
        //[Association(@"SystemNotificationReferencesDocumentFinanceMaster")]
        //public sys_systemnotification Notification
        //{
        //    get { return fNotification; }
        //    set { SetPropertyValue<sys_systemnotification>("Notification", ref fNotification, value); }
        //}

        ////SystemNotification One <> Many DocumentFinanceMaster
        //[Association(@"SystemNotificationReferencesDocumentFinanceMaster", typeof(sys_systemnotification))]
        //public XPCollection<sys_systemnotification> Notification
        //{
        //    get { return GetCollection<sys_systemnotification>("Notification"); }
        //}

        ////DocumentFinanceMaster Many <> Many SystemNotification
        //[Association(@"DocumentFinanceMasterReferenceSystemNotification", typeof(sys_systemnotification))]
        //public XPCollection<sys_systemnotification> Notification
        //{
        //    get { return GetCollection<sys_systemnotification>("Notification"); }
        //}

        //SystemNotification One <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceMasterReferenceSystemNotification", typeof(sys_systemnotificationdocumentmaster))]
        public XPCollection<sys_systemnotificationdocumentmaster> Notifications
        {
            get { return GetCollection<sys_systemnotificationdocumentmaster>("Notifications"); }
        }

        public string IncreaseDocumentNumber()
        {
            string result = string.Empty;

            try
            {
                string documentNumber = this.DocumentNumber;
                string[] split = documentNumber.Split('/');
                int number = Convert.ToInt16(split[1]) + 1;
                this.DocumentNumber = string.Format("{0}/{1}", split[0], number);
                this.Save();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

    }
}
