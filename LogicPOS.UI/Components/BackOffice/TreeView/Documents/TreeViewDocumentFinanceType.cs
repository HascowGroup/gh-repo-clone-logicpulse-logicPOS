﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceType : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceType() { }

        public TreeViewDocumentFinanceType(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceType(Window pSourceWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinancetype);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinancetype defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinancetype : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceType);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("Acronym") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_acronym") },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null)) ? null : pXpoCriteria;
            SortProperty[] sortProperty = new SortProperty[1];
            sortProperty[0] = new SortProperty("Designation", SortingDirection.Ascending);
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