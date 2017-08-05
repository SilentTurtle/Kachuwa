using System;
using System.ComponentModel.DataAnnotations;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Model
{
    [Table("Seo")]
    public class SEO
    {
        [Data.Crud.Attribute.Key]
        public int SEOId { get; set; }
        [Required]
        public string MetaTitle { get; set; }
        [TextArea]
        public string MetaKeyWords { get; set; }
        [Required]
        [TextArea]
        public string MetaDescription { get; set; }
        [Required]
        public string SeoType { get; set; }//product /news/ category
        [Hide]
        public string LastUrl { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string PageName { get; set; }

        [Image(Allow = ".jpg|.png",IsSingleFile = true)]
        public string Image { get; set; }
        [Hide]
        public int ProductId { get; set; }
        [AutoFill(true)]
        public bool IsActive { get; set; }
        [IgnoreAll]
        public bool IsDeleted { get; set; }

        [IgnoreUpdate]
        [AutoFill(IsDate = true)]
        public DateTime AddedOn { get; set; }

        [AutoFill(GetCurrentUser = true)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}