namespace AssetLoaningApplication.Models.Domain
{
    public class UpdateReturnAssetTransaction
    {
        public string userType { get; set; }
        public Guid requestingSupervisorId { get; set; }
        public Guid transactionId { get; set; }
        public Guid recievingSupervisorId { get; set; }
        public Guid studentId { get; set; }

        public Guid assetId { get; set; }

        public string date { get; set; }

    }
}
