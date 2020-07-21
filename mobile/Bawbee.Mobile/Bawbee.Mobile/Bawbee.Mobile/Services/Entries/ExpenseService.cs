﻿using Bawbee.Mobile.Configs;
using Bawbee.Mobile.Models;
using Bawbee.Mobile.Models.Entries;
using Bawbee.Mobile.ReadModels.Entries;
using Bawbee.Mobile.Services.HttpRequestProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Bawbee.Mobile.Services.Entries
{
    public class ExpenseService
    {
        private static readonly string Endpoint = $"{AppConfiguration.BASE_URL}/api/v1/expenses";

        private readonly RequestProvider _httpClient;

        public ExpenseService()
        {
            _httpClient = new RequestProvider();
        }

        public async Task<ObservableCollection<EntryReadModel>> GetEntries()
        {
            try
            {
                var response = await _httpClient.GetAsync<ApiResponse<IEnumerable<EntryReadModel>>>(Endpoint);

                return new ObservableCollection<EntryReadModel>(response.Data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Add(Expense expense)
        {
            try
            {
                var json = JsonConvert.SerializeObject(expense);

                await _httpClient.PostAsync(Endpoint, expense);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}