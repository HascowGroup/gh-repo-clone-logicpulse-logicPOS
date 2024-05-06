﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewArticle : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticle() { }

        [Obsolete]
        public TreeViewArticle(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //XpoMode
        [Obsolete]
        public TreeViewArticle(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_article);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_article defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_article : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticle);

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_code"), MinWidth = 100 },
                new GenericTreeViewColumnProperty("Designation") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("TotalStock")
                {
                    Query = "SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;",
                    Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_total_stock"),
                    MinWidth = 100,
                    //Alignment = 1.0F,
                    FormatProvider = new FormatterDecimal(),
                    //CellRenderer = new CellRendererText()
                    //{
                    //    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                    //    Alignment = Pango.Alignment.Right,
                    //    Xalign = 1.0F
                    //}
                },
                //To Test XPGuidObject InitialValue (InitialValue = xArticleFamily) : ArticleFamily xArticleFamily = (ArticleFamily)DataLayerUtils.GetXPGuidObjectFromSession(XPOSettings.SessionBackoffice, typeof(ArticleFamily), new Guid("471d8c1e-45c1-4dbe-8526-349c20bd53ef"));
                new GenericTreeViewColumnProperty("IsComposed") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_composite_article") },
                //Artigos Compostos [IN:016522]
                new GenericTreeViewColumnProperty("Family") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article_family"), ChildName = "Designation" },
                new GenericTreeViewColumnProperty("SubFamily") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article_subfamily"), ChildName = "Designation" },
                new GenericTreeViewColumnProperty("Type") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article_type"), ChildName = "Designation" },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            CriteriaOperator criteria;
            // Override Criteria adding XpoOidHiddenRecordsFilter
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              pGenericTreeViewNavigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }
    }
}
