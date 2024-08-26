﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Holidays.GetAllHolidays;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class HolidayPage : Page<Holiday>
    {
        protected override IRequest<ErrorOr<IEnumerable<Holiday>>> GetAllQuery => new GetAllHolidaysQuery();
        public HolidayPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new HolidayModal(mode, SelectedEntity as Holiday);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateDayColumn());
            GridView.AppendColumn(CreateMonthColumn());
            GridView.AppendColumn(CreateYearColumn());
            GridView.AppendColumn(Columns.CreateDesignationColumn(4));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(5));
        }

        private TreeViewColumn CreateMonthColumn()
        {

            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var holiday = (Holiday)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = holiday.Month.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_month");
            return Columns.CreateColumn(title, 4, RenderMonth);
        }

        private TreeViewColumn CreateYearColumn()
        {
            void RenderYear(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var holiday = (Holiday)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = holiday.Year.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_year");
            return Columns.CreateColumn(title, 3, RenderYear);
        }

        private TreeViewColumn CreateDayColumn()
        {
            void RenderDay(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var holiday = (Holiday)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = holiday.Day.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_day");
            return Columns.CreateColumn(title, 2, RenderDay);
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
