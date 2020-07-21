﻿using Bawbee.Application.Query.Users.Documents;
using Bawbee.Application.Query.Users.Interfaces;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bawbee.Infra.Data.NoSQLRepositories
{
    public class EntryRavenDBRepository : IEntryReadRepository
    {
        private readonly IAsyncDocumentSession _session;

        public EntryRavenDBRepository(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<EntryDocument>> GetAllByUser(int userId)
        {
            return await _session.Query<EntryDocument>()
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }
    }
}