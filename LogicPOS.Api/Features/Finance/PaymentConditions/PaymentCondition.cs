﻿using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Entities
{
    public class PaymentCondition : ApiEntity, IWithDesignation, IWithCode
    {
        public uint Order {  get; set; }
        public string Code { get; set; }
        public string Designation {  get; set; }
        public string Acronym { get; set; }

    }
}