using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FormFilling.Models
{
    public class Employer
    {

        public Employer()
        {
        } // end public Employer()

        public string EmployerName{ get; set; } = "";

        public string Address1{ get; set; } = "";

        public string? Address2{ get; set; }
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Zipcode { get; set; } = "";

        public string? EIN{ get; set; }



    } // end public class Employer

} 
