﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class PrinterTypesPage : Page<PrinterType>
    {
       
        protected override IRequest<ErrorOr<IEnumerable<PrinterType>>> GetAllQuery => new GetAllPrinterTypesQuery();
        public PrinterTypesPage(Window parent) : base(parent)
        {
        }


        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new PrinterTypeModal(mode, SelectedEntity as PrinterType);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateThermalPrinterColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateThermalPrinterColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var printerType = (PrinterType)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = GeneralUtils.GetResourceByName("global_treeview_true");
                if (!printerType.ThermalPrinter)
                {
                    (cell as CellRendererText).Text = GeneralUtils.GetResourceByName("global_treeview_false");
                }
            }

            var title = GeneralUtils.GetResourceByName("global_printer_thermal_printer");
            return Columns.CreateColumn(title, 2, RenderValue);
        }


        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddThermalPrinterSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddThermalPrinterSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPrinterType = (PrinterType)model.GetValue(left, 0);
                var rightPrinterType = (PrinterType)model.GetValue(right, 0);

                if (leftPrinterType == null || rightPrinterType == null)
                {
                    return 0;
                }

                return leftPrinterType.ThermalPrinter.CompareTo(rightPrinterType.ThermalPrinter);
            });
        }

    }
}
