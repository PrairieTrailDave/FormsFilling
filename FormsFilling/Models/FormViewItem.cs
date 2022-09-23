using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class FormViewItem
    {
        public enum FormItemType { Label, Input };
        // Input type can be:
        //                 A - Address
        //                 B - checkBox
        //                 C - City state zip
        //                 D - Decimal
        //                 E - datE
        //                 F - true/False
        //                 L - Last name
        //                 M - Middle name
        //                 N - Number
        //                 p - Phone
        //                 R - fiRst name
        //                 S - Ssn
        //                 T - Text 
        //
        // These have to match the value clause in the options on the edit page
        public class FieldInputTypes
        {
            public const string Address = "Address";
            public const string CheckBox = "CheckBox";
            public const string CityState = "CityState";
            public const string Decimal = "Decimal";
            public const string Date = "Date";
            public const string TrueFalse = "TrueFalse";
            public const string LastName = "LastName";
            public const string MiddleName = "MiddleName";
            public const string Number = "Number";
            public const string Phone = "Phone";
            public const string FirstName = "FirstName";
            public const string SSN = "SSN";
            public const string Text = "Text";
    }


        public string TypeOfFormItem { get; set; } = "";
        public string FieldName { get; set; } = "";
        public string? DisplayFormattingClass { get; set; } = "";
        public string? DisplayPrompt { get; set; } = "";
        public PromptItem[] Prompt { get; set; } = new PromptItem[0];

        public string? InputType { get; set; } = "";
        public string? sInputType { get; set; } = "";
        public string? InputFormattingClass { get; set; } = "";
        public string? CurrentValue { get; set; } = "";
    }

    public class PromptItem
    {
        public string Language { get; set; } = "";
        public string PromptValue { get; set; } = "";
    }
}
