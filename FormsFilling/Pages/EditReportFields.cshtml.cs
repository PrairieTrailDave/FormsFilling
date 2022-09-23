using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using FormFilling.Services;


namespace FormFilling.Pages
{
    public class EditReportFieldsModel : PageModel
    {
        public string APIKey { get; set; } = string.Empty;
        public void OnGet()
        {
            APIKey = Constants.UrlAPIKey;
        }
    }
}
