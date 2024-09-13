﻿using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.Api.Features.Customers.AddCustomer;
using LogicPOS.Api.Features.Customers.Types.GetAllCustomerTypes;
using LogicPOS.Api.Features.Customers.UpdateCustomer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerModal : EntityModal<Customer>
    {
        public CustomerModal(EntityModalMode modalMode, Customer entity = null) : base(modalMode, entity)
        {

        }

        protected override void ShowEntityData()
        {
            _txtName.Text = _entity.Name;
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtBirthDate.Text = _entity.BirthDate?.ToString("yyyy/MM/dd");
            _txtAddress.Text = _entity.Address;
            _txtLocality.Text = _entity.Locality;
            _txtCity.Text = _entity.City;
            _txtPostalCode.Text = _entity.ZipCode;
            _txtPhone.Text = _entity.Phone;
            _txtMobile.Text = _entity.MobilePhone;
            _txtEmail.Text = _entity.Email;
            _txtFiscalNumber.Text = _entity.FiscalNumber;
            _txtCardNumber.Text = _entity.CardNumber;
            _txtCardCredit.Text = _entity.CardCredit.ToString();
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
            _checkSupplier.Active = _entity.Supplier;
            _txtDiscount.Text = _entity.Discount.ToString();

        }

        private UpdateCustomerCommand CreateUpdateCommand()
        {
            return new UpdateCustomerCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewName = _txtName.Text,
                NewPriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                NewCustomerTypeId = _comboCustomerTypes.SelectedEntity?.Id,
                NewCountryId = _comboCountries.SelectedEntity?.Id,
                NewBirthDate = DateTime.Parse(_txtBirthDate.Text),
                NewAddress = _txtAddress.Text,
                NewLocality = _txtLocality.Text,
                NewZipCode = _txtPostalCode.Text,
                NewCity = _txtCity.Text,
                NewPhone = _txtPhone.Text,
                NewMobilePhone = _txtMobile.Text,
                NewEmail = _txtEmail.Text,
                NewFiscalNumber = _txtFiscalNumber.Text,
                NewCardNumber = _txtCardNumber.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active,
                Supplier = _checkSupplier.Active
            };
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private AddCustomerCommand CreateAddCommand()
        {
            return new AddCustomerCommand
            {
                Name = _txtName.Text,
                PriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                CustomerTypeId = _comboCustomerTypes.SelectedEntity.Id,
                CountryId = _comboCountries.SelectedEntity.Id,
                BirthDate = string.IsNullOrEmpty(_txtBirthDate.Text) ? null : (DateTime?)DateTime.Parse(_txtBirthDate.Text),
                Address = _txtAddress.Text,
                Locality = _txtLocality.Text,
                ZipCode = _txtPostalCode.Text,
                City = _txtCity.Text,
                Phone = _txtPhone.Text,
                MobilePhone = _txtMobile.Text,
                Email = _txtEmail.Text,
                FiscalNumber = _txtFiscalNumber.Text,
                Notes = _txtNotes.Value.Text,
                CardNumber = _txtCardNumber.Text,
                CardCredit = decimal.Parse(_txtCardCredit.Text),
                Supplier = _checkSupplier.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

        private IEnumerable<Country> GetCountries()
        {
            var getCountriesResult = _mediator.Send(new GetAllCountriesQuery()).Result;

            if (getCountriesResult.IsError)
            {
                return Enumerable.Empty<Country>();
            }

            return getCountriesResult.Value;
        }

        private IEnumerable<PriceType> GetPriceTypes() => ExecuteGetAllQuery(new GetAllPriceTypesQuery());

        private IEnumerable<CustomerType> GetCustomerTypes()=>ExecuteGetAllQuery(new GetAllCustomerTypesQuery());

    }
}
