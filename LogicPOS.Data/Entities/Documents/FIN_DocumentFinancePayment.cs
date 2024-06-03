﻿using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinancepayment : XPGuidObject
    {
        public fin_documentfinancepayment() : base() { }
        public fin_documentfinancepayment(Session session) : base(session) { }

        // BOF SAFT-T PT

        private string fPaymentRefNo;
        [Size(60)]
        public string PaymentRefNo
        {
            get { return fPaymentRefNo; }
            set { SetPropertyValue<string>("PaymentRefNo", ref fPaymentRefNo, value); }
        }

        private string fTransactionID;
        [Size(70)]
        public string TransactionID
        {
            get { return fTransactionID; }
            set { SetPropertyValue<string>("TransactionID", ref fTransactionID, value); }
        }

        private string fTransactionDate;
        [Size(19)]
        public string TransactionDate
        {
            get { return fTransactionDate; }
            set { SetPropertyValue<string>("TransactionDate", ref fTransactionDate, value); }
        }

        //RC — Recibo emitido no âmbito do regime de IVA de Caixa (incluindo os relativos a adiantamentos desse regime);
        //RG — Outros recibos emitidos.

        private string fPaymentType;
        [Size(2)]
        public string PaymentType
        {
            get { return fPaymentType; }
            set { SetPropertyValue<string>("PaymentType", ref fPaymentType, value); }
        }

        //DocumentStatus

        //N — Recibo normal e vigente;
        //A — Recibo anulado.
        private string fPaymentStatus;
        [Size(1)]
        public string PaymentStatus
        {
            get { return fPaymentStatus; }
            set { SetPropertyValue<string>("PaymentStatus", ref fPaymentStatus, value); }
        }

        private string fPaymentStatusDate;
        [Size(50)]
        public string PaymentStatusDate
        {
            get { return fPaymentStatusDate; }
            set { SetPropertyValue<string>("PaymentStatusDate", ref fPaymentStatusDate, value); }
        }

        private string fReason;
        [Size(50)]
        public string Reason
        {
            get { return fReason; }
            set { SetPropertyValue<string>("Reason", ref fReason, value); }
        }

        //Utilizador responsável pelo estado atual do recibo.
        private string fDocumentStatusSourceID;
        [Size(30)]
        public string DocumentStatusSourceID
        {
            get { return fDocumentStatusSourceID; }
            set { SetPropertyValue<string>("DocumentStatusSourceID", ref fDocumentStatusSourceID, value); }
        }

        //P — Recibo produzido na aplicação;
        //I — Recibo integrado e produzido noutra aplicação;
        //M — Recibo
        private string fSourcePayment;
        [Size(1)]
        public string SourcePayment
        {
            get { return fSourcePayment; }
            set { SetPropertyValue<string>("SourcePayment", ref fSourcePayment, value); }
        }

        //CC — Cartão crédito;
        //CD — Cartão débito;
        //CH — Cheque bancário;
        //CO — Cheque ou cartão oferta;
        //CS — Compensação de saldos em conta corrente;
        //DE — Dinheiro eletrónico, por exemplo residente em cartões de fidelidade ou de pontos;
        //LC — Letra comercial;
        //MB — Referências de pagamento para Multibanco;
        //NU — Numerário;
        //OU — Outros meios aqui não assinalados;
        //PR — Permuta de bens;
        //TB — Transferência bancária ou débito direto autorizado;
        //TR — Ticket restaurante.
        private string fPaymentMechanism;
        [Size(2)]
        public string PaymentMechanism
        {
            get { return fPaymentMechanism; }
            set { SetPropertyValue<string>("PaymentMechanism", ref fPaymentMechanism, value); }
        }

        private decimal fPaymentAmount;
        public decimal PaymentAmount
        {
            get { return fPaymentAmount; }
            set { SetPropertyValue<decimal>("PaymentAmount", ref fPaymentAmount, value); }
        }

        private string fPaymentDate;
        [Size(19)]
        public string PaymentDate
        {
            get { return fPaymentDate; }
            set { SetPropertyValue<string>("PaymentDate", ref fPaymentDate, value); }
        }

        private string fSourceID;
        [Size(30)]
        public string SourceID
        {
            get { return fSourceID; }
            set { SetPropertyValue<string>("SourceID", ref fSourceID, value); }
        }

        private string fSystemEntryDate;
        [Size(50)]
        public string SystemEntryDate
        {
            get { return fSystemEntryDate; }
            set { SetPropertyValue<string>("SystemEntryDate", ref fSystemEntryDate, value); }
        }

        //Skipped for Now
        //
        //4.4.4.14. * Linha (Line)
        //ao
        //4.4.4.14.6. Taxa de imposto (Tax)

        //4.4.4.15 - DocumentTotals

        private decimal fTaxPayable;
        public decimal TaxPayable
        {
            get { return fTaxPayable; }
            set { SetPropertyValue<decimal>("TaxPayable", ref fTaxPayable, value); }
        }

        private decimal fNetTotal;
        public decimal NetTotal
        {
            get { return fNetTotal; }
            set { SetPropertyValue<decimal>("NetTotal", ref fNetTotal, value); }
        }

        private decimal fGrossTotal;
        public decimal GrossTotal
        {
            get { return fGrossTotal; }
            set { SetPropertyValue<decimal>("GrossTotal", ref fGrossTotal, value); }
        }

        //4.4.4.16. Acordos (Settlement)

        private decimal fSettlementAmount;
        public decimal SettlementAmount
        {
            get { return fSettlementAmount; }
            set { SetPropertyValue<decimal>("SettlementAmount", ref fSettlementAmount, value); }
        }

        private string fCurrencyCode;
        [Size(3)]
        public string CurrencyCode
        {
            get { return fCurrencyCode; }
            set { SetPropertyValue<string>("CurrencyCode", ref fCurrencyCode, value); }
        }

        private decimal fCurrencyAmount;
        public decimal CurrencyAmount
        {
            get { return fCurrencyAmount; }
            set { SetPropertyValue<decimal>("CurrencyAmount", ref fCurrencyAmount, value); }
        }

        private decimal fExchangeRate;
        public decimal ExchangeRate
        {
            get { return fExchangeRate; }
            set { SetPropertyValue<decimal>("ExchangeRate", ref fExchangeRate, value); }
        }

        private decimal fWithholdingTaxAmount;
        public decimal WithholdingTaxAmount
        {
            get { return fWithholdingTaxAmount; }
            set { SetPropertyValue<decimal>("WithholdingTaxAmount", ref fWithholdingTaxAmount, value); }
        }

        //EOF SAF-T PT

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

        private string fDocumentDate;
        [Size(19)]
        public string DocumentDate
        {
            get { return fDocumentDate; }
            set { SetPropertyValue<string>("DocumentDate", ref fDocumentDate, value); }
        }

        private string fExtendedValue;
        [Size(1024)]
        public string ExtendedValue
        {
            get { return fExtendedValue; }
            set { SetPropertyValue<string>("ExtendedValue", ref fExtendedValue, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinancePayment
        private fin_documentfinancetype fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinancePayment")]
        public fin_documentfinancetype DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue("DocumentType", ref fDocumentType, value); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinancePayment
        private fin_documentfinanceseries fDocumentSerie;
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinancePayment")]
        public fin_documentfinanceseries DocumentSerie
        {
            get { return fDocumentSerie; }
            set { SetPropertyValue("DocumentSerie", ref fDocumentSerie, value); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinancePayment
        private fin_configurationpaymentmethod fPaymentMethod;
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinancePayment")]
        public fin_configurationpaymentmethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue("PaymentMethod", ref fPaymentMethod, value); }
        }

        //ConfigurationCurrency One <> Many DocumentFinancePayment
        private cfg_configurationcurrency fCurrency;
        [Association(@"ConfigurationCurrencyReferencesDocumentFinancePayment")]
        public cfg_configurationcurrency Currency
        {
            get { return fCurrency; }
            set { SetPropertyValue("Currency", ref fCurrency, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinancePayment
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinancePayment", typeof(fin_documentfinancemasterpayment))]
        public XPCollection<fin_documentfinancemasterpayment> DocumentPayment
        {
            get { return GetCollection<fin_documentfinancemasterpayment>("DocumentPayment"); }
        }

        //DocumentFinancePayment One <> Many SystemPrint
        [Association(@"DocumentFinancePaymentReferencesSystemPrint", typeof(sys_systemprint))]
        public XPCollection<sys_systemprint> SystemPrint
        {
            get { return GetCollection<sys_systemprint>("SystemPrint"); }
        }
    }
}