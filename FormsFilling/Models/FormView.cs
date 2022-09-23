using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class FormView
    {
        public string CurrentLanguage { get; set; } = "";
        public string AlternativeLanguage { get; set; } = "";
        public string? Header { get; set; } = "";
        public string? HeaderFormatClass { get; set; } = "";
        public string? Instructions { get; set; } = "";
        public string? InstructionFormatClass { get; set; } = "";
        public List<FormViewItem> ListOfFormItems { get; set; } = new List<FormViewItem>();
        public string? Footer { get; set; } = "";
        public string? FooterFormatClass { get; set; } = "";
    }
}
