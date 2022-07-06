using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Localization
{
    [Table("LocaleRegion")]
    public class LocaleRegion
    {
        [Key]
        public int LocaleRegionId { get; set; }
        [Range(1,300, ErrorMessage = "Localization.SelectCountry")]
        public int CountryId { get; set; }
        public string Flag { get; set; }
        [Required(ErrorMessage = "Localization.EnterCulture")]
        public string Culture { get; set; }
        [AutoFill(false)]
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        [AutoFill(false)]
        [IgnoreUpdate]
        public bool IsDeleted { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        [IgnoreUpdate]
        [AutoFill(AutoFillProperty.CurrentUtcDate)]
        public DateTime AddedOn { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
        [IgnoreAll]
        public string Country { get; set; }
        [IgnoreAll]
        public string Iso { get; set; }

    }
  
}