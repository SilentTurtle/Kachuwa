using System;
using System.ComponentModel.DataAnnotations;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web
{
    [Table("Page")]
    public class Page
    {
        [Data.Crud.Attribute.Key]
        public long PageId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public string Content { get; set; }
        public bool UseMasterLayout { get; set; }
        public bool IsPublished { get; set; }
       
        [AutoFill(IsDate = true)]
        public DateTime LastModified { get; set; }
       
        [IgnoreAll]
        public DateTime LastRequested { get; set; }

        [AutoFill(AutoFillProperty.CurrentCulture)]
        public string Culture { get; set; }

        public bool IsActive { get; set; }

        
        [AutoFill(false)]
        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}