namespace CallChecker.Models
{
    public class AzureEventGridWebHookValidation
    {
        public string id { get; set; }
        public string topic { get; set; }
        public string subject { get; set; }

        public ValidationData data { get; set; }

        public class ValidationData
        {
            public string validationCode { get; set; }
            public string validationUrl { get; set; }
        }

        public string eventType { get; set; }
        public string eventTime { get; set; }
        public string metadataVersion { get; set; }

        public string dataVersion { get; set; }

}
}