﻿using Azure;
using MediatR;
using SuperSafeBank.Service.Core.Azure.Common.Persistence;
using SuperSafeBank.Service.Core.Common.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace SuperSafeBank.Service.Core.Azure.QueryHandlers
{
    public class AccountByIdHandler(IViewsContext dbContext) : IRequestHandler<AccountById, AccountDetails>
    {
        private readonly IViewsContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<AccountDetails> Handle(AccountById request, CancellationToken cancellationToken)
        {
            Response<ViewTableEntity> response = null;
            try
            {
                var key = request.AccountId.ToString();

                response = await _dbContext.Accounts.GetEntityAsync<ViewTableEntity>(
                   partitionKey: key,
                   rowKey: key,
                   cancellationToken: cancellationToken);
            }
            catch (RequestFailedException ex)
            {
                return null;
            }

            if (response?.Value is null)
                return null;

            return JsonSerializer.Deserialize<AccountDetails>(response.Value.Data);
        }
    }
}