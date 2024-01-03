namespace AssetLoaningApplication.Models.Domain
{
    public class CheckLoan
    {

        public Guid transactionId { get; set; }
        public DateOnly date { get; set; }

        public Guid supervisorDetailsuserId { get; set; }
        public Guid studentDetailsuserId { get; set; }
        public Guid assetDetailsassetId { get; set; }
    }
}
