﻿using Bawbee.Application.Command.Users;
using Bawbee.Domain.Core.Bus;
using Bawbee.Domain.Core.Commands;
using Bawbee.Domain.Core.Notifications;
using Bawbee.Domain.Core.UnitOfWork;
using Bawbee.Domain.Entities;
using Bawbee.Domain.Events;
using Bawbee.Domain.Interfaces;
using Bawbee.Infra.CrossCutting.Common.Security;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Bawbee.Application.Users.Handlers
{
    public class UserCommandHandler : BaseCommandHandler,
        ICommandHandler<RegisterNewUserCommand>,
        ICommandHandler<LoginCommand>
    {
        private readonly IMediatorHandler _mediator;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;

        public UserCommandHandler(
            IUnitOfWork unitOfWork,
            IMediatorHandler mediator,
            INotificationHandler<DomainNotification> notificationHandler,
            IJwtService jwtService,
            IUserRepository userReadRepository) : base(mediator, unitOfWork, notificationHandler)
        {
            _mediator = mediator;
            _jwtService = jwtService;
            _userRepository = userReadRepository;
        }

        public async Task<CommandResult> Handle(RegisterNewUserCommand command, CancellationToken cancellationToken)
        {
            var userDatabase = await _userRepository.GetByEmail(command.Email);

            if (userDatabase != null)
            {
                await _mediator.PublishEvent(new DomainNotification("E-mail already used."));
                return CommandResult.Error();
            }

            var user = User.UserFactory.CreateNewPlataformUser(command.Name, command.LastName, command.Email, command.Password);
            
            await _userRepository.Add(user);

            if (await CommitTransaction())
            {
                //var userRegisteredEvent = new UserRegisteredEvent(
                //    user.UserId, user.Name, user.LastName, user.Email, 
                //    user.Password, user.BankAccounts, user.EntryCategories);

                var userRegisteredEvent = new UserRegisteredEvent(user);

                await _mediator.PublishEvent(userRegisteredEvent);
            }
            
            return CommandResult.Ok();
        }

        public async Task<CommandResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAndPassword(command.Email, command.Password);

            if (user == null)
            {
                await _mediator.PublishEvent(new DomainNotification("E-mail or password is invalid"));
                return CommandResult.Error();
            }

            var userAccessToken = _jwtService.GenerateSecurityToken(user.UserId, user.Name, user.Email);

            await _mediator.PublishEvent(new UserLoggedEvent(user.UserId, user.Name, user.Email));
            
            return CommandResult.Ok(userAccessToken);
        }
    }
}
