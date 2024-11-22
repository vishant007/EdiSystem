using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // Fetch all payments
    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var payments = await _paymentService.GetPaymentsAsync();
        return Ok(payments);
    }

    // Fetch payment by container number
    [HttpGet("{containerNumber}")]
    public async Task<IActionResult> GetPaymentByContainer(string containerNumber)
    {
        var payment = await _paymentService.GetPaymentByContainerAsync(containerNumber);

        if (payment == null)
        {
            return NotFound($"No payment details found for container: {containerNumber}");
        }

        return Ok(payment);
    }

}
