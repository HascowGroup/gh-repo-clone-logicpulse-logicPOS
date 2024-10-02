﻿using System;

namespace LogicPOS.Api.Features.Documents
{
    public class DocumentCustomer
    {
        public string Name { get; set; } 
        public string Address { get; set; }
        public string Locality { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Guid CountryId { get; set; }
        public string FiscalNumber { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
