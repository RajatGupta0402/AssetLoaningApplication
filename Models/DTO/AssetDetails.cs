using System.ComponentModel.DataAnnotations;

namespace AssetLoaningApplication.Models.DTO
{
    public class AssetDetails
    {
        [Key]
        public Guid assetId { get; set; }
        public string serialNumber { get; set; }
        public string name { get; set; }

        public string model { get; set; }
        public string availability { get; set; } = "available";

    }
}
