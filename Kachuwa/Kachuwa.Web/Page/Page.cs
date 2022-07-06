using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web
{
    [Table("Page")]
    public class Page
    {
        [Key]
        public long PageId { get; set; }
        [Required(ErrorMessage ="Page.Name.Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Page.Url.Required")]
        public string Url { get; set; }
        public string Content { get; set; }
        public string ContentConfig { get; set; }
        public bool UseMasterLayout { get; set; }
        public bool IsPublished { get; set; }

        [AutoFill(false)]
        public bool IsBackend { get; set; }

        [AutoFill(false)]
        public bool IsSystem { get; set; }
        public int ModuleId { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime LastModified { get; set; }
       
        [IgnoreAll]
        public DateTime LastRequested { get; set; }

        [AutoFill(AutoFillProperty.CurrentCulture)]
        public string Culture { get; set; }

        public bool IsActive { get; set; }

        [AutoFill(false)]
        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long AddedBy { get; set; }


        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long DeletedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime DeletedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}