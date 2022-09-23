using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


using FormFilling.Models;
using FormFilling.Services;


namespace FormFilling.Pages.Administration.Company.Reports
{
    public class ShowW4ReportModel : PageModel
    {
        public string APIKey { get; set; } = string.Empty;

        public List<SelectListItem> SelectablePeople { get; set; } = new List<SelectListItem>();
        public string WhoToShow { get; set; } = "";

        // injected variables

        private readonly DatabaseContext _context;


        // internal variables

        public readonly string ActionName = "W4";
        public readonly string WhatPartOfSystem = "Report Services";


        public ShowW4ReportModel(
              DatabaseContext context
        )
        {
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync()
        {

            APIKey = Constants.UrlAPIKey;

            try
            {

                // need to get the people who have an W4 on file
                // may want to have a time range

                // the following gets the last W4 an employee entered.
                // The code that this was pulled from had a lot more complexity to the SQL

                var SQL = "SELECT Employees.* FROM Employees" +
                          "  JOIN W4Data W4 on Employees.ID = W4.EmployeeRecordID" +
                          " WHERE W4.ID in " +
                          "             (SELECT MAX(ID) OVER (PARTITION BY EmployeeRecordID)" +
                          "              FROM W4Data)" +
                          "         ";

                var res = _context.Employees.FromSqlRaw(SQL);

                var qu = from emp in res
                         select new SelectListItem
                         {
                             Text = emp.FirstName + " " + emp.LastName + " " + emp.CompanyEmployeeId,
                             Value = emp.ID.ToString()
                         };

                SelectablePeople = await (qu).ToListAsync();

            }
            catch (Exception Ex)
            {
                ModelState.AddModelError("", "A system error happened in " + ActionName);
            }
            return Page();
        }

    }
}
