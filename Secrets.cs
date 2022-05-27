using System;

namespace dynamodb_charp_lambda
{
    public class Secrets
    {
        public string City { get; set; }
    }

    public class SecretsOptions
    {
        public int Count { get; set; }
        public string ApiKey { get; set; }
    }
}
