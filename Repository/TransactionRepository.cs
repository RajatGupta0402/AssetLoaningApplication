using AssetLoaningApplication.Data;
using AssetLoaningApplication.Models.Domain;
using AssetLoaningApplication.Models.DTO;

public class TransactionRepository
{
    private readonly AssetLoanDbContext dbContext;

    public TransactionRepository(AssetLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public object AddLoanAsset(AddTransaction loanAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            {

                var supervisor = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.supervisorId);
                var student = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.studentId);
                var asset = db.AssetDetails.FirstOrDefault(a => a.assetId == loanAssetTransaction.assetId);

                if (asset.availability != "available")
                {
                    throw new InvalidOperationException("Asset not available");
                }

                if (supervisor == null || supervisor.role != "Supervisor")
                {
                    throw new InvalidOperationException("Supervisor not found.");
                }
                else if (student == null || student.role != "Student")
                {
                    throw new InvalidOperationException("Student not found.");
                }
                else if (asset == null)
                {
                    throw new InvalidOperationException("Asset not found");
                }

                DateOnly date;
                try
                {
                    date = DateOnly.Parse(loanAssetTransaction.date);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException($"Invalid date format: {ex.Message}");
                }

                var transactionDetails = new TransactionDetails
                {
                    transactionId = Guid.NewGuid(),
                    SupervisorDetails = supervisor,
                    StudentDetails = student,
                    assetDetails = asset,
                    date = date,
                    loanedOrReturned = "loaned",
                    transactionType = "Loan"
                };

                db.TransactionDetails.Add(transactionDetails);
                asset.availability = "unavailable";
                db.AssetDetails.Update(asset);
                db.SaveChanges();


                return transactionDetails;
            }
        }
        catch (Exception ex)
        {
            
            throw;
        }
    }

    public object AddReturnAsset(AddTransaction returnAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            { 
            var returnAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == returnAssetTransaction.assetId);

            if (returnAsset == null)
            {
                throw new KeyNotFoundException("Asset not found");
            }

            var returnStudent = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.studentId);
            if (returnStudent == null || returnStudent.role != "Student")
            {
                throw new KeyNotFoundException("Student not found");
            }

            var loanTransaction = db.TransactionDetails
                .Where(a => a.assetId == returnAsset.assetId && a.studentId == returnStudent.userId && a.loanedOrReturned == "loaned")
                .FirstOrDefault();

            if (loanTransaction == null)
            {
                throw new KeyNotFoundException("Loaned transaction for this asset not found");
            }

            var asset = loanTransaction.assetDetails;
            var student = loanTransaction.StudentDetails;

            loanTransaction.loanedOrReturned = "returned";
            db.TransactionDetails.Update(loanTransaction);

            asset.availability = "available";
            db.AssetDetails.Update(asset);

            var loanId = loanTransaction.transactionId;

            DateOnly date = DateOnly.MaxValue;
            try
            {
                date = DateOnly.Parse(returnAssetTransaction.date);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Invalid date format: {ex.Message}");
            }

            var supervisor = db.UserDetails.Where(a => a.userId == returnAssetTransaction.supervisorId && a.role == "Supervisor").FirstOrDefault();

            if (supervisor != null)
            {
                var transactionDetails = new TransactionDetails
                {
                    transactionId = Guid.NewGuid(),
                    SupervisorDetails = supervisor,
                    StudentDetails = student,
                    assetDetails = asset,
                    date = date,
                    loanTransactionId = loanId,
                    loanedOrReturned = "returned",
                    transactionType = "Return"
                };
                loanTransaction.loanTransactionId = transactionDetails.transactionId;
                db.TransactionDetails.Update(loanTransaction);
                db.TransactionDetails.Add(transactionDetails);
                db.SaveChanges();

                return transactionDetails;
            }

            throw new KeyNotFoundException("Supervisor not found");
        }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public object UpdateLoanAsset(UpdateTransaction loanAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            {
                var transaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == loanAssetTransaction.transactionId);
                if (transaction == null || transaction.transactionType != "Loan")
                {
                    throw new InvalidOperationException("Transaction not found");
                }
                var supervisor = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.supervisorId);
                var student = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.studentId);
                var asset = db.AssetDetails.FirstOrDefault(a => a.assetId == loanAssetTransaction.assetId);

                if (asset.availability != "available" && asset.assetId != transaction.assetId)
                {
                    throw new InvalidOperationException("Asset not available");
                }

                if (supervisor == null || supervisor.role != "Supervisor")
                {
                    throw new InvalidOperationException("Supervisor not found.");
                }
                else if (student == null || student.role != "Student")
                {
                    throw new InvalidOperationException("Student not found.");
                }
                else if (asset == null)
                {
                    throw new InvalidOperationException("Asset not found");
                }

                var assetId = asset.assetId;
                DateOnly date;

                try
                {
                    date = DateOnly.Parse(loanAssetTransaction.date);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException($"Invalid date format: {ex.Message}");
                }

                transaction.date = date;

                if (transaction.assetDetails != asset)
                {
                    var changeAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == transaction.assetId);
                    if (changeAsset != null)
                    {
                        changeAsset.availability = "available";
                        db.AssetDetails.Update(changeAsset);
                    }

                    transaction.assetDetails = asset;
                }

                transaction.SupervisorDetails = supervisor;
                transaction.StudentDetails = student;

                db.TransactionDetails.Update(transaction);
                asset.availability = "unavailable";
                db.AssetDetails.Update(asset);

                db.SaveChanges();
                return transaction;
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public object UpdateReturnAsset(UpdateTransaction returnAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            {


                var transaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == returnAssetTransaction.transactionId);
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction id not found");
                }

                int f = 0;
                var asset = new AssetDetails();



                var student = new UserDetails();
                var supervisor = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.supervisorId);
                if (supervisor == null)
                {
                    throw new InvalidOperationException("Supervisor not found");
                }
                string s = returnAssetTransaction.date;

                var returnAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == returnAssetTransaction.assetId);
                if (returnAsset == null)
                {
                    throw new InvalidOperationException("Asset not found");
                }

                var returnStudent = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.studentId && a.role == "Student");
                if (returnStudent == null)
                {
                    throw new InvalidOperationException("Student not found");
                }

                var details = db.TransactionDetails.Where(a => a.assetId == returnAsset.assetId && a.studentId == returnStudent.userId).FirstOrDefault();
                if (details == null)
                {
                    throw new InvalidOperationException("Asset and student does not match or asset already returned");
                }
                if (transaction.assetDetails != details.assetDetails)
                {
                    var changeAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == transaction.assetId);
                    if (changeAsset == null)
                    {
                        throw new InvalidOperationException();
                    }
                   // Console.WriteLine(changeAsset.availability);
                    changeAsset.availability = "unavailable";
                  //  Console.WriteLine(changeAsset.availability);
                    var changeTransaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == transaction.loanTransactionId);
                    if (changeTransaction == null)
                    {
                        throw new InvalidOperationException();
                    }
                    changeTransaction.loanedOrReturned = "loaned";
                    transaction.assetDetails = details.assetDetails;
                    transaction.loanTransactionId = details.transactionId;
                    details.loanTransactionId = transaction.transactionId;
                    details.loanedOrReturned = "returned";
                    asset = details.assetDetails;
                    //Console.WriteLine(asset.availability);
                    asset.availability = "available";
                    //Console.WriteLine(asset.availability);
                    transaction.StudentDetails = details.StudentDetails;
                    db.AssetDetails.Update(changeAsset);
                    db.AssetDetails.Update(asset);
                    db.TransactionDetails.Update(details);
                    db.TransactionDetails.Update(changeTransaction);
                }
                DateOnly date;
                try
                {
                    date = DateOnly.Parse(s);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                transaction.date = date;
                transaction.SupervisorDetails = supervisor;
                db.TransactionDetails.Update(transaction);
                db.SaveChanges();
                return transaction;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public object GetLoanAsset(Guid userId,Guid transactionId)
    {
        try
        {
            using (var db = dbContext)
            {
                var getLoanTransaction = new GetLoanTransaction();
                var transaction = db.TransactionDetails.Where(a => a.transactionId == transactionId).FirstOrDefault();
                if (transaction != null && transaction.transactionType == "Loan")
                {
                    String supervisorName = "", studentName = "", assetName = "";
                    Guid? suid = null, stid = null, asid = null;
                    var user = db.UserDetails.Where(a => a.userId == userId).FirstOrDefault();
                    getLoanTransaction.transactionId = transactionId;
                    getLoanTransaction.transactionType = "Loan";
                    getLoanTransaction.loanedDate = transaction.date;
                    var supervisorDetails = db.UserDetails.FirstOrDefault(a => a.userId == transaction.supervisorId);
                    var studentDetails = db.UserDetails.FirstOrDefault(a => a.userId == transaction.studentId);
                    var assetDetails = db.AssetDetails.FirstOrDefault(a => a.assetId == transaction.assetId);
                    if (user.role == "Student" && studentDetails.userId != userId)
                    {
                        throw new InvalidOperationException("Unauthorized user");
                    }


                    getLoanTransaction.loaningSupervisorName = supervisorName;

                    getLoanTransaction.studentName = studentName;

                    getLoanTransaction.assetName = assetName;
                    suid = supervisorDetails.userId;
                    stid = studentDetails.userId;
                    asid = assetDetails.assetId;
                    supervisorName = supervisorDetails.firstName + " " + supervisorDetails.lastName;
                    studentName = studentDetails.firstName + " " + studentDetails.lastName;
                    assetName = assetDetails.name;
                    getLoanTransaction.loaningSupervisorId = Guid.Parse(suid.ToString());
                    getLoanTransaction.studentId = Guid.Parse(stid.ToString());
                    getLoanTransaction.assetId = Guid.Parse(asid.ToString());
                    getLoanTransaction.studentName = studentName;
                    getLoanTransaction.assetName = assetName;
                    getLoanTransaction.loaningSupervisorName = supervisorName;
                    if (transaction.loanTransactionId != null)
                    {
                        var loan = db.TransactionDetails.Where(a => a.transactionId == transaction.loanTransactionId).FirstOrDefault();
                        getLoanTransaction.receivedDate = loan.date;

                        getLoanTransaction.receivingSupervisorId = (Guid)loan.supervisorId;
                        getLoanTransaction.receivingSupervisorName = loan.SupervisorDetails.firstName + " " + loan.SupervisorDetails.lastName;




                        getLoanTransaction.receivedDate = loan.date;
                    }
                    return getLoanTransaction;
                }


                else
                {
                    throw new InvalidOperationException("Transaction id not found");
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public object GetReturnAsset(Guid userId, Guid transactionId)
    {
        try
        {
            using (var db = dbContext)
            {
                var getReturnTransaction = new GetReturnTransaction();
                var returnTransaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == transactionId);
                if (returnTransaction != null && returnTransaction.transactionType == "Return")
                {
                    String supervisorName = "", studentName = "", assetName = "";
                    Guid? suid = null, stid = null, asid = null;
                    var user = dbContext.UserDetails.Where(a => a.userId == userId).FirstOrDefault();
                    getReturnTransaction.receivingTransactionId = transactionId;
                    getReturnTransaction.transactionType = "Return";
                    getReturnTransaction.receivedDate = returnTransaction.date;
                    var supervisorDetails = db.UserDetails.FirstOrDefault(a => a.userId == returnTransaction.supervisorId);
                    var studentDetails = db.UserDetails.FirstOrDefault(a => a.userId == returnTransaction.studentId);
                    var assetDetails = db.AssetDetails.FirstOrDefault(a => a.assetId == returnTransaction.assetId);
                    if (user.role == "Student" && studentDetails.userId != userId)
                    {
                        throw new InvalidOperationException("Unauthorized user");
                    }




                    getReturnTransaction.assetName = assetName;
                    suid = supervisorDetails.userId;
                    stid = studentDetails.userId;
                    asid = assetDetails.assetId;
                    supervisorName = supervisorDetails.firstName + " " + supervisorDetails.lastName;
                    studentName = studentDetails.firstName + " " + studentDetails.lastName;
                    assetName = assetDetails.name;
                    getReturnTransaction.receivingSupervisorId = Guid.Parse(suid.ToString());
                    getReturnTransaction.studentId = Guid.Parse(stid.ToString());
                    getReturnTransaction.assetId = Guid.Parse(asid.ToString());
                    getReturnTransaction.studentName = studentName;
                    getReturnTransaction.assetName = assetName;
                    getReturnTransaction.receivingSupervisorName = supervisorName;
                    var loanTransaction = db.TransactionDetails.Where(a => a.transactionId == returnTransaction.loanTransactionId).FirstOrDefault();
                    getReturnTransaction.loaningSupervisorName = loanTransaction.SupervisorDetails.firstName + " " + loanTransaction.SupervisorDetails.lastName;
                    getReturnTransaction.loanedDate = loanTransaction.date;
                    getReturnTransaction.loanTransactionId = loanTransaction.transactionId;
                    return getReturnTransaction;
                }
                else
                {
                    throw new InvalidOperationException("Transaction id not found");
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public object GetByFilter(Guid userId, FilterTransaction filterTransaction)
    {
        try
        {
            using (var db = dbContext)
            {
                var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == userId);

                List<GetLoanTransaction> filteredTransactions = new List<GetLoanTransaction>();
                var transactions = new List<TransactionDetails>();
                var supervisorDetails = db.UserDetails.Where(a => a.role == "Supervisor").ToList();
                var studentDetails = db.UserDetails.Where(addLoanAsset => addLoanAsset.role == "Student").ToList();
                var assetDetails = db.AssetDetails.ToList();
                if (requestingUser.role == "Student")
                {
                    if (filterTransaction.studentId != null && userId != filterTransaction.studentId)
                    {
                        throw new InvalidOperationException("Unauthorized user");
                    }
                    else if (userId == filterTransaction.studentId || filterTransaction.studentId == null)
                    {
                        transactions = db.TransactionDetails.Where(a => a.studentId == userId).ToList();
                    }
                }
                else
                {
                    transactions = db.TransactionDetails.ToList();
                }
                foreach (var transaction in transactions)
                {
                    var getLoanTransaction = new GetLoanTransaction();
                    var tId = transaction.transactionId;
                    String supervisorName = "", studentName = "", assetName = "";
                    Guid? suid = null, stid = null, asid = null;
                    getLoanTransaction.transactionId = tId;
                    getLoanTransaction.transactionType = transaction.transactionType;
                    if (transaction.transactionType == "Return")
                    {
                        var supervisor = db.UserDetails.Where(a => a == transaction.SupervisorDetails).FirstOrDefault();
                        var loan = db.TransactionDetails.Where(a => a.transactionId == transaction.loanTransactionId).FirstOrDefault();
                        getLoanTransaction.loanedDate = loan.date;
                        suid = supervisor.userId;
                        getLoanTransaction.loaningSupervisorId = (Guid)loan.supervisorId;
                        getLoanTransaction.loaningSupervisorName = loan.SupervisorDetails.firstName + " " + loan.SupervisorDetails.lastName;
                        supervisorName = supervisor.firstName + " " + supervisor.lastName;
                        getLoanTransaction.receivingSupervisorId = Guid.Parse(suid.ToString());
                        getLoanTransaction.receivingSupervisorName = supervisorName;
                        getLoanTransaction.receivedDate = transaction.date;
                        getLoanTransaction.loanedDate = loan.date;
                    }
                    else if (transaction.transactionType == "Loan")
                    {
                        var supervisor = db.UserDetails.Where(a => a == transaction.SupervisorDetails).FirstOrDefault();
                        suid = supervisor.userId;
                        supervisorName = supervisor.firstName + " " + supervisor.lastName;
                        getLoanTransaction.loaningSupervisorName = supervisorName;
                        getLoanTransaction.loaningSupervisorId = Guid.Parse(suid.ToString());
                        getLoanTransaction.loanedDate = transaction.date;
                        if (transaction.loanTransactionId != null)
                        {
                            var loan = db.TransactionDetails.Where(a => a.transactionId == transaction.loanTransactionId).FirstOrDefault();
                            getLoanTransaction.receivedDate = loan.date;

                            getLoanTransaction.receivingSupervisorId = (Guid)loan.supervisorId;
                            getLoanTransaction.receivingSupervisorName = loan.SupervisorDetails.firstName + " " + loan.SupervisorDetails.lastName;




                            getLoanTransaction.receivedDate = loan.date;
                        }
                    }
                    var student = db.UserDetails.Where(a => a == transaction.StudentDetails).FirstOrDefault();
                    var asset = db.AssetDetails.Where(a => a == transaction.assetDetails).FirstOrDefault();


                    stid = student.userId;
                    studentName = student.firstName + " " + student.lastName;
                    getLoanTransaction.studentId = Guid.Parse(stid.ToString());
                    getLoanTransaction.studentName = studentName;
                    asid = asset.assetId;
                    assetName = asset.name;
                    getLoanTransaction.assetId = Guid.Parse(asid.ToString());
                    getLoanTransaction.assetName = assetName;
                    filteredTransactions.Add(getLoanTransaction);
                }
                List<GetLoanTransaction> varFilteredTransactions = new List<GetLoanTransaction>();
                //Console.WriteLine(filteredTransactions.Count+"HEllo");
                if (filterTransaction.studentId != null)
                {
                    //Console.WriteLine(filterTransaction.studentId);
                    //Console.WriteLine(filteredTransactions.Count);
                    foreach (var fT in filteredTransactions)
                    {

                        if (fT.studentId.Equals(filterTransaction.studentId))
                        {
                            varFilteredTransactions.Add(fT);

                        }
                    }
                    filteredTransactions.Clear();
                    filteredTransactions.AddRange(varFilteredTransactions);
                    varFilteredTransactions.Clear();
                }
                if (filterTransaction.supervisorId != null)
                {
                    foreach (var fT in filteredTransactions)
                    {
                        if (fT.transactionType == "Loan")
                        {
                            if (fT.loaningSupervisorId.Equals(filterTransaction.supervisorId))
                            {
                                varFilteredTransactions.Add(fT);
                            }
                        }
                        else if (fT.transactionType == "Return")
                        {
                            if (fT.receivingSupervisorId.Equals(filterTransaction.supervisorId))
                            {
                                varFilteredTransactions.Add(fT);
                            }
                        }
                    }
                    filteredTransactions.Clear();
                    filteredTransactions.AddRange(varFilteredTransactions);
                    varFilteredTransactions.Clear();
                }

                if (filterTransaction.assetId != null)
                {
                    foreach (var fT in filteredTransactions)
                    {
                        if (fT.assetId.Equals(filterTransaction.assetId))
                        {
                            varFilteredTransactions.Add(fT);
                        }
                    }
                    filteredTransactions.Clear();
                    filteredTransactions.AddRange(varFilteredTransactions);
                    varFilteredTransactions.Clear();
                }
                if (filterTransaction.date != null)
                {
                    DateOnly date = DateOnly.MaxValue;
                    try
                    {
                        date = DateOnly.Parse(filterTransaction.date);
                    }
                    catch (FormatException e)
                    {
                        throw new InvalidOperationException(e.Message);
                    }
                    foreach (var fT in filteredTransactions)
                    {
                        if (fT.transactionType == "Loan")
                        {
                            if (fT.loanedDate.Equals(date))
                            {
                                varFilteredTransactions.Add(fT);
                            }
                        }
                        if (fT.transactionType == "Return")
                        {
                            if (fT.receivedDate.Equals(date))
                            {
                                varFilteredTransactions.Add(fT);
                            }
                        }
                    }
                    filteredTransactions.Clear();
                    filteredTransactions.AddRange(varFilteredTransactions);
                    varFilteredTransactions.Clear();
                }
                return filteredTransactions;

            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
