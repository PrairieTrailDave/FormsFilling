using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormFilling.Models
{
    [Table("FormPrompts")]
    public class FormPrompt
    {
        public int ID { get; set; }
        public string FormCode { get; set; } = "";
        public string LabelName { get; set; } = "";
        public string Language { get; set; } = "";
        public string? Prompt { get; set; } = "";

        public const int LanguageLength = 30;
        public const int PromptLength = 800;
    }
}
