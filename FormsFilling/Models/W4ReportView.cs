using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using FormFilling.Models.DatabaseModels;
using FormFilling.Services;

namespace FormFilling.Models
{
    public class W4ReportView
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; } = "";

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string SSN { get; set; } = "";
        public string EmployeeAddress { get; set; } = "";
        public string EmployeeCity { get; set; } = "";
        public string EmployeeState { get; set; } = "";
        public string EmployeeZip { get; set; } = "";
        public string EmployeeCityStateZip { get; set; } = "";      
        public string SingleOrMarriedSeparately { get; set; } = "";
        public string MarriedFilingJointly { get; set; } = "";
        public string HeadOfHousehold { get; set; } = "";
        public string OnlyTwoJobsTotal { get; set; } = "";
        public string QualifyingChildrenAmount { get; set; } = "";
        public string OtherDependentsAmount { get; set; } = "";
        public string TotalDependentAmount { get; set; } = "";
        public string OtherIncome { get; set; } = "";
        public string Deductions { get; set; } = "";
        public string ExtraWithholding { get; set; } = "";
        public string Signed { get; set; } = "";
        public string Signature { get; set; } = "";
        public string SignedDate { get; set; } = "";
        public string EmployersName { get; set; } = "";
        public string EmployerAddress { get; set; } = "";
        public string FirstDateOfEmployment { get; set; } = "";
        public string EmployerEIN { get; set; } = "";


        public static W4ReportView ConvertToW4ReportView(W4Data? tData)
        {
            W4ReportView tView = new W4ReportView();
            if (tData == null)
            {
                tView.ID = 0;
                tView.FirstName = "Not";
                tView.LastName = "Found";
                tView.SSN = "";
                tView.EmployeeAddress = "";
                tView.EmployeeCityStateZip = "";
                tView.SingleOrMarriedSeparately = "";
                tView.MarriedFilingJointly = "";
                tView.HeadOfHousehold = "";
                tView.OnlyTwoJobsTotal = "";
                tView.QualifyingChildrenAmount = "";
                tView.OtherDependentsAmount = "";
                tView.TotalDependentAmount = "";
                tView.OtherIncome = "";
                tView.Deductions = "";
                tView.ExtraWithholding = "";
                tView.Signed = "";
                tView.SignedDate = "";
            }
            else
            {
                tView.ID = tData.ID;
                tView.FirstName = tData.FirstName;
                tView.LastName = tData.LastName;
                tView.SSN = tData.SSN;
                tView.EmployeeAddress = tData.EmployeeAddress ?? "";
                tView.EmployeeCityStateZip = tData.EmployeeCityStateZip ?? "";
                tView.SingleOrMarriedSeparately = XorSpace(tData.SingleOrMarriedSeparately);
                tView.MarriedFilingJointly = XorSpace(tData.MarriedFilingJointly);
                tView.HeadOfHousehold = XorSpace(tData.HeadOfHousehold);
                tView.OnlyTwoJobsTotal = XorSpace(tData.OnlyTwoJobsTotal);
                tView.QualifyingChildrenAmount = "";
                if (tData.QualifyingChildrenAmount.HasValue)
                    tView.QualifyingChildrenAmount = tData.QualifyingChildrenAmount.Value.ToString("N2");
                tView.OtherDependentsAmount = "";
                if (tData.OtherDependentsAmount.HasValue)
                    tView.OtherDependentsAmount = tData.OtherDependentsAmount.Value.ToString("N2");
                tView.TotalDependentAmount = "";
                if (tData.TotalDependentAmount.HasValue)
                    tView.TotalDependentAmount = tData.TotalDependentAmount.Value.ToString("N2");
                tView.OtherIncome = "";
                if (tData.OtherIncome.HasValue)
                    tView.OtherIncome = tData.OtherIncome.Value.ToString("N2");
                tView.Deductions = "";
                if (tData.Deductions.HasValue)
                    tView.Deductions = tData.Deductions.Value.ToString("N2");
                tView.ExtraWithholding = "";
                if (tData.ExtraWithholding.HasValue)
                    tView.ExtraWithholding = tData.ExtraWithholding.Value.ToString("N2");
                tView.Signed = XorSpace(tData.Signed);
                tView.SignedDate = "";
                if (tData.SignedDate.HasValue)
                    tView.SignedDate = tData.SignedDate.Value.ToShortDateString();
            }
            tView.EmployersName = "";
            tView.EmployerAddress = "";
            tView.FirstDateOfEmployment = "";
            tView.EmployerEIN = "";
            return tView;
        }
        public void SetSignature (string newSig)
        {
            Signature = newSig;
            Signed = "";
        }

        public void SetEmployerFields(Employer? TEmployer)
        {
            if (TEmployer == null) return;

            EmployersName = TEmployer.EmployerName;
            EmployerAddress = TEmployer.Address1 + " " + TEmployer.City + ", " + TEmployer.State + " " + TEmployer.Zipcode;
            EmployerEIN = TEmployer.EIN ?? "";
        }

        public void SetEmployeeFields(Employee? TEmployee)
        {
            if (TEmployee == null) return;
            EmployeeID = TEmployee.CompanyEmployeeId;
            FirstDateOfEmployment = TEmployee.HireDate.ToShortDateString();
        }

        private static string XorSpace(bool BooleanVariable)
        {
            if (BooleanVariable) return "X";
            return " ";
        }
    }
}
