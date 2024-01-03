﻿namespace AssetLoaningApplication.Models.Domain
{
    public class AddTransaction
    {
        public Guid requestingUserId { get; set; }

        public Guid supervisorId { get; set; }
        public Guid studentId { get; set; }

        public Guid assetId { get; set; }

        public string date { get; set; }


    }
}
