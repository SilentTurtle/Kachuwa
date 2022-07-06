﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("Restriction")]
    public class Restriction
    {
        [Key]
        public int RestrictionId { get; set; }
        public int RestrictionKeyId { get; set; }
        public string Value { get; set; }
        public string Reason { get; set; }
        public string Narration { get; set; }

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