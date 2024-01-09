namespace AssetLoaningApplication.Models.Domain
{
    public class Constants
    {
        public const string loanType = "loan";
        public const string returnType = "return";
        public const string loanTransactionType = "Loan";
        public const string returnTransactionType = "Return";
        public const string loaned = "loaned";
        public const string returned = "returned";
        public const string supervisorRole = "Supervisor";
        public const string studentRole = "Student";
        public const string available = "available";
        public const string unavailable = "unavailable";
        public const string studentNotFound = "Student Not Found";
        public const string supervisorNotFound = "Supervisor Not Found";
        public const string assetNotFound = "Assett Not Found";
        public const string assetNotAvailable = "Asset Not Available";
        public const string requestingSupervisorNotFound = "Requesting Supervisor Not Found";
        public const string notLoaned = "Loaned transaction for this asset not found";
        public const string transactionNotFound= "Transaction not found";
        public const string mismatch = "Asset and student does not match or asset already returned";
        public const string userNotFound = "User Not Found";
        public const string unauthorizedUser = "Unauthorised User";
        public const string nullUserId = "User Id cannot be null";

    }
}
