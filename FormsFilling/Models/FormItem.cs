using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class FormItem
    {
        public int ID { get; set; }
        public string FormCode { get; set; } = "";
        public int DisplayOrder { get; set; }
        public string ItemType { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string DisplayFormatting { get; set; } = "";

        // Input type can be:
        //                 A - Address
        //                 B - checkBox
        //                 C - City state zip
        //                 D - Decimal
        //                 E - datE
        //                 F - true/False
        //                 G - siGnature
        //                 L - Last name
        //                 M - Middle name
        //                 N - Number
        //                 p - Phone
        //                 R - fiRst name
        //                 S - Ssn
        //                 T - Text 
        //
        public string? InputType { get; set; } = "";
        public string? InputFormatting { get; set; } = "";

        public const int ItemNameLength = 30;
        public const int DisplayFormatingLength = 40;
        public const int InputTypeLength = 1;
        public const int InputFormattingLength = 40;
    }
}
