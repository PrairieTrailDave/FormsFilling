using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using FormFilling.Services;

namespace FormFilling.Models.DatabaseModels
{
    [Table("W4Data")]
    public class W4Data
    {
        public int ID { get; set; }
        public int EmployeeRecordID { get; set; }
        public long TaskID { get; set; }
        public int FormID { get; set; }

        public string FirstName{ get; set; } = "";

        public string LastName{ get; set; } = "";

        public string SSN{ get; set; } = "";

        public string? EmployeeAddress { get; set; }
        public string? EmployeeCityStateZip { get; set; }
        public bool SingleOrMarriedSeparately { get; set; }
        public bool MarriedFilingJointly { get; set; }
        public bool HeadOfHousehold { get; set; }
        public bool OnlyTwoJobsTotal { get; set; }
        public decimal? QualifyingChildrenAmount { get; set; }
        public decimal? OtherDependentsAmount { get; set; }
        public decimal? TotalDependentAmount { get; set; }
        public decimal? OtherIncome { get; set; }
        public decimal? Deductions { get; set; }
        public decimal? ExtraWithholding { get; set; }
        public bool Signed { get; set; }
        public DateTime? SignedDate { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<W4Data>()
                .Property(prop => prop.QualifyingChildrenAmount)
                .HasPrecision(14, 2);

            builder.Entity<W4Data>()
               .Property(prop => prop.OtherDependentsAmount)
               .HasPrecision(14, 2);

            builder.Entity<W4Data>()
               .Property(prop => prop.TotalDependentAmount)
               .HasPrecision(14, 2);

            builder.Entity<W4Data>()
               .Property(prop => prop.OtherIncome)
               .HasPrecision(14, 2);

            builder.Entity<W4Data>()
               .Property(prop => prop.Deductions)
               .HasPrecision(14, 2);

            builder.Entity<W4Data>()
               .Property(prop => prop.ExtraWithholding)
               .HasPrecision(14, 2);

        }
    }
}
