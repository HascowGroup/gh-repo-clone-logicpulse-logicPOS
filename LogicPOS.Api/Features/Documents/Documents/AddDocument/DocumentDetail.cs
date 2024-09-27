﻿using System;

namespace LogicPOS.Api.Features.Documents.AddDocument
{
    public class DocumentDetail
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Vat { get; set; }
        public string VatExemptionReason { get; set; }
        public decimal? Discount { get; set; }
        public int? PriceType { get; set; }
    }
}
