using System;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using N17Solutions.Semaphore.ServiceContract.Signals;

namespace N17Solutions.Semaphore.Requests.Signals
{
    public class PatchSignalRequest : GetSignalRequest, IRequest
    {
        /// <summary>
        /// The identifier of the Signal to patch
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The Patch Document that will be applied to the Signal to perform the patch request
        /// </summary>
        public JsonPatchDocument<SignalWriteModel> Patch { get; set; }
    }
}