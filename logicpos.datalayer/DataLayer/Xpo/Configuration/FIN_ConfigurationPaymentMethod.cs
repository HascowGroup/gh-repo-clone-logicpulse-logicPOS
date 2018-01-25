using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ConfigurationPaymentMethod : XPGuidObject
    {
        public FIN_ConfigurationPaymentMethod() : base() { }
        public FIN_ConfigurationPaymentMethod(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationPaymentMethod", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationPaymentMethod", "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public String Token
        {
            get { return fToken; }
            set { SetPropertyValue<String>("Token", ref fToken, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        string fAcronym;
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        string fAllowPayback;
        public string AllowPayback
        {
            get { return fAllowPayback; }
            set { SetPropertyValue<string>("AllowPayback", ref fAllowPayback, value); }
        }

        string fSymbol;
        public string Symbol
        {
            get { return fSymbol; }
            set { SetPropertyValue<string>("Symbol", ref fSymbol, value); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinanceMaster
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinanceMaster", typeof(FIN_DocumentFinanceMaster))]
        public XPCollection<FIN_DocumentFinanceMaster> DocumentMaster
        {
            get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentMaster"); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinancePayment
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinancePayment", typeof(FIN_DocumentFinancePayment))]
        public XPCollection<FIN_DocumentFinancePayment> DocumentPayment
        {
            get { return GetCollection<FIN_DocumentFinancePayment>("DocumentPayment"); }
        }
    }
}
