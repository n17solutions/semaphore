namespace N17Solutions.Semaphore.Domain.Model
{
    public class Setting
    {
        /// <summary>
        /// The identifier of the setting
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The name of the setting
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The value of the setting
        /// </summary>
        public string Value { get; set; }
    }
}