using System;
using MediatR;
using N17Solutions.Semaphore.ServiceContract.Signals;

namespace N17Solutions.Semaphore.Requests.Signals
{
    public class CreateSignalRequest : SignalWriteModel, IRequest<Guid>
    { }
}