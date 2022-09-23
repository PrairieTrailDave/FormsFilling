using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormFilling.Models
{
    public class FormInputTypes
    {

        public const string Address = "A";
        public const string CheckBox = "B";
        public const string CityState = "C";
        public const string Decimal = "D";
        public const string Date = "E";
        public const string TrueFalse = "F";
        public const string Signature = "G";
        public const string LastName = "L";
        public const string MiddleName = "M";
        public const string Number = "N";
        public const string Phone = "P";
        public const string FirstName = "R";
        public const string SSN = "S";
        public const string Text = "T";


        // build a list that can be used in a drop down

        public static List<FormInputType> GetFormInputTypes()
        {
            List<FormInputType> Types = new List<FormInputType>();
            Types.Add(new FormInputType { Name = "Text Input", Value = Text });
            Types.Add(new FormInputType { Name = "Check Box", Value = CheckBox });
            Types.Add(new FormInputType { Name = "First Name", Value = FirstName });
            Types.Add(new FormInputType { Name = "Last Name", Value = LastName });
            Types.Add(new FormInputType { Name = "Address", Value = Address });
            Types.Add(new FormInputType { Name = "City State", Value = CityState });
            Types.Add(new FormInputType { Name = "Social Security Number", Value = SSN });
            Types.Add(new FormInputType { Name = "Number", Value = Number });
            Types.Add(new FormInputType { Name = "Decimal", Value = Decimal });
            Types.Add(new FormInputType { Name = "Date", Value = Date });
            Types.Add(new FormInputType { Name = "True/False", Value = TrueFalse });
            Types.Add(new FormInputType { Name = "Signature", Value = Signature });
            return Types;
        }
    }


    public class FormInputType
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
