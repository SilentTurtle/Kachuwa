using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Kachuwa.Data.Crud;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web.Theme;

namespace Kachuwa.HtmlContent.Model
{
    [Table("HtmlContent")]
    public class HtmlContent
    {
        [Data.Crud.Attribute.Key]
        public long HtmlContentId { get; set; }

        [Validate("required")]
        [Required]
        public string KeyName { get; set; }
        [Editor]
        public string Content { get; set; }

        public bool IsMarkDown { get; set; }

        [AutoFill(AutoFillProperty.CurrentCulture)]
        public string Culture { get; set; }
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
