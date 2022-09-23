using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class W4_2022Controller : ControllerBase
    {
        // injected variables

        private readonly DatabaseContext _context;

        // internal variables

        public readonly string ActionName = "Get";
        public readonly string WhatPartOfSystem = "W4 2022 API";

        public W4_2022Controller(
            DatabaseContext context

            )
        {
            _context = context;

        }

        //[HttpGet]
        [HttpGet("[Action]")]
        [AllowAnonymous]
        public async Task GetW4Async([FromQuery] string APIKey, [FromQuery] int? EmployeeNumber)
        {
            // this returns a bitmap of the report page

            try
            {
                if (APIKey == Constants.UrlAPIKey)
                {
                    if (EmployeeNumber != null)
                    {
                        var TEmployee = await _context.Employees.FindAsync(EmployeeNumber);
                        if (TEmployee != null)
                        {
                            var EmployeeW4 = await _context.W4DataStorage.Where(w4 => w4.EmployeeRecordID == EmployeeNumber).OrderBy(w4 => w4.ID).LastOrDefaultAsync();
                            if (EmployeeW4 != null)
                            {
                                string ReportCode = "W4";
                                var EmployerW4 = await _context.Employers.FirstOrDefaultAsync();
                                if (EmployerW4 != null)
                                {
                                    // convert the data into the view format

                                    W4ReportView W4View = W4ReportView.ConvertToW4ReportView(EmployeeW4);
                                    W4View.SetEmployerFields(EmployerW4);
                                    W4View.SetEmployeeFields(TEmployee);
                                    SigData? EmployeeSignature = await _context.Signatures.Where(sg => sg.EmployeeID == EmployeeNumber).FirstOrDefaultAsync();
                                    if (EmployeeSignature != null)
                                        W4View.SetSignature(EmployeeSignature.Signature);

                                    var (content, contentType) = await FormatAndGeneratePage(ReportCode, W4View);
                                    HttpContext.Response.ContentType = contentType;
                                    await HttpContext.Response.BodyWriter.WriteAsync(content);
                                }
                            }
                            else
                            {
                                var (content, ncontentType) = FormDisplayService.GenerateMessageImage("W4 Not Found");
                                HttpContext.Response.ContentType = ncontentType;
                                await HttpContext.Response.BodyWriter.WriteAsync(content);
                            }
                        }
                        else
                        {
                            var (content, ncontentType) = FormDisplayService.GenerateMessageImage("Employee Not Found");
                            HttpContext.Response.ContentType = ncontentType;
                            await HttpContext.Response.BodyWriter.WriteAsync(content);
                        }
                    }
                    else
                    {
                        var (content, ncontentType) = FormDisplayService.GenerateMessageImage("Employee Not Found");
                        HttpContext.Response.ContentType = ncontentType;
                        await HttpContext.Response.BodyWriter.WriteAsync(content);
                    }
                }
                else
                {
                    var (content, ncontentType) = FormDisplayService.GenerateMessageImage("Function Not Found");
                    HttpContext.Response.ContentType = ncontentType;
                    await HttpContext.Response.BodyWriter.WriteAsync(content);
                }
            }
            catch (Exception Ex)
            {
                var (content, contentType) = FormDisplayService.GenerateMessageImage(Ex.Message);
                HttpContext.Response.ContentType = contentType;
                await HttpContext.Response.BodyWriter.WriteAsync(content);
            }
        }

        private async Task<(byte[] content, string contentType)> FormatAndGeneratePage(string ReportCode, W4ReportView W4View)
        {

            // get the data into the report format 

            var W4FieldMapping = await _context.ReportLayoutFields.Where(rlf => rlf.ReportCode == ReportCode).ToListAsync();

            ReportImage? tReportImage = _context.ReportImages.Where(di => di.ReportCode == ReportCode).FirstOrDefault();
            List<FormDisplayItem> DataToPrint = new List<FormDisplayItem>();

            if (tReportImage != null)
            {
                foreach (ReportLayoutField RF in W4FieldMapping)
                {
                    var w4field = W4View.GetType().GetProperty(RF.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    if (w4field != null)
                    {
                        var mthod = w4field.GetGetMethod();
                        if (mthod != null)
                        {
                            string? fieldvalue = "";
                            fieldvalue = (string?)mthod.Invoke(W4View, null);
                            if (fieldvalue != null)
                            {
                                int itemtype = FormDisplayItem.ItemText;
                                if (RF.FieldType == ReportLayoutField.ImageType)
                                    itemtype = FormDisplayItem.ItemImage;
                                if (RF.FieldType == ReportLayoutField.Base64ImageType)
                                    itemtype = FormDisplayItem.ItemBase64Image;

                                DataToPrint.Add(new FormDisplayItem()
                                {
                                    ItemName = RF.FieldName,
                                    ItemContents = fieldvalue,
                                    ItemType = itemtype
                                });
                            }
                        }
                    }
                }

                // get the font to use for this page

                ReportFont? FontToUse = await _context.ReportFonts.Where(fnt => fnt.ReportCode == ReportCode).FirstOrDefaultAsync();
                if (FontToUse == null)
                {
                    FontToUse = new ReportFont
                    {
                        FontName = "Courier New",
                        FontSize = 80,
                        FontStyle = ReportFont.BoldStyle
                    };
                }

                return FormDisplayService.GeneratePage(tReportImage, FontToUse, W4FieldMapping, DataToPrint);
            }

            return (new byte[0], "image/bmp");
        }



    }
}
