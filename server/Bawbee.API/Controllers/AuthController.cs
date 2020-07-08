﻿using Bawbee.Application.Users.InputModels;
using Bawbee.Application.Users.Interfaces;
using Bawbee.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bawbee.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserApplication _userApplication;

        public AuthController(
            IUserApplication userApplication,
            INotificationHandler<DomainNotification> notificationHandler)
            : base(notificationHandler)
        {
            _userApplication = userApplication;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser(RegisterNewUserInputModel model)
        {
            var result = await _userApplication.Register(model);
            return Response(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var commandResult = await _userApplication.Login(model);
            return Response(commandResult);
        }
    }
}
