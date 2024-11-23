using EDI315Payment.Models;
using EDI315Payment.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDI315Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly CosmosService _cosmosService;
        private readonly AzureServiceBusService _serviceBusService;

        public PaymentController(CosmosService cosmosService, AzureServiceBusService serviceBusService)
        {
            _cosmosService = cosmosService;
            _serviceBusService = serviceBusService;
        }

        // GET: api/EDI315Payment/{containerNumber}
        [HttpGet("{containerNumber}")]
        public async Task<IActionResult> GetPaymentDetails(string containerNumber)
        {
            var paymentDetails = await _cosmosService.GetPaymentDetailsAsync(containerNumber);
            if (paymentDetails == null)
            {
                return NotFound("Payment details not found.");
            }

            return Ok(new
            {
                paymentDetails.ContainerNumber,
                paymentDetails.FeeStatus,
                paymentDetails.TotalDemurrageFees,
                paymentDetails.OtherPayments
            });
        }

        // POST: api/EDI315Payment/UpdatePayments
        [HttpPost("UpdatePayments")]
        public async Task<IActionResult> UpdatePayments([FromBody] List<UpdatePaymentRequest> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest("No payment requests provided.");
            }

            try
            {
                // Update FeeStatus for all valid requests
                await _cosmosService.UpdateFeeStatusForMultipleContainersAsync(requests);

                // Generate a list of transaction IDs
                var transactionIds = new List<string>();
                foreach (var request in requests)
                {
                    var transactionId = Guid.NewGuid().ToString();
                    transactionIds.Add(transactionId);

                    // Send message to Azure Service Bus
                    var message = new
                    {
                        TransactionId = transactionId,
                        FeeStatus = "Paid",
                        ContainerNumber = request.ContainerNumber
                    };
                    await _serviceBusService.SendMessageAsync(message);
                }

                return Ok(new
                {
                    TransactionIds = transactionIds,
                    Message = "FeeStatus updated for all containers."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
