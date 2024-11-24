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
        private readonly SqlService _sqlService;

        public PaymentController(CosmosService cosmosService, AzureServiceBusService serviceBusService, SqlService sqlService)
        {
            _cosmosService = cosmosService;
            _serviceBusService = serviceBusService;
            _sqlService = sqlService;
        }

        // GET: api/EDI315Payment/ServiceBus
        [HttpGet("ServiceBus")]
        public async Task<IActionResult> GetServiceBusMessage()
        {
            var msgData = await _serviceBusService.ReceiveMessageAsync();
            if (msgData == null)
            {
                return NotFound("No message available in Service Bus.");
            }

            return Ok(new
            {
                msgData.UserId,
                msgData.ContainerNumber,
                msgData.TotalDemurrageFees,
                msgData.OtherPayments,
                msgData.FeeStatus
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
                // Fetch message from Service Bus
                var serviceBusMessage = await _serviceBusService.ReceiveMessageAsync();
                if (serviceBusMessage == null)
                {
                    return NotFound("No message available in Service Bus.");
                }

                string userId = serviceBusMessage.UserId;

                // Update FeeStatus for all valid requests
                await _cosmosService.UpdateFeeStatusForMultipleContainersAsync(requests);

                // Generate a list of transaction IDs and save them to SQL
                var transactionIds = new List<string>();
                foreach (var request in requests)
                {
                    var transactionId = Guid.NewGuid().ToString();
                    transactionIds.Add(transactionId);

                    // Create a SQL transaction record
                    var transaction = new PaymentSqlTransaction
                    {
                        TransactionId = transactionId,
                        UserId = userId,
                        TotalDemurrageFees = request.TotalDemurrageFees,
                        OtherPayments = request.OtherPayments,
                        TransactionDate = DateTime.UtcNow
                    };

                    // Save transaction to SQL
                    await _sqlService.SaveTransactionAsync(transaction);

                    // Send message back to Service Bus
                    var message = new
                    {
                        UserId = userId,
                        TransactionId = transactionId,
                        FeeStatus = "Paid",
                        ContainerNumber = request.ContainerNumber
                    };
                    await _serviceBusService.SendMessageAsync(message);
                }

                return Ok(new
                {
                    UserId = userId,
                    TransactionIds = transactionIds,
                    Message = "FeeStatus updated for all containers and transactions saved to SQL."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
