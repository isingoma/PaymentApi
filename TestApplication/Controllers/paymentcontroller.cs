using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TestApplication.Logic;
using TestApplication.Objects;

namespace TestApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Paymentcontroller : ControllerBase
    {
        private readonly Processor _logic;

        public Paymentcontroller(Processor context)
        {
            _logic = context;
        }
        [HttpPost("GenerateJWTToken")]
        public async Task<IActionResult> GenerateAccessToken([FromBody] AccessTokenRequest Request)
        {
            AccessTokenResponse response = new AccessTokenResponse();
            try
            {
                if (string.IsNullOrEmpty(Request.username)
                    || string.IsNullOrEmpty(Request.password))
                {
                    return BadRequest(new ErrorResponse
                    {
                        StatusCode = "400",
                        StatusDescription = "Username or password cannot be null or empty."
                    });
                }
                else
                {
                    string accesstoken = await _logic.GenerateTokenAsync(Request.username);

                    bool valid = await _logic.IsValidAccessToken(accesstoken, Request.username);

                    if (valid)
                    {
                        response.validTime = "180s";
                        response.StatusDescription = "Token generated successfully";
                        response.StatusCode = "200";
                        response.Valid = true;
                        response.Accesstoken = accesstoken;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Ok(response);

        }


        [HttpPost("InitiatePayment")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request)
        {
            // Validate the payment request
            if (request == null)
            {
                return BadRequest("Invalid payment request.");
            }

            if (request.amount <= 0)
            {
                return BadRequest("Amount should be greater than zero.");
            }

            if (string.IsNullOrEmpty(request.currency))
            {
                return BadRequest("Currency is required.");
            }
            if (string.IsNullOrEmpty(request.username))
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = "400",
                    StatusDescription = "Username cannot be null or empty."
                });
            }
            if (string.IsNullOrEmpty(request.accesstoken))
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = "400",
                    StatusDescription = "Access Token cannot be null or empty."
                });
            }

            bool valid = await _logic.IsValidAccessToken(request.accesstoken, request.username);

            if (!valid) 
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = "400",
                    StatusDescription = "Invalid Access Token."
                });
            }
            else
            {
                // Example of calling a mock payment service
                var paymentGatewayResponse = await MockInitiatePaymentGateway(request);

                // Simulate a successful payment initiation response from the payment gateway
                if (paymentGatewayResponse.Status == "Completed")
                {
                    var paymentResponse = new PaymentResponse
                    {
                        PaymentId = Guid.NewGuid().ToString(),  // Simulated Payment ID
                        Status = "Pending",
                    };

                    // Optionally, save payment info to the database here

                    return Ok(paymentResponse);  // Return the response to the client
                }
                else
                {
                    return BadRequest(new ErrorResponse
                    {
                        StatusCode = "99",
                        StatusDescription = "Payment Failed."
                    });
                }
            }

        }
     
        [HttpGet("RetrievePayment/{paymentId}")]
        public IActionResult RetrievePayment(string paymentId)
        {
            PaymentDetails paymentDetails = new PaymentDetails();
            try
            {
                if (string.IsNullOrEmpty(paymentId))
                {
                    return BadRequest("Invalid payment request. PaymentId is required.");
                }

                // Simulate retrieving payment details based on PaymentId
                // Here you would typically query your database or payment gateway
                paymentDetails = new PaymentDetails
                {
                    PaymentId = paymentId,
                    Amount = 100.00m,
                    Status = "Completed",
                    Date = DateTime.Now
                };


            }
            catch (Exception ex)
            {

            }
            return Ok(paymentDetails);
        }
        [HttpGet("RetrievePaymentList")]
        public IActionResult RetrievePaymentList([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Invalid date range. Both start date and end date are required.");
            }

            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be greater than end date.");
            }

            // Simulate querying the database to retrieve payments within the specified date range
            var paymentList = new List<PaymentDetails>
    {
        new PaymentDetails { PaymentId = "1", Amount = 50.00m, Status = "Completed", Date = new DateTime(2024, 12, 1) },
        new PaymentDetails { PaymentId = "2", Amount = 75.00m, Status = "Completed", Date = new DateTime(2024, 12, 3) },
        new PaymentDetails { PaymentId = "3", Amount = 120.00m, Status = "Pending", Date = new DateTime(2024, 12, 5) }
    };

            // Filter payments based on the provided date range
            var filteredPayments = paymentList.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();

            return Ok(filteredPayments);
        }
        private async Task<PaymentResponse> MockInitiatePaymentGateway(PaymentRequest request)
        {
            // Simulate calling a payment service (this would be replaced by actual API calls to a payment gateway)
            await Task.Delay(500);  // Simulate network delay

            // Simulate a successful response
            return new PaymentResponse
            {
                Status = "Completed",
                PaymentId = Guid.NewGuid().ToString(),  //transaction ID
            };
        }
    }
}
