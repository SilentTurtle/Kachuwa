using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServer
{
    [Table("TemporaryRoleDetail")]
    public class TemporaryRoleDetail
    {

        [Key]
        public long Id { get; set; }
        public long RoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Narration { get; set; }
    }
}