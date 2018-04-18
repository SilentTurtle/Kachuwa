using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web.Layout;
using Kachuwa.Web.Model;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    [Table("Page")]
    public class Page
    {
        [Key]
        public long PageId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public string Content { get; set; }
        public string ContentConfig { get; set; }
        public bool UseMasterLayout { get; set; }
        public bool IsPublished { get; set; }

        [AutoFill(false)]
        public bool IsBackend { get; set; }

        [AutoFill(false)]
        public bool IsSystem { get; set; }

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
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }

    }

    public class PageConfigViewModel: Page
    {
        public LayoutContent Layout{ get; set; }
    }
    [Table("Page")]
    public class PageViewModel : SEO
    {
        public long PageId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Content { get; set; }
        public bool UseMasterLayout { get; set; }
        public bool IsPublished { get; set; }
        public bool IsNew { get; set; }
        public string OldUrl { get; set; }
        public bool IsBackend { get; set; }
    }
}