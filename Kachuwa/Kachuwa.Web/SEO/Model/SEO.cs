using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Model
{
    [Table("Seo")]
    public class SEO
    {
        [Key]
        public int SEOId { get; set; }

        [Required(ErrorMessage = "SEO.MetaTitle.Required")]
        public string MetaTitle { get; set; }

        public string MetaKeyWords { get; set; } = "";

        [Required(ErrorMessage = "SEO.MetaDescription.Required")]
        public string MetaDescription { get; set; }
        [Required(ErrorMessage = "SEO.SeoType.Required")]
        public string SeoType { get; set; }//product /news/ category
       [IgnoreAll]
       public IFormFile ImageFile { get; set; }
        public string LastUrl { get; set; }
        [Required(ErrorMessage = "SEO.Url.Required")]
        //[Remote("CheckUrlExist", "Page", HttpMethod = "POST", ErrorMessage = "Url already in use!", AdditionalFields = "IsNew,OldUrl")]
        public string Url { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }
     
        public string Image { get; set; }
       
        public int ProductId { get; set; }
        [AutoFill(true)]
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