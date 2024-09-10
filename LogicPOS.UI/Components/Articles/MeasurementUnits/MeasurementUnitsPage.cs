﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class MeasurementUnitsPage : Page<MeasurementUnit>
    {
        public MeasurementUnitsPage(Window parent) : base(parent)
        {
        }


        protected override IRequest<ErrorOr<IEnumerable<MeasurementUnit>>> GetAllQuery => new GetAllMeasurementUnitsQuery();
       

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new MeasurementUnitModal(mode, SelectedEntity as MeasurementUnit);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var measurementUnit = (MeasurementUnit)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = measurementUnit.Acronym.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_acronym");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftMeasurementUnit = (MeasurementUnit)model.GetValue(left, 0);
                var rightMeasurementUnit = (MeasurementUnit)model.GetValue(right, 0);

                if (leftMeasurementUnit == null || rightMeasurementUnit == null)
                {
                    return 0;
                }

                return leftMeasurementUnit.Acronym.CompareTo(rightMeasurementUnit.Acronym);
            });
        }
    }
    
}
