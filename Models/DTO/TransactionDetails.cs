using AssetLoaningApplication.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetLoaningApplication.Models.DTO
{
    public class TransactionDetails
    {
        [Key]
        public Guid transactionId { get; set; }

        public DateOnly date { get; set; }
        public Guid? supervisorId { get; set; }
        public Guid? studentId { get; set; }
        public string loanedOrReturned {  get; set; }
        public Guid? assetId { get; set; }
        [ForeignKey("supervisorId")]
        public virtual UserDetails SupervisorDetails { get; set; } = null!;

        [ForeignKey("studentId")]
        public virtual UserDetails StudentDetails { get; set; } = null!;
        [ForeignKey("assetId")]
        public virtual AssetDetails assetDetails { get; set; }
        public string transactionType { get; set; }

        public Guid? loanTransactionId { get; set; }



    }
}
