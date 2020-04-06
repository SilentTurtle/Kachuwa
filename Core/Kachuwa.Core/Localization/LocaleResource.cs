using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Localization
{

    [Table("LocaleResource")]
    public class LocaleResource
    {
        [Key]
        public int LocaleResourceId { get; set; }
        [Required(ErrorMessage = "Localization.Resource.Name.Required")]
        public string Name { get; set; }
        public string Value { get; set; }

        [Required(ErrorMessage = "Localization.Resource.Culture.Required")]
        public string Culture { get; set; }
        public string GroupName { get; set; } = "";

        [IgnoreAll]
        public int RowTotal { get; set; }


    }

    public class LocaleResourcesExportModel
    {
        public string CountryName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
     
        public string Culture { get; set; }
        public string GroupName { get; set; } = "";
    }
    public class LocaleResourcesImportModel
    {  public string CountryName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public string Culture { get; set; }
        public string GroupName { get; set; } = "";
      
    }

    public class LocaleResourcesImportViewModel
    {
        public IFormFile ImportFile { get; set; }
    }

    public class ImportedStatus
    {
        public bool IsImported { get; set; }
        public string Error { get; set; }
        public bool HasError { get; set; }
    }
}