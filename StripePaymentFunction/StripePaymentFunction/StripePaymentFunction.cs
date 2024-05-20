using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Stripe;
using System.Web.Http;

namespace StripePaymentFunction
{
    public static class StripePaymentFunction
    {
        [FunctionName("StripePaymentFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //Read the data sent in request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripeSecretKey");

                var options = new PaymentIntentCreateOptions
                {
                    Amount = data.Amount,
                    Currency = data.Currency,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
                };

                //Requesting Stripe for payment
                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = service.Create(options);

                return new OkObjectResult(paymentIntent);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
