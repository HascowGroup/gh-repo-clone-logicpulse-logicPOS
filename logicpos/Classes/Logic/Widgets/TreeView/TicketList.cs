﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.Extensions;
using logicpos.resources.Resources.Localization;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public partial class TicketList
    {
        //Prev
        private void _buttonKeyPrev_Clicked(object sender, EventArgs e)
        {
            Prev();
        }

        //Next
        private void _buttonKeyNext_Clicked(object sender, EventArgs e)
        {
            Next();
        }

        //Delete
        private void _buttonKeyDelete_Clicked(object sender, EventArgs e)
        {
            if (_listMode == TicketListMode.OrderMain)
            {
                ResponseType responseType = logicpos.Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, new System.Drawing.Size(400, 280), MessageType.Question, ButtonsType.YesNo, string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_warning"), SharedFramework.ServerVersion), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message__pos_order_cancel"));

                if (responseType == ResponseType.Yes)
                {
                    try
                    {
                        _listStoreModel.Clear();
                        _listStoreModelSelectedIndex = -1;
                        _listStoreModelTotalItems = 0;
                        //Get Reference to current OrderMain
                        OrderMain orderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
                        fin_documentordermain documentOrderMain = null;
                        //Get current OrderMain Article Bag, After Process Payment/PartialPayment to check if current OrderMain has Items, or is Empty
                        ArticleBag pParameters = ArticleBag.TicketOrderToArticleBag(orderMain);
                        //Start UnitOfWork
                        using (UnitOfWork uowSession = new UnitOfWork())
                        {
                            if (pParameters.Count > 0)
                            {
                                // Warning required to check if (documentOrderMain != null), when we work with SplitPayments and work only one product, 
                                // the 2,3,4....orders are null, this is because first FinanceDocument Closes Order

                                //Close OrderMain
                                //if (documentOrderMain != null) documentOrderMain.OrderStatus = OrderStatus.Close;

                                //Required to Update and Sync Terminals
                                //if (documentOrderMain != null) documentOrderMain.UpdatedAt = documentDateTime;

                                //Change Table Status to Free
                                pos_configurationplacetable placeTable;
                                placeTable = (pos_configurationplacetable)DataLayerUtils.GetXPGuidObject(DataLayerFramework.SessionXpo, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                                documentOrderMain = (fin_documentordermain)uowSession.GetObjectByKey(typeof(fin_documentordermain), orderMain.PersistentOid);

                                placeTable.TableStatus = TableStatus.Free;
                                SharedUtils.Audit("TABLE_OPEN", string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "audit_message_table_open"), placeTable.Designation));
                                placeTable.DateTableClosed = DateTime.Now;
                                placeTable.TotalOpen = 0;
                                placeTable.Save();
                                //Required to Reload Objects after has been changed in Another Session(uowSession)
                                if (documentOrderMain != null)
                                {
                                    documentOrderMain = (fin_documentordermain)DataLayerUtils.GetXPGuidObject(DataLayerFramework.SessionXpo, typeof(fin_documentordermain), orderMain.PersistentOid);
                                    documentOrderMain.OrderStatus = OrderStatus.Close;
                                    documentOrderMain.Save();
                                }

                                if (documentOrderMain != null) documentOrderMain.Reload();
                                //aceTable = (pos_configurationplacetable)DataLayerUtils.GetXPGuidObject(DataLayerFramework.SessionXpo, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                                //placeTable.Reload();
                                ArticleBag.TicketOrderToArticleBag(orderMain).Clear();
                                //Clean Session if Commited without problems
                                orderMain.OrderStatus = OrderStatus.Close;
                                orderMain.CleanSessionOrder();
                                SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid] = orderMain;
                                SharedFramework.SessionApp.DeleteEmptyTickets();
                                //obalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].CleanSessionOrder();
                                uowSession.CommitChanges();
                            }
                        }
                        //PartialPayment Detected
                        //else
                        //{
                        //    //Required to Update and Sync Terminals
                        //    if (documentOrderMain != null) documentOrderMain.UpdatedAt = documentDateTime;
                        //}


                        //Remove from SessionApp
                        //_currentOrderDetails.Delete(_listStoreModelSelectedIndex);

                        //Remove from TreviewModel
                        //_listStoreModel.Remove(ref _treeIter);
                        //Get Reference to current OrderMain

                        CurrentOrderDetails = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;

                        //Always Change to OrderMain ListMode before Update Model
                        _listMode = TicketListMode.Ticket;
                        orderMain.CleanSessionOrder();
                        Gdk.Color colorListMode = (_listMode == TicketListMode.Ticket) ? colorListMode = _colorPosTicketListModeTicketBackground.ToGdkColor() : colorListMode = _colorPosTicketListModeOrderMainBackground.ToGdkColor();
                        _treeView.ModifyBase(StateType.Normal, colorListMode);
                        //UpdateModel();
                        UpdateOrderStatusBar();
                        UpdateTicketListOrderButtons();
                        //IMPORTANT & REQUIRED: Assign Current Order Details from New CurrentTicketId, ELSE we cant add items to OrderMain
                        CurrentOrderDetails = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;

                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message, ex);
                    }
                }
            }
            else
            {
                DeleteItem_Event(TicketListDeleteMode.Delete);
            }

        }

        //Decrease
        private void _buttonKeyDecrease_Clicked(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                var price = Convert.ToDecimal(_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price));
                //_listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindLastIndex(item => item.ArticleOid == _currentDetailArticleOid && item.Properties.PriceFinal == _currentDetailArticle.Price1);
                foreach (var item in CurrentOrderDetails.Lines)
                {
                    if (item.ArticleOid == _currentDetailArticleOid && item.Properties.PriceFinal == price)
                    {
                        _listStoreModelSelectedIndex = i;
                    }
                    i++;
                }
                //_listMode = TicketListMode.EditList;
                if (_listStoreModelSelectedIndex == -1)
                {
                    //Get current Index with LINQ : To Get OrderDetail articleId Index, If Exists
                    //_listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindIndex(item => item.ArticleOid == _currentDetailArticleOid);
                }
                DeleteItem_Event(TicketListDeleteMode.Decrease);
                UpdateTicketListButtons();
                //UpdateModel();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        //Increase
        private void _buttonKeyIncrease_Clicked(object sender, EventArgs e)
        {
            try
            {       
                //Get Article defaultQuantity
                decimal defaultQuantity = GetArticleDefaultQuantity(_currentDetailArticleOid);
                int i = 0;
                var price = Convert.ToDecimal(_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price));
                //_listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindIndex(item => item.ArticleOid == _currentDetailArticleOid && item.Properties.PriceFinal == _currentDetailArticle.Price1);
                //foreach (var item in CurrentOrderDetails.Lines)
                //{
                //    if (item.ArticleOid == _currentDetailArticleOid && item.Properties.PriceFinal == price)
                //    {
                //        _listStoreModelSelectedIndex = i;
                //    }
                //    i++;
                //}
                decimal oldValueQnt = Convert.ToDecimal(_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Quantity));
                if (logicpos.Utils.CheckStocks())
                {
                    if (!logicpos.Utils.ShowMessageMinimumStock(_sourceWindow, CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].ArticleOid, (oldValueQnt + defaultQuantity)))
                    {
                        ChangeQuantity(oldValueQnt + defaultQuantity);
                        UpdateTicketListButtons();
                    }
                    else
                    {
                        UpdateTicketListButtons();
                    }
                }
                else
                {
                    ChangeQuantity(oldValueQnt + defaultQuantity);
                    UpdateTicketListButtons();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        //Change Quantity
        private void _buttonKeyChangeQuantity_Clicked(object sender, EventArgs e)
        {
            try
            {
                decimal oldValueQnt = CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;
                decimal newValueQnt = PosKeyboardDialog.RequestDecimalValue(_sourceWindow, oldValueQnt);
                bool showMessage;
                if (newValueQnt > 0)
                {
                    if (logicpos.Utils.CheckStocks()) 
                    {
                        if (!logicpos.Utils.ShowMessageMinimumStock(_sourceWindow, CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].ArticleOid, newValueQnt, out showMessage))
                        {
                            if (showMessage)
                            {
                                newValueQnt = oldValueQnt;
                                return;
                            }
                        }
                    }
                    ChangeQuantity(newValueQnt);
                }
                UpdateTicketListButtons();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Change Price
        private void _buttonKeyChangePrice_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Get Index of article with correct final price
                _listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindIndex(item => item.ArticleOid == (Guid)_listStoreModel.GetValue(_treeIter, 
                    (int)TicketListColumns.ArticleId) && Math.Round(Convert.ToDecimal(item.Properties.PriceFinal), SharedSettings.DecimalRoundTo) == Convert.ToDecimal(_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price)));

                decimal oldValueQuantity = CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;
                decimal oldValuePrice = CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceFinal;

                MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(_sourceWindow, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_moneypad_product_price"), oldValuePrice);
                decimal newValuePrice = result.Value;

                if (result.Response == ResponseType.Ok && newValuePrice > 0)
                {
                    //Create a Fresh Object to Get Input Price and Calc from TotalFinal with Quantity 1, Without Touch Quantity in current Line
                    PriceProperties priceProperties = PriceProperties.GetPriceProperties(
                      PricePropertiesSourceMode.FromTotalFinal,
                      CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceWithVat,
                      newValuePrice,
                      1.0m,
                      CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.DiscountArticle,
                      CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.DiscountGlobal,
                      CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Vat
                    );
                    int countDuplicatedArticles = 0;
                    int i = 0;
                    foreach (var line in _currentOrderDetails.Lines)
                    {
                        if (_listStoreModelSelectedIndex == i)
                        {
                            countDuplicatedArticles++;
                        }
                        if (line.ArticleOid == CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].ArticleOid && line.Properties.PriceUser == priceProperties.PriceUser) countDuplicatedArticles++;
                        i++;
                    }

                    if (countDuplicatedArticles > 1)
                    {
                        int oldIndex = _listStoreModelSelectedIndex;
                        _listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindIndex(item => item.ArticleOid == (Guid)_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.ArticleId) 
                        && item.Properties.PriceFinal == priceProperties.PriceFinal);
                        oldValueQuantity += CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;
                        CurrentOrderDetails.Delete(oldIndex);
                        _listStoreModel.Remove(ref _treeIter);
                    }

                    //Update orderDetails 
                    CurrentOrderDetails.Update(_listStoreModelSelectedIndex, oldValueQuantity, priceProperties.PriceUser);
                    //Update TreeView Model Price
                    _listStoreModel.SetValue(_treeIter, (int)TicketListColumns.Price, SharedUtils.DecimalToString(newValuePrice));
                    //Update Total
                    decimal totalLine = CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.TotalFinal;
                    _listStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, SharedUtils.DecimalToString(totalLine));
                }
                UpdateTicketListTotal();                
                UpdateModel();
                UpdateTicketListButtons();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Finish Order
        private void _buttonKeyFinishOrder_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Call Framework FinishOrder
                OrderMain orderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
                //CurrentOrderDetailsAll = CurrentOrderDetails;
                /* 
                 * TK013134 
                 * Parking Ticket Module: Checking for duplicates in Order Main after finishing order
                 */
                if (SharedFramework.AppUseParkingTicketModule)
                {
                    orderMain.CheckForDuplicatedArticleInArticleBag(DataLayerFramework.SessionXpo);
                }

                fin_documentorderticket orderTicket = orderMain.FinishOrder(DataLayerFramework.SessionXpo);


                // If OrderTicket and has a ThermalPrinter connected
                // Impressoras - Diferenciação entre Tipos [TK:016249]
                SharedFramework.UsingThermalPrinter = true;
                if (DataLayerFramework.LoggedTerminal.ThermalPrinter != null &&
                    DataLayerFramework.LoggedTerminal.ThermalPrinter.PrinterType.ThermalPrinter &&
                    orderTicket.OrderDetail.Count != 0)
                {
                    //public static bool PrintOrderRequest(Window pSourceWindow, sys_configurationprinters pPrinter, OrderMain pDocumentOrderMain, fin_documentorderticket pOrderTicket)
                    //IN009239 - This avoids orders being printed when in use of ParkingTicketModule
					//Opção para imprimir ou não o ticket
                    //bool printTicket = true;
                    //printTicket = Convert.ToBoolean(DataLayerFramework.Settings["printTicket"]);

					//Criação de variável nas configurações para imprimir ou não ticket's [IN:013328]
                    if (!SharedFramework.AppUseParkingTicketModule && logicpos.Utils.PrintTicket())
                    {
                        // TK016249 Impressoras - Diferenciação entre Tipos 
                        FrameworkCalls.PrintOrderRequest(_sourceWindow, DataLayerFramework.LoggedTerminal.ThermalPrinter, orderMain, orderTicket);
                    }
                    FrameworkCalls.PrintArticleRequest(_sourceWindow, orderTicket);
                }

                //Change Mode
                _listMode = TicketListMode.OrderMain;
                //Reset TicketList TotalItems Counter
                _listStoreModelTotalItemsTicketListMode = 0;
                UpdateOrderStatusBar();
                UpdateModel();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Payments and SplitAccount (Shared for Both Actions)
        private void _buttonKeyPayments_Clicked(object sender, EventArgs e)
        {
            TouchButtonIconWithText button = (sender as TouchButtonIconWithText);

            try
            {
                //Used when we pay without FinishOrder, to Skip print Ticket
                bool printTicket = false;

                //Request Finish Open Ticket
                if (_listStoreModelTotalItemsTicketListMode > 0)
                {
                    ResponseType dialogResponse = logicpos.Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.OkCancel, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_message_dialog"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_request_close_open_ticket"));
                    if (dialogResponse != ResponseType.Ok)
                    {
                        return;
                    };
                };

                //Get Reference to current OrderMain
                OrderMain orderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];

                //Finish Order, if Has Ticket Details
                if (orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails.Lines.Count > 0)
                {
                    //Before Use FrameworkCall
                    orderMain.FinishOrder(DataLayerFramework.SessionXpo, printTicket);
                    //TODO: Continue to implement FrameworkCall here
                    //DocumentOrderTicket documentOrderTicket = orderMain.FinishOrder(DataLayerFramework.SessionXpo, printTicket);
                    //if (printTicket) FrameworkCalls.PrintTableTicket(_sourceWindow, DataLayerFramework.LoggedTerminal.Printer, DataLayerFramework.LoggedTerminal.TemplateTicket, orderMain, documentOrderTicket.Oid);

                    //Reset TicketList TotalItems Counter
                    _listStoreModelTotalItemsTicketListMode = 0;
                }

                //Always Change to OrderMain ListMode before Update Model
                _listMode = TicketListMode.OrderMain;

                //Update Model and Gui
                UpdateModel();
                UpdateOrderStatusBar();
                UpdateTicketListOrderButtons();

                //Initialize ArticleBag to Send to Payment Dialog
                ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);

                // Shared Referencesfor Dialog
                PosBaseDialog dialog = null;

                // Get Dialog Reference
                if (button.Name.Equals("touchButtonPosTicketPadPayments_Green"))
                {
                    dialog = new PosPaymentsDialog(_sourceWindow, DialogFlags.DestroyWithParent, articleBag);
                }
                else
                if (button.Name.Equals("touchButtonPosTicketPadSplitAccount_Green"))
                {
                    dialog = new PosSplitPaymentsDialog(_sourceWindow, DialogFlags.DestroyWithParent, _articleBag, this);
                }

                // Shared code to call Both Dialogs
                ResponseType response = (ResponseType)dialog.Run();

                if (response == ResponseType.Ok)
                {
                    //Update Cleaned TreeView Model
                    UpdateModel();
                    UpdateOrderStatusBar();
                    UpdateTicketListOrderButtons();
                    //IMPORTANT & REQUIRED: Assign Current Order Details from New CurrentTicketId, ELSE we cant add items to OrderMain
                    CurrentOrderDetails = orderMain.OrderTickets[orderMain.CurrentTicketId].OrderDetails;
                    //Valid Result Destroy Dialog
                    dialog.Destroy();
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //BarCode
        private void _buttonKeyBarCode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_input_text_barcode.png");
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(_sourceWindow, DialogFlags.Modal, fileWindowIcon, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_barcode_articlecode"), string.Empty, SharedSettings.RegexAlfaNumericExtended, true);

            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                if (SharedFramework.AppUseParkingTicketModule) /* IN009239 */
                {
                    GlobalApp.ParkingTicket.GetTicketDetailFromWS(dialogResponse.Text);
                }
                else
                {
                    InsertOrUpdate(dialogResponse.Text);
                }
            }
        }

        //IN009279 CardCode scanner
        private void _buttonKeyCardCode_Clicked(object sender, EventArgs e)
        {
            string fileWindowIcon = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Windows\icon_pos_ticketpad_card_entry.png");
            logicpos.Utils.ResponseText dialogResponse = logicpos.Utils.GetInputText(_sourceWindow, DialogFlags.Modal, fileWindowIcon, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_cardcode_small"), string.Empty, SharedSettings.RegexAlfaNumericExtended, true);

            if (dialogResponse.ResponseType == ResponseType.Ok)
            {
                if (SharedFramework.AppUseParkingTicketModule) /* IN009239 */
                {
                    GlobalApp.ParkingTicket.GetTicketDetailFromWS(dialogResponse.Text);
                }
                else
                {
                    InsertOrUpdate(dialogResponse.Text);
                }
            }
        }

        //ListOrder
        private void _buttonKeyListOrder_Clicked(object sender, EventArgs e)
        {
            try
            {
                OrderMain currentOrderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
                PosOrdersDialog dialog = new PosOrdersDialog(this.SourceWindow, DialogFlags.DestroyWithParent, currentOrderMain.Table.Name);
                ResponseType response = (ResponseType)dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //ChangeTable - Change Order From Table to Table
        private void _buttonKeyChangeTable_Clicked(object sender, EventArgs e)
        {
            PosTablesDialog dialog = new PosTablesDialog(this.SourceWindow, DialogFlags.DestroyWithParent, TableFilterMode.OnlyFreeTables);
            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
            {
                if (response == ResponseType.Ok)
                {
                    OrderMain currentOrderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
                    pos_configurationplacetable xOldTable = (pos_configurationplacetable)DataLayerUtils.GetXPGuidObject(typeof(pos_configurationplacetable), currentOrderMain.Table.Oid);
                    pos_configurationplacetable xNewTable = (pos_configurationplacetable)DataLayerUtils.GetXPGuidObject(typeof(pos_configurationplacetable), dialog.CurrentTableOid);
                    //Require to Prevent A first chance exception of type 'DevExpress.Xpo.DB.Exceptions.LockingException' occurred in DevExpress.Xpo.v13.2.dll when it is Changed in Diferent Session ex UnitOfWork
                    //TODO: Confirm working with Reload Commented
                    //xOldTable.Reload();
                    //xNewTable.Reload();

                    if (xNewTable.TableStatus != TableStatus.Free)
                    {
                        logicpos.Utils.ShowMessageTouch(
                            GlobalApp.PosMainWindow, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error"),
                            resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_table_is_not_free")
                        );
                    }
                    else
                    {
                        //Put Old table Status to Free
                        xOldTable.TableStatus = TableStatus.Free;
                        xOldTable.Save();
                        SharedUtils.Audit("TABLE_OPEN", string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "audit_message_table_open"), xOldTable.Designation));

                        //Put New table Status to Open
                        xNewTable.TableStatus = TableStatus.Open;
                        xNewTable.Save();
                        SharedUtils.Audit("TABLE_CLOSE", string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "audit_message_table_close"), xNewTable.Designation));

                        //Change DocumentOrderMain table, If OpenOrder Exists in That table
                        Guid documentOrderMainOid = currentOrderMain.GetOpenTableFieldValueGuid(xOldTable.Oid, "Oid");
                        fin_documentordermain xDocumentOrderMain = (fin_documentordermain)DataLayerUtils.GetXPGuidObject(typeof(fin_documentordermain), documentOrderMainOid);
                        if (xDocumentOrderMain != null)
                        {
                            xDocumentOrderMain.PlaceTable = xNewTable;
                            xDocumentOrderMain.UpdatedAt = DataLayerUtils.CurrentDateTimeAtomic();
                            xDocumentOrderMain.Save();
                        }
                        //Assign Session Data
                        currentOrderMain.Table.Oid = xNewTable.Oid;
                        currentOrderMain.Table.Name = xNewTable.Designation;
                        currentOrderMain.Table.PriceType = (PriceType)xNewTable.Place.PriceType.EnumValue;
                        currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId].PriceType = (PriceType)xNewTable.Place.PriceType.EnumValue;
                        currentOrderMain.Table.PlaceId = xNewTable.Place.Oid;
                        SharedFramework.SessionApp.Write();
                        //Finally Update Status Bar, Table Name, Totals Etc
                        UpdateOrderStatusBar();
                    }
                }
                dialog.Destroy();
            };
        }

        //Change List Mode
        private void _buttonKeyListMode_Clicked(object sender, EventArgs e)
        {
            //Toggle Mode
            _listMode = (_listMode == TicketListMode.Ticket) ? TicketListMode.OrderMain : TicketListMode.Ticket;
            //ArticleBag
            UpdateModel();
            //CurrentOrderDetails = CurrentOrderDetailsAll;
            //generateTicketAll();
            //for(int i=0; i < CurrentOrderDetailsAll.Lines.Count; i++) { CurrentOrderDetailsAll.Delete(i); }
            //_listMode = TicketListMode.EditList;

        }

        //Gifts
        private void _buttonKeyGifts_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //Weight
        private void _buttonKeyWeight_Clicked(object sender, EventArgs e)
        {
            try
            {
                _listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindIndex(item => item.ArticleOid == (Guid)_listStoreModel.GetValue(_treeIter,
       (int)TicketListColumns.ArticleId) && item.Properties.PriceNet == Convert.ToDecimal(_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price)));
                //_logger.Debug(string.Format("PriceUser: [{0}]", _currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceUser));
                // Round Price before Send to WeighingBalance
                decimal articlePricePerKg = decimal.Round(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceFinal, 2, MidpointRounding.AwayFromZero);
                GlobalApp.WeighingBalance.WeighArticle(articlePricePerKg);
                // Old Deprecated code, Before Method WeighingBalance.WeighArticle
                //string priceString = articlePricePerKg.ToString().Replace(",", string.Empty).Replace(".", string.Empty);
                //string textSendFormatted = Convert.ToInt16(priceString).ToString("00000");
                //string weightHex = GlobalApp.WeighingBalance.ToHexString(textSendFormatted);
                //GlobalApp.WeighingBalance.WriteData(GlobalApp.WeighingBalance.CalculateHexFromPrice(weightHex));
                //GlobalApp.WeighingBalance.ClosePort();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //WeighingBalance

        public void WeighingBalanceDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //_logger.Debug("WeighingBalanceDataReceived");
            string inData = GlobalApp.WeighingBalance.ComPort().ReadLine() + "\n";

            if (inData.Substring(0, 2) == "99")
            {
                List<int> result = GlobalApp.WeighingBalance.CalculateFromHex(inData);

                decimal quantity = Convert.ToDecimal(result[0]) / 1000;
                // Use ChangeQuantity to Change/Update Quantity in TicketList
                //_currentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity = quantity;
                if (logicpos.Utils.CheckStocks())
                {
                    if(logicpos.Utils.ShowMessageMinimumStock(_sourceWindow, _currentOrderDetails.Lines[_listStoreModelSelectedIndex].ArticleOid, quantity))
                    {
                        ChangeQuantity(quantity);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ChangeQuantity(quantity);
                }
                //_listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Quantity);
                //_logger.Debug(string.Format("Quantity: {0}", _listStoreModel.GetValue(_treeIter, (int)TicketListColumns.Quantity)));
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        /// <summary>
        /// Helper method to get VatExemptionReason
        /// </summary>
        /// <returns>VatExemptionReason Guid</returns>
        private Guid GetVatExemptionReason()
        {
            Guid result = new Guid();

            CriteriaOperator criteria = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationVatExceptionReason>
              dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationVatExceptionReason>(
                _sourceWindow,
                DialogFlags.DestroyWithParent,
                    resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_vat_exemption_reason"),
                GlobalApp.MaxWindowSize,
                null, //XpoDefaultValue
                criteria,
                GenericTreeViewMode.Default,
                null  //pActionAreaButtons
              );

            int response = dialog.Run();
            if (response == (int)ResponseType.Ok)
            {
                result = dialog.GenericTreeView.DataSourceRow.Oid;
            }
            dialog.Destroy();

            return result;
        }
    }
}
