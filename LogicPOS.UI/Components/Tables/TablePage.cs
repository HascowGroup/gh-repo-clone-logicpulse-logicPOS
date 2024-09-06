﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Pages
{
    public class TablesPage : Page<Table>
    {
        public TablesPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<Table>>> GetAllQuery => new GetAllTablesQuery();

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new TableModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(2));
            GridView.AppendColumn(CreatePlaceColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

        private TreeViewColumn CreatePlaceColumn()
        {
            void RenderPlace(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var table = (Table)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = table.Place.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_places");
            return Columns.CreateColumn(title, 3, RenderPlace);
        }


        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }
    }
}
