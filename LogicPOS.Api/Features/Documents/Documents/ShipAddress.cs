﻿using System;

namespace LogicPOS.Api.Features.Documents
{
    public class ShipAddress
    {
        public string DeliveryID { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string WarehouseID { get; set; }
        public string LocationID { get; set; }
        public string BuildingNumber { get; set; }
        public string StreetName { get; set; }
        public string AddressDetail { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

    }
}
