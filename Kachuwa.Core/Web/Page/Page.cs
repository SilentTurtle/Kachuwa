using System;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web
{
    [Table("Page")]
    public class Page
    {
        [Key]
        public long PageId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public string Content { get; set; }
        public bool IsPublished { get; set; }
        [Hide]
        public DateTime LastModified { get; set; }
        [Hide]
        public DateTime LastRequested { get; set; }
        public bool IsActive { get; set; }
        [Hide]
        public bool IsDeleted { get; set; }

        [AutoFill(IsDate = true)]
        public DateTime AddedOn { get; set; }
        [AutoFill(GetCurrentUser = true)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}