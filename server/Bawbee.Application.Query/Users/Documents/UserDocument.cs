﻿using Bawbee.Application.Query.Base;
using System;
using System.Collections.Generic;

namespace Bawbee.Application.Query.Users.Documents
{
    public class UserDocument : Document
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<BankAccountDocument> BankAccounts { get; set; }
        public List<EntryCategoryDocument> EntryCategories { get; set; }

        public UserDocument()
        {
            CreatedAt = DateTime.Now;
            BankAccounts = new List<BankAccountDocument>();
            EntryCategories = new List<EntryCategoryDocument>();
        }
    }
}