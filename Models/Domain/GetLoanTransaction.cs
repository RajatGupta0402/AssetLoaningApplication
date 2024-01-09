namespace AssetLoaningApplication.Models.Domain
{
    public class GetLoanTransaction
    {
        public Guid loanTransactionId { get; set; }
        public string transactionType { get; set; }
        public Guid loaningSupervisorId { get; set; }
        public string loaningSupervisorName { get; set; }
        public Guid studentId { get; set; }
        public string studentName { get; set; }
        public Guid assetId { get; set; }
        public string assetName { get; set; }
        public DateOnly loanedDate { get; set; }
        public Guid? receivingSupervisorId { get; set; } = null;
        public string? receivingSupervisorName { get; set; } = null;
        public DateOnly? receivedDate { get; set; } = null;
        public Guid? receivingTransactionId {  get; set; } = null;

    }
}
