﻿using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.AddCountry
{
    public class AddCountryCommandHandler : RequestHandler<AddCountryCommand, ErrorOr<Guid>>
    {
        public AddCountryCommandHandler(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
 
        public override async Task<ErrorOr<Guid>> Handle(
            AddCountryCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("countries",
                                                                     command,
                                                                     cancellationToken);
                var response = await httpResponse.Content.ReadAsStringAsync();

                return await HandleHttpResponseAsync(httpResponse);

            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
