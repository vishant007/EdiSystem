public class PaymentService
{
    private readonly CosmosService _cosmosService;

    public PaymentService(CosmosService cosmosService)
    {
        _cosmosService = cosmosService;
    }

    // Fetch all payments
    public async Task<List<Payment>> GetPaymentsAsync()
    {
        return await _cosmosService.GetPaymentsAsync();
    }

    // Fetch payment by container number
    public async Task<Payment> GetPaymentByContainerAsync(string containerNumber)
    {
        return await _cosmosService.GetPaymentByContainerAsync(containerNumber);
    }
    

}
