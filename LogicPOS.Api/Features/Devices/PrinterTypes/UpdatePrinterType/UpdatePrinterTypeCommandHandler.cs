﻿using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PrinterTypes.UpdatePrinterType
{
    public class UpdatePrinterTypeCommandHandler
         : RequestHandler<UpdatePrinterTypeCommand, ErrorOr<Unit>>
    {
        public UpdatePrinterTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePrinterTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"printers/types/{command.Id}", command, cancellationToken);
        }
    }
}
