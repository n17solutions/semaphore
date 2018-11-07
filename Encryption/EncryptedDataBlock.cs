namespace N17Solutions.Semaphore.Encryption
{
    public class EncryptedDataBlock
    {
        public string EncryptedData { get; set; }
        public string DigitalSignature { get; set; }
        public string InitialisationVector { get; set; }
        public string AesKey { get; set; }
    }
}