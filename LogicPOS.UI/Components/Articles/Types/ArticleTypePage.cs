﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Types.GetAllArticleTypes;
using LogicPOS.Api.Features.Currencies.GetAllCurrencies;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ArticleTypePage : Page<ArticleType>
    {
        protected override IRequest<ErrorOr<IEnumerable<ArticleType>>> GetAllQuery => new GetAllArticleTypesQuery();
        public ArticleTypePage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
         var modal = new ArticleTypeModal(mode, SelectedEntity as ArticleType);
            modal.Run();
            modal.Destroy();
         }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateHasPriceColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateHasPriceColumn()
        {
            void RenderHasPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var articleType = (ArticleType)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = GeneralUtils.GetResourceByName("global_treeview_true");
                if (articleType.HasPrice == false)
                {
                    (cell as CellRendererText).Text = GeneralUtils.GetResourceByName("global_treeview_false");
                }
            }

            var title = GeneralUtils.GetResourceByName("global_articletype_haveprice");
            return Columns.CreateColumn(title, 2, RenderHasPrice);
        }


        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddHasPriceSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddHasPriceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftArticleType = (ArticleType)model.GetValue(left, 0);
                var rightArticleType = (ArticleType)model.GetValue(right, 0);

                if (leftArticleType == null || rightArticleType == null)
                {
                    return 0;
                }

                return leftArticleType.HasPrice.CompareTo(rightArticleType.HasPrice);
            });
        }
    }
}
