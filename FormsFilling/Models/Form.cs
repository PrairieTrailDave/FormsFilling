using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class Form
    {
        public int ID { get; set; }
        public string FormCode { get; set; } = "";
        public string? FormDescription { get; set; } 
        public string? FormPage { get; set; } 
        public int VersionID { get; set; }
        public DateTime VersionDate { get; set; }
        public bool IsActive { get; set; }

        public const int FormCodeLength = 10;
        public const int PageCodeLength = 20;
        public const int DescriptionLength = 30;
    }
}
