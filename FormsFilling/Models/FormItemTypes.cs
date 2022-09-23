using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class FormItemTypes
    {
        public const string ItemTypeHeader = "H";
        public const string ItemTypeInstructions = "N";
        public const string ItemTypePrompt = "P";
        public const string ItemTypeInput = "I";

        // build a list that can be used in a drop down
        public static List<FormItemType> GetFormInputTypes()
        {
            List<FormItemType> Types = new()
            {
                new FormItemType { Name = "Header", Value = ItemTypeHeader },
                new FormItemType { Name = "Instructions", Value = ItemTypeInstructions },
                new FormItemType { Name = "Prompt", Value = ItemTypePrompt },
                new FormItemType { Name = "Input", Value = ItemTypeInput }
            };
            return Types;
        }

    }



    public class FormItemType
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
