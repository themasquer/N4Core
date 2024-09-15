using N4Core.Records.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace N4Core.Accounts.Entities.Bases
{
    [Table("AccountGroups")]
    public class AccountGroupBase : Record, ISoftDelete, IModifiedBy
    {
        public string GroupName { get; set; } = null!;

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
