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
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime AddedOn { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }

    public class LocaleRegionViewModel : LocaleRegion
    {
        public string CountryName { get; set; }

    }
    public class LocaleRegionEditViewModel : LocaleRegion
    {
       public List<EditLocaleResource> Resources { get; set; }

    }
    public class EditLocaleResource
    {
        public int LocaleResourceId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string BaseValue { get; set; }
        public string GroupName { get; set; } = "";
        public string Culture { get; set; }
        public int RowTotal { get; set; }


    }
}