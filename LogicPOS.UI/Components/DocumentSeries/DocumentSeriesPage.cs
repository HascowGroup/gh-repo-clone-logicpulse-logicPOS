﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries;
using LogicPOS.Api.Features.Terminals.GetAllTerminals;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class DocumentSeriesPage : Page<DocumentSerie>
    {
        protected override IRequest<ErrorOr<IEnumerable<DocumentSerie>>> GetAllQuery => new GetAllDocumentSeriesQuery();
        public DocumentSeriesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new DocumentSerieModal(mode, SelectedEntity as DocumentSerie);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateFiscalYearColumn());
            GridView.AppendColumn(CreateDocumentTypeColumn());
            GridView.AppendColumn(Columns.CreateDesignationColumn(3));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

      

        private TreeViewColumn CreateFiscalYearColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSerie)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.FiscalYear.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_year");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSerie)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.DocumentType.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_documentfinance_type");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddHardwareIdSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddHardwareIdSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftTerminal = (Terminal)model.GetValue(left, 0);
                var rightTerminal = (Terminal)model.GetValue(right, 0);

                if (leftTerminal == null || rightTerminal == null)
                {
                    return 0;
                }

                return leftTerminal.HardwareId.CompareTo(rightTerminal.HardwareId);
            });
        }
    }
}