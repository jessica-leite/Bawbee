﻿using Bawbee.Domain.Commands.Users;
using Bawbee.Domain.Core.Bus;
using Bawbee.Domain.Core.Commands;
using Bawbee.Domain.Core.Notifications;
using Bawbee.Domain.Entities;
using Bawbee.Domain.Events.Users;
using Bawbee.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Bawbee.Domain.CommandHandlers
{
    public class UserCommandHandler : BaseCommandHandler,
        ICommandHandler<RegisterNewUserCommand>
    {
        private readonly IMediatorHandler _mediator;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IUserReadRepository _userReadRepository;

        public UserCommandHandler(
            IMediatorHandler mediator,
            INotificationHandler<DomainNotification> notificationHandler,
            IUserWriteRepository userWriteRepository,
            IUserReadRepository userReadRepository) : base(mediator, notificationHandler)
        {
            _mediator = mediator;
            _userWriteRepository = userWriteRepository;
            _userReadRepository = userReadRepository;
        }

        public async Task<CommandResult> Handle(RegisterNewUserCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
            {
                SendNotificationsErrors(command);
                return CommandResult.Error();
            }

            var userDatabase = await _userReadRepository.GetByEmail(command.Email);

            if (userDatabase != null)
            {
                await _mediator.PublishEvent(new DomainNotification("E-mail already used."));
                return CommandResult.Error();
            }

            var user = new User(command.Name, command.LastName, command.Email, command.Password);
            await _userWriteRepository.Add(user);

            await _mediator.PublishEvent(new UserRegisteredEvent(user.Id, user.Name, user.LastName, user.Email, user.Password));
            return CommandResult.Ok(user.Id);
        }
    }
}
