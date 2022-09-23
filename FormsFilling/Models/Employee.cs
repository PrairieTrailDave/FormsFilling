using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FormFilling.Services;

namespace FormFilling.Models
{

    public class Employee
    {
        public int ID { get; set; }

        public string CompanyEmployeeId { get; set; } = "";


        public string FirstName{ get; set; } = "";

        public string? MiddleName{ get; set; }
        public string LastName { get; set; } = "";

        public string? EmployeeAddress1{ get; set; }

        public string? EmployeeCity { get; set; }
        public string? EmployeeState { get; set; }
        public string? EmployeeZip { get; set; }



        public string SSN { get; set; } = "";
        public DateTime HireDate { get; set; }
    } 

} 