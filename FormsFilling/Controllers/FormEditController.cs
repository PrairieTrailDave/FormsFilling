using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FormFilling.Models;
using FormFilling.Models.DatabaseModels;
using FormFilling.Services;



namespace FormFilling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]



    public class FormEditController : Controller
    {

        // injected variables

        private readonly DatabaseContext _context;

        // internal variables

        public readonly string WhatPartOfSystem = "Form Edit";


        public FormEditController(
        DatabaseContext context
        )
        {
            _context = context;
        }





        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]

        public async Task<List<string>> GetAllReports()
        {
            List<string> Reports = await _context.ReportImages.Select(ri => ri.ReportCode).ToListAsync();
            return Reports;
        }



        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]

        public List<string?> GetFieldNames (string FormName)
        {
            List<string?> Fields = new List<string?>();

            switch(FormName)
            {

                case "W4":
                    var w4members = typeof(W4ReportView).GetProperties();
                    foreach (var member in w4members)
                        Fields.Add(member.Name);
                    break;

                default:
                    break;
            }
            return Fields;
        }



        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]

        public async Task<List<ReportLayoutField>> GetAllFields(string FormName)
        {
            List<ReportLayoutField> Fields = await _context.ReportLayoutFields.Where(lf => lf.ReportCode == FormName).ToListAsync();
            return Fields;
        }



        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]

        public List<string> GetAllFonts()
        {
            List<string> Fonts = new List<string>();
            foreach (FontFamily oneFontFamily in FontFamily.Families)
            {
                Fonts.Add(oneFontFamily.Name);
            }
            return Fonts;
        }

        // a return class to send the font to the editor page
        public class fontClass
        {
            public string fontName { get; set; } = String.Empty;
            public int fontSize { get; set; }
            public bool boldChecked { get; set; }
            public bool italicChecked { get; set; }
        }

        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]

        public async Task<fontClass> GetReportFont(string FormName)
        {
            ReportFont? REF = await _context.ReportFonts.Where(fnt => fnt.ReportCode == FormName).FirstOrDefaultAsync();
            if (REF != null)
            {
                bool isBold = (REF.FontStyle & ReportFont.BoldStyle) == 1;
                bool isItalic = (REF.FontStyle & ReportFont.ItalicStyle) == 1;
                fontClass retvar = new() 
                {
                    fontName = REF.FontName,
                    fontSize = REF.FontSize,
                    boldChecked = isBold,
                    italicChecked = isItalic
                };

                return retvar;
            }
            return new fontClass()
            {
                fontName = "Courier New",
                fontSize = 80,
                boldChecked = true,
                italicChecked = false
        };
        }



        // Background color 
        private readonly Color bgColor = Color.FromArgb(0xe9, 0xec, 0xef);
        // Code color 
        private readonly Color codeColor = Color.FromArgb(0x0, 0x0, 0x0);


        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]
        public async Task GetImageAsync([FromQuery] string? FormName)
        {
            // this returns a bitmap of the report page

            try
            {
                if (FormName != null)
                {
                    var fieldsToMap = await _context.ReportLayoutFields.Where(rp => rp.ReportCode == FormName).ToListAsync();
                    var (content, contentType) = await GeneratePage(FormName, fieldsToMap);
                    HttpContext.Response.ContentType = contentType;
                    await HttpContext.Response.BodyWriter.WriteAsync(content);
                }
            }
            catch (Exception Ex)
            {
                // need to create an image with the message
                var bytes = Encoding.UTF8.GetBytes(Ex.Message);
                await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
        }





















        [HttpPost("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePosition([FromForm] string APIKey, [FromForm] string? formName, [FromForm] string? fieldName, [FromForm] int? currentFieldX, [FromForm] int? currentFieldY, [FromForm] int? length, [FromForm] int? height, [FromForm] string fieldType)
        {
            try
            {
                if (APIKey == null) return BadRequest("API key"); 
                if (APIKey != Constants.UrlAPIKey)
                {
                    await Task.Delay(1000);
                    return BadRequest("API key");
                }
                string FieldType = ReportLayoutField.TextType;
                if (fieldType == "imag") FieldType = ReportLayoutField.ImageType;

                ReportLayoutField? REF = await _context.ReportLayoutFields.Where(fld => (fld.ReportCode == formName) && (fld.FieldName == fieldName)).FirstOrDefaultAsync();
                if (REF != null)
                {
                    REF.Xposition = (int)currentFieldX!;
                    REF.Yposition = (int)currentFieldY!;
                    if (length != null)
                        REF.Length = (int)length;
                    if (height != null)
                        REF.Height = (int)height;
                    REF.FieldType = FieldType;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    REF = new ReportLayoutField
                    {
                        ReportCode = formName!,
                        FieldName = fieldName ?? "Test Field",
                        Xposition = (int)currentFieldX!,
                        Yposition = (int)currentFieldY!,
                        Length = (int)length!,
                        Height = (int)height!,
                        FieldType = FieldType
                    };
                    _context.ReportLayoutFields.Add(REF);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception Ex)
            {
                // log the error
            }
            return Ok();
        }


        [HttpPost("[Action]")]
        [AllowAnonymous]
        public async Task UpdateFont([FromForm] string formName, [FromForm] string fontName, [FromForm] int fontSize, [FromForm] bool boldChecked, [FromForm] bool italicChecked)
        {
            try
            {
                int tStyle = 0;
                if (boldChecked) tStyle += ReportFont.BoldStyle;
                if (italicChecked) tStyle += ReportFont.ItalicStyle;

                ReportFont? REF = await _context.ReportFonts.Where(fnt => fnt.ReportCode == formName).FirstOrDefaultAsync();
                if (REF != null)
                {
                    REF.ReportCode = formName;
                    REF.FontName = fontName;
                    REF.FontSize = fontSize;
                    REF.FontStyle = tStyle;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    REF = new ReportFont
                    {
                        ReportCode = formName,
                        FontName = fontName,
                        FontSize = fontSize,
                        FontStyle = tStyle
                    };
                    _context.ReportFonts.Add(REF);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception Ex)
            {
                // log the error
            }
        }




        private async Task<(byte[] content, string contentType)> GeneratePage(string ReportCode, List<ReportLayoutField> FieldsToMap)
        {
            // Setup output format
            var contentType = "image/bmp";
            // Image width
            //const int imageWidth = 150;
            // Image height
            //const int imageHeight = 50;
            using var ms = new MemoryStream();


            // Create the graphics 
            //using Graphics graphics = Graphics.FromImage(bitmap);
            ReportImage? tReportImage = _context.ReportImages.Where(di => di.ReportCode == ReportCode).FirstOrDefault();
            if (tReportImage != null)
            {
                byte[] ImageFromDisk = tReportImage.Image;
                MemoryStream FD = new MemoryStream(ImageFromDisk);
                Image img = Image.FromStream(FD);
                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                // Create the bitmap to work with
                //using Bitmap bitmap = new Bitmap(tReportImage.Width, tReportImage.Height);
                //ImageFormatConverter converter = new ImageFormatConverter();
                //converter.ConvertTo(null, null, img, System.Drawing.Imaging.ImageFormat.Bmp)
                //img.Save(bitmap, System.Drawing.Imaging.ImageFormat.Bmp);
                MemoryStream bmpms = new MemoryStream();
                img.Save(bmpms, System.Drawing.Imaging.ImageFormat.Bmp);
                Bitmap bmp = new Bitmap(new Bitmap(bmpms));
                using Graphics graphics = Graphics.FromImage(bmp);

                // get the report font 

                FontFamily SelectedFont;
                FontStyle ReportStyle;
                float FontSize;

                ReportFont? REF = await _context.ReportFonts.Where(fnt => fnt.ReportCode == ReportCode).FirstOrDefaultAsync();
                if (REF != null)
                {
                    bool isBold = (REF.FontStyle & ReportFont.BoldStyle) == 1;
                    bool isItalic = (REF.FontStyle & ReportFont.ItalicStyle) == 1;
                    SelectedFont = new FontFamily(REF.FontName);
                    ReportStyle = FontStyle.Regular;
                    if (isBold)
                        ReportStyle = FontStyle.Bold;
                    if (isItalic)
                        ReportStyle = FontStyle.Italic;
                    FontSize = (float)Convert.ToDecimal(REF.FontSize);
                }
                else
                {
                    SelectedFont = new FontFamily("Courier New");
                    ReportStyle = FontStyle.Bold;
                    FontSize = 80.0F;
                }

                

                //FontStyle tStyle = new FontStyle();
                //if (boldChecked) tStyle = FontStyle.Bold;
                //if (italicChecked) tStyle = FontStyle.Italic;

                using (Font font = new Font(SelectedFont, FontSize, ReportStyle, GraphicsUnit.Pixel))
                {
                    // Test field
                    ReportField RF;
                    if (FieldsToMap.Count == 0)
                    {
                        RF = GetReportField("");
                        var TextSize = graphics.MeasureString(RF.ContentsName, font);
                        graphics.FillRectangle(new SolidBrush(bgColor), RF.Xposition, RF.Yposition, TextSize.Width, TextSize.Height);
                        graphics.DrawString(RF.ContentsName, font, new SolidBrush(codeColor), RF.Xposition, RF.Yposition);
                    }
                    else
                    {
                        foreach (ReportLayoutField RFF in FieldsToMap)
                        {
                            var TextSize = graphics.MeasureString(RFF.FieldName, font);
                            if (TextSize.Width < RFF.Length) TextSize.Width = RFF.Length;
                            graphics.FillRectangle(new SolidBrush(bgColor), RFF.Xposition, RFF.Yposition, TextSize.Width, TextSize.Height);
                            graphics.DrawString(RFF.FieldName, font, new SolidBrush(codeColor), RFF.Xposition, RFF.Yposition);
                        }
                    }
                }
                // Save image, image format type is consistent with response content type.
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return (ms.ToArray(), contentType);
            }

            return (new byte[0], "image/bmp");
        }






        private ReportField GetReportField(string FieldID)
        {
            ReportField RF;
            ReportLayoutField? REF;

            if (String.IsNullOrEmpty(FieldID))
                REF = _context.ReportLayoutFields.FirstOrDefault();
            else
                REF = _context.ReportLayoutFields.Where(rf => rf.FieldName == FieldID).FirstOrDefault();

            if (REF != null)
            {
                RF = new ReportField
                {
                    id = 0,
                    ContentsName = REF.FieldName,
                    Xposition = REF.Xposition,
                    Yposition = REF.Yposition,
                    Length = REF.Length
                };
            }
            else
            {
                RF = new ReportField
                {
                    id = 0,
                    ContentsName = "Test Employee",
                    Xposition = 400,
                    Yposition = 1600,
                    Length = 100
                };
            }
            return RF;
        }

    }
}
