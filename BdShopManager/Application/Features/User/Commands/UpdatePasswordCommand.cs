﻿using Common.RequestWrapper;

namespace Application.Features.User.Commands
{
    public class UpdatePasswordCommand : ICommand<Guid>
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
