namespace AssetLoaningApplication.Models.Domain
{
    public class FilterTransaction
    {

        public Guid? supervisorId { get; set; }
        public Guid? studentId { get; set; }

        public Guid? assetId { get; set; }
        public string? date { get; set; }
        //public string? year { get; set; }
        //public string? month { get; set; }
        //public string? day { get; set; }

    }
}
