using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using FormFilling.Models;

namespace FormFilling.Pages.Administration.Platform.ReportSupport
{
    public class UploadReportImageModel : PageModel
    {
        // Variables used by the page

        [BindProperty]
        public string ImageCode { get; set; } = String.Empty;

        [BindProperty]
        public int Height { get; set; }

        [BindProperty]
        public int Width { get; set; }

        [BindProperty]
        public string ImageType { get; set; } = String.Empty;

        [BindProperty]
        public IFormFile? FileContainingImage { get; set; }


        // injected variables

        private readonly DatabaseContext _context;

        // internal variables

        public UploadReportImageModel(
              DatabaseContext context
            )
        {
            _context = context;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //upload file to folder
            //if (FileContainingImage.Length > 0)
            //{
            //    using (var stream = new FileStream(Path.Combine(_hostenvironment.WebRootPath, "uploadfiles", FileContainingImage.FileName), FileMode.Create))
            //    {
            //        await FileContainingImage.CopyToAsync(stream);
            //    }
            //}
            if (FileContainingImage != null)
            {
                //save image to database.
                using (var memoryStream = new MemoryStream())
                {
                    await FileContainingImage.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        var tReportImage = new ReportImage()
                        {
                            ReportCode = ImageCode,
                            Height = Height,
                            Width = Width,
                            ImageFormat = ImageType,
                            Image = memoryStream.ToArray()
                        };
                        _context.ReportImages.Add(tReportImage);

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("FileContainingImage", "The file is too large.");
                    }
                }
            }
            return RedirectToPage("../Index");
        }

    }
}
