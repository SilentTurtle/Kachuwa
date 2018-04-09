using System.ComponentModel.DataAnnotations;
using MySqlX.XDevAPI.Relational;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.ContactUs
{
    [Table("ContactUs")]
    public class ContactUsInfo
    {
        [Key]
        public int Id { get;set;}
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }


    }
}