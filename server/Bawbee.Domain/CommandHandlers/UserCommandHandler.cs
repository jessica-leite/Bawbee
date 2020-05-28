﻿using Bawbee.Domain.Commands.Users;
using Bawbee.Domain.Core.Bus;
using Bawbee.Domain.Core.Notifications;
using Bawbee.Domain.Entities;
using Bawbee.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Bawbee.Domain.CommandHandlers
{
    public class UserCommandHandler : BaseCommandHandler,
        IRequestHandler<RegisterNewUserCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediatorHandler _mediator;
        private readonly IUserRepository _userRepository;

        public UserCommandHandler(
            IUnitOfWork uow, 
            IMediatorHandler mediator, 
            INotificationHandler<DomainNotification> notificationHandler,
            IUserRepository userRepository) : base(uow, mediator, notificationHandler)
        {
            _uow = uow;
            _mediator = mediator;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RegisterNewUserCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
            {
                SendNotificationsErrors(command);
                return false;
            }

            var alreadyExists = _userRepository.GetByEmail(command.Email) != null;

            if (alreadyExists)
            {
                await _mediator.PublishEvent(new DomainNotification("E-mail já está em uso"));
                return false;
            }

            var user = new User(command.Name, command.LastName, command.Email, command.Password);
            await _userRepository.Add(user);

            if (await CommitTransaction())
            {
                //_mediator.PublishEvent(new UserRegisteredEvent());
            }

            return true;
        }
    }
}
