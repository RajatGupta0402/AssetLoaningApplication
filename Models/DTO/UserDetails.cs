using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace AssetLoaningApplication.Models.DTO
{
    public class UserDetails
    {
        [Key]
        public Guid userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string role { get; set; }

    }

}
