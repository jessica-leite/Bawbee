﻿using Bawbee.Domain.Events;
using Bawbee.Domain.Events.BankAccounts;
using Bawbee.Domain.Events.Categories;
using Bawbee.Infra.Data.Documents;
using Bawbee.Infra.Data.ReadInterfaces;
using MediatR;
using Raven.Client.Documents.Session;
using System.Threading;
using System.Threading.Tasks;

namespace Bawbee.Infra.Data.RavenDB.EventHandlers
{
    public class UserRavenDBHandler :
        INotificationHandler<UserRegisteredEvent>,
        INotificationHandler<CategoryCreatedEvent>,
        INotificationHandler<BankAccountCreatedEvent>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IUserReadRepository _userReadRepository;

        public UserRavenDBHandler(IAsyncDocumentSession session, IUserReadRepository userReadRepository)
        {
            _session = session;
            _userReadRepository = userReadRepository;
        }

        public async Task Handle(UserRegisteredEvent @event, CancellationToken cancellationToken)
        {
            // TODO: automapper?

            var userDocument = new UserDocument();

            userDocument.UserId = @event.User.Id;
            userDocument.Name = @event.User.Name;
            userDocument.LastName = @event.User.LastName;
            userDocument.Email = @event.User.Email;
            userDocument.Password = @event.User.Password;

            foreach (var b in @event.User.BankAccounts)
            {
                userDocument.BankAccounts.Add(new BankAccountDocument
                {
                    BankAccountId = b.Id,
                    InitialBalance = b.InitialBalance,
                    Name = b.Name
                });
            }

            foreach (var ec in @event.User.Categories)
            {
                userDocument.EntryCategories.Add(new EntryCategoryDocument
                {
                    CategoryId = ec.Id,
                    Name = ec.Name
                });
            }

            await _session.StoreAsync(userDocument);

            // TODO: check
            //await _session.SaveChangesAsync();
        }

        public async Task Handle(CategoryCreatedEvent @event, CancellationToken cancellationToken)
        {
            var userDocument = await _userReadRepository.GetByUserId(@event.UserId);

            userDocument.EntryCategories.Add(new EntryCategoryDocument
            {
                CategoryId = @event.CategoryId,
                Name = @event.Name
            });

            await _session.StoreAsync(userDocument);

            // TODO: check
            //await _session.SaveChangesAsync();
        }

        public async Task Handle(BankAccountCreatedEvent @event, CancellationToken cancellationToken)
        {
            var userDocument = await _userReadRepository.GetByUserId(@event.UserId);

            userDocument.BankAccounts.Add(new BankAccountDocument
            {
                BankAccountId = @event.BankAccountId,
                Name = @event.Name,
                InitialBalance = @event.InitialBalance
            });

            await _session.StoreAsync(userDocument);
            
            // TODO: check
            //await _session.SaveChangesAsync();
        }
    }
}