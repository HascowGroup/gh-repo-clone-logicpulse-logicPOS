﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.GetAllCustomers;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CustomersPage : Page<Customer>
    {

        protected override IRequest<ErrorOr<IEnumerable<Customer>>> GetAllQuery => new GetAllCustomersQuery();
        public CustomersPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new CustomerModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateNameColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateCardNumberColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

        private TreeViewColumn CreateNameColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (Customer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.Name;
            }

            var title = GeneralUtils.GetResourceByName("global_users");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateCardNumberColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var customer = (Customer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = customer.CardNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_card_number");
            return Columns.CreateColumn(title, 3, RenderValue);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (Customer)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.FiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddNameSorting();
            AddCardNumberSorting();
            AddFiscalNumberSorting();
            AddUpdatedAtSorting(4);
        }

        private void AddNameSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftCustomer = (Customer)model.GetValue(left, 0);
                var rightCustomer = (Customer)model.GetValue(right, 0);

                if (leftCustomer == null || rightCustomer == null)
                {
                    return 0;
                }

                return leftCustomer.Name.CompareTo(rightCustomer.Name);
            });
        }

        private void AddCardNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftCustomer = (Customer)model.GetValue(left, 0);
                var rightCustomer = (Customer)model.GetValue(right, 0);

                if (leftCustomer == null || rightCustomer == null)
                {
                    return 0;
                }

                return leftCustomer.CardNumber?.CompareTo(rightCustomer.CardNumber) ?? 0;
            });
        }

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftCustomer = (Customer)model.GetValue(left, 0);
                var rightCustomer = (Customer)model.GetValue(right, 0);

                if (leftCustomer == null || rightCustomer == null)
                {
                    return 0;
                }

                return leftCustomer.FiscalNumber.CompareTo(rightCustomer.FiscalNumber);
            });
        }
    }
}
