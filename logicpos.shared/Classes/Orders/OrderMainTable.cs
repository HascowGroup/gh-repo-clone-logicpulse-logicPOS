﻿using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.shared.App;
using System;
using static logicpos.datalayer.App.DataLayerUtils;

namespace logicpos.shared.Classes.Orders
{
    public class OrderMainTable
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Guid _oid;
        public Guid Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private PriceType _priceType;
        public PriceType PriceType
        {
            get { return _priceType; }
            set { _priceType = value; }
        }

        private Guid _placeId;
        public Guid PlaceId
        {
            get { return _placeId; }
            set { _placeId = value; }
        }

        private Guid _orderMainOid;
        public Guid OrderMainOid
        {
            get { return _orderMainOid; }
            set { _orderMainOid = value; }
        }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderMainTable() { }
        //Constructor without Json.NET Load, With Defaults
        public OrderMainTable(Guid pOrderMainOid, Guid pTableOid)
        {
            _orderMainOid = pOrderMainOid;
            _oid = pTableOid;
            try
            {
                pos_configurationplacetable table = (pos_configurationplacetable)GetXPGuidObject(typeof(pos_configurationplacetable), pTableOid);

                //If table is null, select Table with code 10
                if (table == null)
                {
                    table = (pos_configurationplacetable)SharedUtils.GetXPGuidObjectFromCriteria(typeof(pos_configurationplacetable), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Code = '{0}')", "10")) as pos_configurationplacetable;
                }
                _name = table.Designation;
                //Enum is not Zero Indexed
                _priceType = (PriceType)table.Place.PriceType.EnumValue;
                _placeId = table.Place.Oid;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

