﻿using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.AddCurrency
{
    public class AddCurrencyCommandHandler :
        RequestHandler<AddCurrencyCommand, ErrorOr<Guid>>
    {
        public AddCurrencyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCurrencyCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("currencies", command, cancellationToken);
        }
    }
}
