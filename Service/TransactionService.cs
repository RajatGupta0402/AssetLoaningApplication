using AssetLoaningApplication.Data;
using AssetLoaningApplication.Models.Domain;
using AssetLoaningApplication.Models.DTO;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class TransactionService
{
    private readonly AssetLoanDbContext dbContext;

    public TransactionService(AssetLoanDbContext dbContext)
    {
        this.dbContext = dbContext; 
    }

    public object AddTransaction(string type, AddTransaction loanAssetTransaction)
    {
        using (var db = dbContext)
        {


            var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.requestingUserId);
            if (requestingUser == null || requestingUser.role != "Supervisor")
            {
                throw new KeyNotFoundException("Requesting supervisor not found");
            }




            var transactionRepopsitory = new TransactionRepository(dbContext);

            if (type == "loan")
            {
                return transactionRepopsitory.AddLoanAsset(loanAssetTransaction);
            }
            else if (type == "return")
            {
                return transactionRepopsitory.AddReturnAsset(loanAssetTransaction);
            }
            else
            {
                throw new UnauthorizedAccessException("No action Found");
            }
        }

    }

    public object UpdateTransaction(string type, UpdateTransaction loanAssetTransaction)
    {

        using (var db = dbContext)
        {
            var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.requestingUserId);
            if (requestingUser == null || requestingUser.role != "Supervisor")
            {
                throw new KeyNotFoundException("Requesting supervisor not found");
            }

            var transactionRepopsitory = new TransactionRepository(dbContext);

            if (type == "loan")
            {
                return transactionRepopsitory.UpdateLoanAsset(loanAssetTransaction);
            }
            else if (type == "return")
            {
                return transactionRepopsitory.UpdateReturnAsset(loanAssetTransaction);
            }
            else
            {
                throw new UnauthorizedAccessException("No action Found");
            }
        }
    }

    public object GetTransaction(string type,Guid userId, Guid transactionId)
    {
        using (var db = dbContext)
        {
            var user = dbContext.UserDetails.Where(a => a.userId == userId).FirstOrDefault();


            if (user == null)
            {
                throw new KeyNotFoundException("User Not Found");
            }

            if (user.role != "Supervisor" && user.role != "Student")
            {
                throw new UnauthorizedAccessException("Unauthorised user");
            }



            var transactionRepopsitory = new TransactionRepository(dbContext);

            if (type == "loan")
            {
                return transactionRepopsitory.GetLoanAsset(userId, transactionId);
            }
            else if (type == "return")
            {
                return transactionRepopsitory.GetReturnAsset(userId, transactionId);
            }
            else
            {
                throw new UnauthorizedAccessException("No action Found");
            }
        }
    }

    public object GetByFilter(Guid userId, FilterTransaction filterTransaction)
    {
        using (var db = dbContext)
        {
            if (userId == null)
            {
                throw new KeyNotFoundException("User Id cannot be null");
            }
            var user = dbContext.UserDetails.FirstOrDefault(a => a.userId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User Not Found");
            }
            if (user.role != "Supervisor" && user.role != "Student")
            {
                throw new UnauthorizedAccessException("Unauthorized user");
            }
            var transactionRepopsitory = new TransactionRepository(dbContext);
            return transactionRepopsitory.GetByFilter(userId, filterTransaction);
        }
    }
}
