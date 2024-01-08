using AssetLoaningApplication.Data;
using AssetLoaningApplication.Models.Domain;
using AssetLoaningApplication.Models.DTO;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class TransactionService
{
    private readonly TransactionRepository transactionRepopsitory;

    public TransactionService(TransactionRepository repository)
    {
        this.transactionRepopsitory = repository; 
    }

    public object AddTransaction(string type, AddTransaction loanAssetTransaction)
    {
       





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

    public object UpdateTransaction(string type, UpdateTransaction loanAssetTransaction)
    {

        
           


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
    

    public object GetTransaction(string type,Guid userId, Guid transactionId)
    {
        
            





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

    public object GetByFilter(Guid userId, FilterTransaction filterTransaction)
    {
        
          
            return transactionRepopsitory.GetByFilter(userId, filterTransaction);
        
    }
}
