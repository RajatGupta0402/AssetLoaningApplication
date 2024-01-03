namespace AssetLoaningApplication.Models.Domain
{
    public class GetTransactionData
    {
        public string userType { get; set; }
        public Guid userId { get; set; }
        public Guid transactionId { get; set; }
    }
}
