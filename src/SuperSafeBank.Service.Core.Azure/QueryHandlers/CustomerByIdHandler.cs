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
    public class CustomerByIdHandler(IViewsContext dbContext) : IRequestHandler<CustomerById, CustomerDetails>
    {
        private readonly IViewsContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<CustomerDetails> Handle(CustomerById request, CancellationToken cancellationToken)
        {
            Response<ViewTableEntity> response = null;
            try
            {
                var key = request.CustomerId.ToString();

                response = await _dbContext.CustomersDetails.GetEntityAsync<ViewTableEntity>(
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

            return JsonSerializer.Deserialize<CustomerDetails>(response.Value.Data);
        }
    }
}