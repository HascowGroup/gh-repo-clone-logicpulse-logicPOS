﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.App;
using System;

namespace logicpos
{
    public partial class StartupWindow
    {
        private void TablePadUser_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            TablePadUser.SelectedButtonOid = button.CurrentButtonOid;

            //Assign User Detail to Member Reference
            AssignUserDetail();
        }

        private void ButtonKeyOK_Clicked(object sender, EventArgs e)
        {
            _numberPadPin.ProcessPassword(this, _selectedUserDetail);
        }

        private void ButtonKeyResetPassword_Clicked(object sender, EventArgs e)
        {
            //Require to store current Current Pin, else when we change mode it resets pin to messages
            string currentPin = _numberPadPin.EntryPin.Text;
            _numberPadPin.Mode = NumberPadPinMode.PasswordReset;
            //Restore Pin after UpdateLabelStatus triggered in mode
            _numberPadPin.EntryPin.Text = currentPin;
            _numberPadPin.ProcessPassword(this, _selectedUserDetail);
        }

        //Removed : Conflited with Change Password, When we Implement Default Enter Key in All Dilogs, It Trigger Twice
        private void StartupWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //if (args.Event.Key.ToString().Equals("Return"))
            //{
            //    _numberPadPin.ProcessPassword(this, _selectedUserDetail);
            //}
        }

        private void ButtonKeyQuit_Clicked(object sender, EventArgs e)
        {
            LogicPOSApp.Quit(this);
        }

        private void ButtonKeyFrontOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowFrontOffice(this);
        }

        private void ButtonKeyBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        //Assign Selected UserDetail to classe Member
        private void AssignUserDetail()
        {
            try
            {
                if (TablePadUser.SelectedButtonOid != null)
                {
                    _selectedUserDetail = (datalayer.App.DataLayerUtils.GetXPGuidObject(typeof(sys_userdetail), TablePadUser.SelectedButtonOid) as sys_userdetail);
                    if (_selectedUserDetail != null)
                    {
                        //Change NumberPadPinMode Mode
                        _numberPadPin.Mode = (_selectedUserDetail.PasswordReset) ? NumberPadPinMode.PasswordOld : NumberPadPinMode.Password;

                        if (_selectedUserDetail.PasswordReset)
                        {
                            //_logger.Debug(string.Format("Name: [{0}], PasswordReset: [{1}]", _selectedUserDetail.Name, _selectedUserDetail.PasswordReset));
                            Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResource(datalayer.App.DataLayerFramework.Settings["customCultureResourceDefinition"], "global_information"),
                                string.Format(resources.CustomResources.GetCustomResource(datalayer.App.DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_user_request_change_password"), _selectedUserDetail.Name, DataLayerSettings.DefaultValueUserDetailAccessPin)
                            );
                        }
                    }
                }

                //Grab Focus
                _numberPadPin.EntryPin.GrabFocus();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Main Logout User Method, Shared for FrontOffice and BackOffice
        public void LogOutUser(bool pShowStartup)
        {
            LogOutUser(pShowStartup, datalayer.App.DataLayerFramework.LoggedUser);
        }

        public void LogOutUser(bool pGotoStartupWindow, sys_userdetail pUserDetail)
        {
            if (
                SharedFramework.SessionApp.LoggedUsers.ContainsKey(pUserDetail.Oid))
            {
                SharedFramework.SessionApp.LoggedUsers.Remove(pUserDetail.Oid);
                SharedFramework.SessionApp.Write();
                SharedUtils.Audit("USER_loggerOUT", string.Format(resources.CustomResources.GetCustomResource(datalayer.App.DataLayerFramework.Settings["customCultureResourceDefinition"], "audit_message_user_loggerout"), pUserDetail.Name));
                //Only Reset LoggedUser if equal to pUser
                if (datalayer.App.DataLayerFramework.LoggedUser.Equals(pUserDetail))
                {
                    datalayer.App.DataLayerFramework.LoggedUser = null;
                    SharedFramework.LoggedUserPermissions = null;
                }
            }
            //Update Table, In case user change Users in BackOffice
            GlobalApp.StartupWindow.TablePadUser.UpdateSql();
            //Required to Assign Details to Update Select User
            AssignUserDetail();
            //Show Startup Windows, or Not (Silent Mode)
            if (pGotoStartupWindow) GlobalApp.StartupWindow.ShowAll();
        }
    }
}