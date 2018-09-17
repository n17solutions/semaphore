using MediatR;

namespace N17Solutions.Semaphore.Requests.Settings
{
    public class GetSettingRequest : IRequest<string>
    {
        /// <summary>
        /// The name of the Setting to get.
        /// </summary>
        public string Name { get; set; }
    }
}