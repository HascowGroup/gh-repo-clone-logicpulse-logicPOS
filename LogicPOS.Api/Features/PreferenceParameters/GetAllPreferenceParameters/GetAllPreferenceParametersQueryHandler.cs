﻿using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters
{
    public class GetAllPreferenceParametersQueryHandler : RequestHandler<GetAllPreferenceParametersQuery, ErrorOr<IEnumerable<PreferenceParameter>>>
    {
        public GetAllPreferenceParametersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PreferenceParameter>>> Handle(GetAllPreferenceParametersQuery query,
                                                                               CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<PreferenceParameter>>("preferenceparameters",
                                                                                  cancellationToken);
                return items;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
