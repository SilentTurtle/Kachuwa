using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Model
{
    [Table("Seo")]
    public class SEO
    {
        [Key]
        public int SEOId { get; set; }

        [Required]
        public string MetaTitle { get; set; }

        public string MetaKeyWords { get; set; } = "";

        [Required]
        public string MetaDescription { get; set; }
        [Required]
        public string SeoType { get; set; }//product /news/ category
       
        public string LastUrl { get; set; }
        [Required]
        //[Remote("CheckUrlExist", "Page", HttpMethod = "POST", ErrorMessage = "Url already in use!", AdditionalFields = "IsNew,OldUrl")]
        public string Url { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }
     
        public string Image { get; set; }
       
        public int ProductId { get; set; }
        [AutoFill(true)]
        public bool IsActive { get; set; }
        [IgnoreAll]
        public bool IsDeleted { get; set; }

        [IgnoreUpdate]
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}