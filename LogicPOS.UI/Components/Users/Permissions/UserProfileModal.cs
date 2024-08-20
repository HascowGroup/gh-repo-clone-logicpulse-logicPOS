﻿using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Users.Profiles.AddUserProfile;
using LogicPOS.Api.Features.Users.Profiles.UpdateUserProfile;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserProfileModal : EntityModal<UserProfile>
    {
        public UserProfileModal(
            EntityModalMode modalMode,
            UserProfile userProfile = null) : base(modalMode, userProfile)
        {
        }

        private AddUserProfileCommand CreateAddCommand()
        {
            return new AddUserProfileCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateUserProfileCommand CreateUpdateCommand()
        {
            return new UpdateUserProfileCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }

        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }
    }
}
