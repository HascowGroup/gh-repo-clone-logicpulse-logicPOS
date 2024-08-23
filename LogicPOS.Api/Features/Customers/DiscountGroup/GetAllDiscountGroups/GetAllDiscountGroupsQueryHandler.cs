﻿using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups
{
    public class GetAllDiscountGroupsQueryHandler :
        RequestHandler<GetAllDiscountGroupsQuery, ErrorOr<IEnumerable<DiscountGroup>>>
    {
        public GetAllDiscountGroupsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DiscountGroup>>> Handle(GetAllDiscountGroupsQuery request,
                                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                var discountGroup = await _httpClient.GetFromJsonAsync<List<DiscountGroup>>("discountgroups", cancellationToken);

                return discountGroup;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
