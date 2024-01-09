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

    public TransactionDetails AddLoanAsset(AddTransaction loanAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            {
                var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.requestingUserId);
                if (requestingUser == null || requestingUser.role != Constants.supervisorRole)
                {
                    throw new KeyNotFoundException(Constants.requestingSupervisorNotFound);
                }
                var supervisor = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.supervisorId);
                var student = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.studentId);
                var asset = db.AssetDetails.FirstOrDefault(a => a.assetId == loanAssetTransaction.assetId);

                if (asset.availability != Constants.available)
                {
                    throw new InvalidOperationException(Constants.assetNotAvailable);
                }

                if (supervisor == null || supervisor.role != Constants.supervisorRole)
                {
                    throw new InvalidOperationException(Constants.supervisorNotFound);
                }
                else if (student == null || student.role != Constants.studentRole)
                {
                    throw new InvalidOperationException(Constants.studentNotFound);
                }
                else if (asset == null)
                {
                    throw new InvalidOperationException(Constants.assetNotFound);
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
                    loanedOrReturned = Constants.loaned,
                    transactionType = Constants.loanTransactionType
                };

                db.TransactionDetails.Add(transactionDetails);
                asset.availability = Constants.unavailable;
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

    public TransactionDetails AddReturnAsset(AddTransaction returnAssetTransaction)
    {
        try
        {
            
            using (var db = dbContext)
            {
                var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.requestingUserId);
                if (requestingUser == null || requestingUser.role != Constants.supervisorRole)
                {
                    throw new KeyNotFoundException(Constants.requestingSupervisorNotFound);
                }
                var returnAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == returnAssetTransaction.assetId);

            if (returnAsset == null)
            {
                throw new KeyNotFoundException(Constants.assetNotFound);
            }

            var returnStudent = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.studentId);
            if (returnStudent == null || returnStudent.role != Constants.studentRole)
            {
                throw new KeyNotFoundException(Constants.studentNotFound);
            }

            var loanTransaction = db.TransactionDetails
                .Where(a => a.assetId == returnAsset.assetId && a.studentId == returnStudent.userId && a.loanedOrReturned == "loaned")
                .FirstOrDefault();

            if (loanTransaction == null)
            {
                throw new KeyNotFoundException(Constants.notLoaned);
            }

            var asset = loanTransaction.assetDetails;
            var student = loanTransaction.StudentDetails;

            loanTransaction.loanedOrReturned = Constants.returned;
            db.TransactionDetails.Update(loanTransaction);

            asset.availability = Constants.available;
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

            var supervisor = db.UserDetails.Where(a => a.userId == returnAssetTransaction.supervisorId && a.role == Constants.supervisorRole).FirstOrDefault();

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
                        loanedOrReturned = Constants.returned,
                        transactionType = Constants.returnTransactionType
                };
                loanTransaction.loanTransactionId = transactionDetails.transactionId;
                db.TransactionDetails.Update(loanTransaction);
                db.TransactionDetails.Add(transactionDetails);
                db.SaveChanges();

                return transactionDetails;
            }

            throw new KeyNotFoundException(Constants.supervisorNotFound);
        }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public TransactionDetails UpdateLoanAsset(UpdateTransaction loanAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            {
                var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.requestingUserId);
                if (requestingUser == null || requestingUser.role != Constants.supervisorRole)
                {
                    throw new KeyNotFoundException(Constants.requestingSupervisorNotFound);
                }
                var transaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == loanAssetTransaction.transactionId);
                if (transaction == null || transaction.transactionType != Constants.loanTransactionType)
                {
                    throw new InvalidOperationException(Constants.transactionNotFound);
                }
                var supervisor = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.supervisorId);
                var student = db.UserDetails.FirstOrDefault(a => a.userId == loanAssetTransaction.studentId);
                var asset = db.AssetDetails.FirstOrDefault(a => a.assetId == loanAssetTransaction.assetId);

                if (asset.availability != Constants.available && asset.assetId != transaction.assetId)
                {
                    throw new InvalidOperationException(Constants.assetNotAvailable);
                }

                if (supervisor == null || supervisor.role != Constants.supervisorRole)
                {
                    throw new InvalidOperationException(Constants.supervisorNotFound);
                }
                else if (student == null || student.role != Constants.studentRole)
                {
                    throw new InvalidOperationException(Constants.studentNotFound);
                }
                else if (asset == null)
                {
                    throw new InvalidOperationException(Constants.assetNotFound);
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
                        changeAsset.availability = Constants.available;
                        db.AssetDetails.Update(changeAsset);
                    }

                    transaction.assetDetails = asset;
                }

                transaction.SupervisorDetails = supervisor;
                transaction.StudentDetails = student;

                db.TransactionDetails.Update(transaction);
                asset.availability = Constants.unavailable;
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

    public TransactionDetails UpdateReturnAsset(UpdateTransaction returnAssetTransaction)
    {
        try
        {
            using (var db = dbContext)
            {

                var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.requestingUserId);
                if (requestingUser == null || requestingUser.role != Constants.supervisorRole)
                {
                    throw new KeyNotFoundException(Constants.requestingSupervisorNotFound);
                }
                var transaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == returnAssetTransaction.transactionId);
                if (transaction == null)
                {
                    throw new InvalidOperationException(Constants.transactionNotFound);
                }

               // int f = 0;
               // var asset = new AssetDetails();



               // var student = new UserDetails();
                var supervisor = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.supervisorId);
                if (supervisor == null)
                {
                    throw new InvalidOperationException(Constants.supervisorNotFound);
                }
                string s = returnAssetTransaction.date;

                var returnAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == returnAssetTransaction.assetId);
                if (returnAsset == null)
                {
                    throw new InvalidOperationException(Constants.assetNotFound);
                }

                var returnStudent = db.UserDetails.FirstOrDefault(a => a.userId == returnAssetTransaction.studentId && a.role == "Student");
                if (returnStudent == null)
                {
                    throw new InvalidOperationException(Constants.studentNotFound);
                }

                var details = db.TransactionDetails.Where(a => a.assetId == returnAsset.assetId && a.studentId == returnStudent.userId).FirstOrDefault();
                if (details == null)
                {
                    throw new InvalidOperationException(Constants.mismatch);
                }
                if (transaction.assetDetails != details.assetDetails)
                {
                    var changeAsset = db.AssetDetails.FirstOrDefault(a => a.assetId == transaction.assetId);
                    if (changeAsset == null)
                    {
                        throw new InvalidOperationException();
                    }
                   // Console.WriteLine(changeAsset.availability);
                    changeAsset.availability = Constants.unavailable;
                  //  Console.WriteLine(changeAsset.availability);
                    var changeTransaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == transaction.loanTransactionId);
                    if (changeTransaction == null)
                    {
                        throw new InvalidOperationException();
                    }
                    changeTransaction.loanedOrReturned = Constants.loaned;
                    transaction.assetDetails = details.assetDetails;
                    transaction.loanTransactionId = details.transactionId;
                    details.loanTransactionId = transaction.transactionId;
                    details.loanedOrReturned = Constants.returned;
                    var asset = details.assetDetails;
                    //Console.WriteLine(asset.availability);
                    asset.availability = Constants.available;
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
    public GetLoanTransaction GetLoanAsset(Guid userId,Guid transactionId)
    {
        try
        {
            using (var db = dbContext)
            {
                var userDb = db.UserDetails.Where(a => a.userId == userId).FirstOrDefault();


                if (userDb == null)
                {
                    throw new KeyNotFoundException(Constants.userNotFound);
                }

                if (userDb.role != Constants.supervisorRole && userDb.role != Constants.studentRole)
                {
                    throw new UnauthorizedAccessException(Constants.unauthorizedUser);
                }
                var getLoanTransaction = new GetLoanTransaction();
                var transaction = db.TransactionDetails.Where(a => a.transactionId == transactionId).FirstOrDefault();
                if (transaction != null && transaction.transactionType == Constants.loanTransactionType)
                {
                    String supervisorName = "", studentName = "", assetName = "";
                    Guid? suid = null, stid = null, asid = null;
                    var user = db.UserDetails.Where(a => a.userId == userId).FirstOrDefault();
                    getLoanTransaction.loanTransactionId = transactionId;
                    getLoanTransaction.transactionType = "Loan";
                    getLoanTransaction.loanedDate = transaction.date;
                    var supervisorDetails = db.UserDetails.FirstOrDefault(a => a.userId == transaction.supervisorId);
                    var studentDetails = db.UserDetails.FirstOrDefault(a => a.userId == transaction.studentId);
                    var assetDetails = db.AssetDetails.FirstOrDefault(a => a.assetId == transaction.assetId);
                    if (user.role == Constants.studentRole && studentDetails.userId != userId)
                    {
                        throw new InvalidOperationException(Constants.unauthorizedUser);
                    }


                    //getLoanTransaction.loaningSupervisorName = supervisorName;

                    //getLoanTransaction.studentName = studentName;

                    //getLoanTransaction.assetName = assetName;
                    //suid = supervisorDetails.userId;
                    //stid = studentDetails.userId;
                    //asid = assetDetails.assetId;
                    //supervisorName = supervisorDetails.firstName + " " + supervisorDetails.lastName;
                    //studentName = studentDetails.firstName + " " + studentDetails.lastName;
                    //assetName = assetDetails.name;
                    getLoanTransaction.loaningSupervisorId = Guid.Parse(supervisorDetails.userId.ToString());
                    getLoanTransaction.studentId = Guid.Parse(studentDetails.userId.ToString());
                    getLoanTransaction.assetId = Guid.Parse(assetDetails.assetId.ToString());
                    getLoanTransaction.studentName = studentDetails.firstName + " " + studentDetails.lastName;
                    getLoanTransaction.assetName = assetDetails.name;
                    getLoanTransaction.loaningSupervisorName = supervisorDetails.firstName + " " + supervisorDetails.lastName; ;
                    if (transaction.loanTransactionId != null)
                    {
                        var loan = db.TransactionDetails.Where(a => a.transactionId == transaction.loanTransactionId).FirstOrDefault();
                        getLoanTransaction.receivedDate = loan.date;

                        getLoanTransaction.receivingSupervisorId = (Guid)loan.supervisorId;
                        getLoanTransaction.receivingSupervisorName = loan.SupervisorDetails.firstName + " " + loan.SupervisorDetails.lastName;

                        getLoanTransaction.receivingTransactionId=transaction.loanTransactionId;


                        getLoanTransaction.receivedDate = loan.date;
                    }
                    return getLoanTransaction;
                }


                else
                {
                    throw new InvalidOperationException(Constants.transactionNotFound);
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public GetLoanTransaction GetReturnAsset(Guid userId, Guid transactionId)
    {
        try
        {
            using (var db = dbContext)
            {
                var userDb = db.UserDetails.Where(a => a.userId == userId).FirstOrDefault();


                if (userDb == null)
                {
                    throw new KeyNotFoundException(Constants.userNotFound);
                }

                if (userDb.role != Constants.supervisorRole && userDb.role != Constants.studentRole)
                {
                    throw new UnauthorizedAccessException(Constants.unauthorizedUser);
                }
                var getReturnTransaction = new GetLoanTransaction();
                var returnTransaction = db.TransactionDetails.FirstOrDefault(a => a.transactionId == transactionId);
                if (returnTransaction != null && returnTransaction.transactionType == Constants.returnTransactionType)
                {
                    //String supervisorName = "", studentName = "", assetName = "";
                    //Guid? suid = null, stid = null, asid = null;
                    var user = dbContext.UserDetails.Where(a => a.userId == userId).FirstOrDefault();
                    getReturnTransaction.receivingTransactionId = transactionId;
                    getReturnTransaction.transactionType = Constants.returnTransactionType;
                    getReturnTransaction.receivedDate = returnTransaction.date;
                    var supervisorDetails = db.UserDetails.FirstOrDefault(a => a.userId == returnTransaction.supervisorId);
                    var studentDetails = db.UserDetails.FirstOrDefault(a => a.userId == returnTransaction.studentId);
                    var assetDetails = db.AssetDetails.FirstOrDefault(a => a.assetId == returnTransaction.assetId);
                    if (user.role == Constants.studentRole && studentDetails.userId != userId)
                    {
                        throw new InvalidOperationException(Constants.unauthorizedUser);
                    }




                    //suid = supervisorDetails.userId;
                    //stid = studentDetails.userId;
                    //asid = assetDetails.assetId;
                    //supervisorName = supervisorDetails.firstName + " " + supervisorDetails.lastName;
                    //studentName = studentDetails.firstName + " " + studentDetails.lastName;
                    //assetName = assetDetails.name;
                    getReturnTransaction.receivingSupervisorId = Guid.Parse(supervisorDetails.userId.ToString());
                    getReturnTransaction.studentId = Guid.Parse(studentDetails.userId.ToString());
                    getReturnTransaction.assetId = Guid.Parse(assetDetails.assetId.ToString());
                    getReturnTransaction.studentName = studentDetails.firstName + " " + studentDetails.lastName;
                    getReturnTransaction.assetName = assetDetails.name;
                    getReturnTransaction.receivingSupervisorName = supervisorDetails.firstName + " " + supervisorDetails.lastName;
                    var loanTransaction = db.TransactionDetails.Where(a => a.transactionId == returnTransaction.loanTransactionId).FirstOrDefault();

                    if (loanTransaction != null)
                    {
                        Console.WriteLine(loanTransaction.supervisorId);
                        getReturnTransaction.loaningSupervisorId = (Guid)loanTransaction.supervisorId;
                        var loaningSupervisorDetails = db.UserDetails.FirstOrDefault(a => a.userId == loanTransaction.supervisorId);
                       
                            getReturnTransaction.loaningSupervisorName = loaningSupervisorDetails.firstName + " " + loaningSupervisorDetails.lastName;
                        
                        getReturnTransaction.loanedDate = loanTransaction.date;
                        getReturnTransaction.loanTransactionId = loanTransaction.transactionId;
                    }
                    return getReturnTransaction;
                }
                else
                {
                    throw new InvalidOperationException(Constants.transactionNotFound);
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public List<GetLoanTransaction> GetByFilter(Guid userId, FilterTransaction filterTransaction)
    {
        try
        {
            using (var db = dbContext)
            {
                if (userId == null)
                {
                    throw new KeyNotFoundException(Constants.nullUserId);
                }
                var user = dbContext.UserDetails.FirstOrDefault(a => a.userId == userId);
                if (user == null)
                {
                    throw new KeyNotFoundException(Constants.userNotFound);
                }
                if (user.role != Constants.supervisorRole && user.role != Constants.studentRole)
                {
                    throw new UnauthorizedAccessException(Constants.unauthorizedUser);
                }
                var requestingUser = db.UserDetails.FirstOrDefault(a => a.userId == userId);

                List<GetLoanTransaction> filteredTransactions = new List<GetLoanTransaction>();
                var transactions = new List<TransactionDetails>();
                var supervisorDetails = db.UserDetails.Where(a => a.role == Constants.supervisorRole).ToList();
                var studentDetails = db.UserDetails.Where(addLoanAsset => addLoanAsset.role == Constants.studentRole).ToList();
                var assetDetails = db.AssetDetails.ToList();
                if (requestingUser.role == Constants.studentRole)
                {
                    if (filterTransaction.studentId != null && userId != filterTransaction.studentId)
                    {
                        throw new InvalidOperationException(Constants.unauthorizedUser);
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
                   // var tId = transaction.transactionId;
                    //String supervisorName = "", studentName = "", assetName = "";
                    //Guid? suid = null, stid = null, asid = null;
                    
                    getLoanTransaction.transactionType = transaction.transactionType;
                    if (transaction.transactionType == Constants.returnTransactionType)
                    {
                        getLoanTransaction.receivingTransactionId = transaction.transactionId;
                        var supervisor = db.UserDetails.Where(a => a == transaction.SupervisorDetails).FirstOrDefault();
                        var loan = db.TransactionDetails.Where(a => a.transactionId == transaction.loanTransactionId).FirstOrDefault();
                        getLoanTransaction.loanedDate = loan.date;
                        //suid = supervisor.userId;
                        getLoanTransaction.loaningSupervisorId = (Guid)loan.supervisorId;
                        getLoanTransaction.loaningSupervisorName = loan.SupervisorDetails.firstName + " " + loan.SupervisorDetails.lastName;
                        //supervisorName = supervisor.firstName + " " + supervisor.lastName;
                        getLoanTransaction.receivingSupervisorId = Guid.Parse(supervisor.userId.ToString());
                        getLoanTransaction.receivingSupervisorName = supervisor.firstName + " " + supervisor.lastName;
                        getLoanTransaction.receivedDate = transaction.date;
                        getLoanTransaction.loanedDate = loan.date;
                        getLoanTransaction.loanTransactionId = (Guid)transaction.loanTransactionId;
                    }
                    else if (transaction.transactionType == Constants.loanTransactionType)
                    {
                        getLoanTransaction.loanTransactionId = transaction.transactionId;
                        var supervisor = db.UserDetails.Where(a => a == transaction.SupervisorDetails).FirstOrDefault();
                       // suid = supervisor.userId;
                       // supervisorName = supervisor.firstName + " " + supervisor.lastName;
                        getLoanTransaction.loaningSupervisorName = supervisor.firstName + " " + supervisor.lastName;
                        getLoanTransaction.loaningSupervisorId = Guid.Parse(supervisor.userId.ToString());
                        getLoanTransaction.loanedDate = transaction.date;
                        if (transaction.loanTransactionId != null)
                        {
                            var loan = db.TransactionDetails.Where(a => a.transactionId == transaction.loanTransactionId).FirstOrDefault();
                            getLoanTransaction.receivedDate = loan.date;

                            getLoanTransaction.receivingSupervisorId = (Guid)loan.supervisorId;
                            getLoanTransaction.receivingSupervisorName = loan.SupervisorDetails.firstName + " " + loan.SupervisorDetails.lastName;


                            getLoanTransaction.receivingTransactionId = transaction.loanTransactionId;

                            getLoanTransaction.receivedDate = loan.date;
                        }
                    }
                    var student = db.UserDetails.Where(a => a == transaction.StudentDetails).FirstOrDefault();
                    var asset = db.AssetDetails.Where(a => a == transaction.assetDetails).FirstOrDefault();


                    //stid = student.userId;
                    //studentName = student.firstName + " " + student.lastName;
                    getLoanTransaction.studentId = Guid.Parse(student.userId.ToString());
                    getLoanTransaction.studentName = student.firstName + " " + student.lastName;
                    //asid = asset.assetId;
                   // assetName = asset.name;
                    getLoanTransaction.assetId = Guid.Parse(asset.assetId.ToString());
                    getLoanTransaction.assetName = asset.name;
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
                        if (fT.transactionType == Constants.loanTransactionType)
                        {
                            if (fT.loaningSupervisorId.Equals(filterTransaction.supervisorId))
                            {
                                varFilteredTransactions.Add(fT);
                            }
                        }
                        else if (fT.transactionType == Constants.returnTransactionType)
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
                        if (fT.transactionType == Constants.loanTransactionType)
                        {
                            if (fT.loanedDate.Equals(date))
                            {
                                varFilteredTransactions.Add(fT);
                            }
                        }
                        if (fT.transactionType == Constants.returnTransactionType)
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
