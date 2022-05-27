using Amazon.SecretsManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dynamodb_charp_lambda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretsController : ControllerBase
    {
        private readonly SecretsOptions options;

        public SecretsController(
            IOptions<SecretsOptions> options)
        {
            this.options = options.Value;
        }

        [HttpGet]
        public async void Get()
        {
            // This is from https://www.youtube.com/watch?v=wIuP2RKy4z4&t=380s
            // Also have a look at https://www.youtube.com/watch?v=PkLLP2tcd28&t=797s
            // Also have a look at https://www.youtube.com/watch?v=bBMSL4vInYU

            // get count directly from AWS Secrets manager
            var client = new AmazonSecretsManagerClient();
            var response = await client.GetSecretValueAsync(new Amazon.SecretsManager.Model.GetSecretValueRequest()
            {
                SecretId = "MyCountSecret"
                // to get the previous value use   ,VersionStage= "AWSPREVIOUS"
            });

            Console.WriteLine("Count from AWS Secret manager is " + int.Parse(response.SecretString));

            // Get count from appsettings.json.  We should actually be getting the value from the Secrets file but it's not 
            // working as it should (see notes in Startup.cs)
            int countFromAppSettings = options.Count;
            Console.WriteLine(countFromAppSettings);

            string apiKey = options.ApiKey;
            Console.WriteLine(apiKey);
        }
    }
}
