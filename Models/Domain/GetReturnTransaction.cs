namespace AssetLoaningApplication.Models.Domain
{
    public class GetReturnTransaction
    {
        public Guid receivingTransactionId { get; set; }
        public string transactionType { get; set; }
        public Guid receivingSupervisorId { get; set; }
        public string receivingSupervisorName { get; set; }
        public Guid studentId { get; set; }
        public string studentName { get; set; }
        public Guid assetId { get; set; }
        public string assetName { get; set; }
        public DateOnly receivedDate { get; set; }
        public Guid loanTransactionId { get; set; }
        public DateOnly loanedDate { get; set; }
        public string loaningSupervisorName { get; set; }
    }
}
