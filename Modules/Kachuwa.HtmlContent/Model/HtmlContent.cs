﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.HtmlContent.Model
{
    [Table("HtmlContent")]
    public class HtmlContent
    {
        [Key]
        public long HtmlContentId { get; set; }
        
        [Required]
        public string KeyName { get; set; }
        public string Content { get; set; }

        public bool IsMarkDown { get; set; }

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
