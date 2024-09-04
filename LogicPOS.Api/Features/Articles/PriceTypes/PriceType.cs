﻿using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class PriceType : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public int EnumValue { get; set; }
    }
}
