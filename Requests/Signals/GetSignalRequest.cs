namespace N17Solutions.Semaphore.Requests.Signals
{
    public abstract class GetSignalRequest
    {
        /// <summary>
        /// If set, will be used to decrypt the value of this Signal.
        /// </summary>
        public string PrivateKey { get; set; }
    }
}