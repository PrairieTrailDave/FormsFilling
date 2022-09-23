using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class FormEditView
    {
        public string APIKey { get; set; } = "";
        public int ID { get; set; }
        public string FormCode { get; set; } = "";
        public string? FormDescription { get; set; } = "";
        public string? FormPage { get; set; } = "";
        public int VersionID { get; set; }

        public List<FormViewItem> ListOfFormItems { get; set; } = new List<FormViewItem>();

    }
}
