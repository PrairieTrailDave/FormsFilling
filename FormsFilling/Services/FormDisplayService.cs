using System;
using System.Collections.Generic;
using System.Drawing;
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



namespace FormFilling.Services
{
    public class FormDisplayService
    {

        public static (byte[] content, string contentType) FormatAndGeneratePage(Object FormDataView, List<ReportLayoutField> FieldMapping, ReportImage? tReportImage, ReportFont? FontToUse)
        {

            // get the data into the report format 

            List<FormDisplayItem> DataToPrint = new();

            if (tReportImage != null)
            {
                foreach (ReportLayoutField RF in FieldMapping)
                {
                    var formfield = FormDataView.GetType().GetProperty(RF.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    if (formfield != null)
                    {
                        var mthod = formfield.GetGetMethod();
                        if (mthod != null)
                        {
                            string? fieldvalue = (string?)mthod.Invoke(FormDataView, null);
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

                FontToUse ??= new ReportFont
                    {
                        FontName = "Courier New",
                        FontSize = 80,
                        FontStyle = ReportFont.BoldStyle
                    };

                return FormDisplayService.GeneratePage(tReportImage, FontToUse, FieldMapping, DataToPrint);
            }

            return (Array.Empty<byte>(), "image/bmp");
        }


        public static (byte[] content, string contentType) GeneratePage(ReportImage tReportImage, ReportFont FontToUse, List<ReportLayoutField> FieldList, List<FormDisplayItem> DataToPrint)
        {
            // Setup output format
            var contentType = "image/bmp";
            // print in black
            Color codeColor = Color.FromArgb(0x0, 0x0, 0x0);

            // define the stream to return the image

            using var ms = new MemoryStream();

            if (tReportImage != null)
            {
                // get the report font 

                FontFamily SelectedFont;
                FontStyle ReportStyle;
                float FontSize;


                if (FontToUse != null)
                {
                    bool isBold = (FontToUse.FontStyle & ReportFont.BoldStyle) == 1;
                    bool isItalic = (FontToUse.FontStyle & ReportFont.ItalicStyle) == 1;
                    SelectedFont = new FontFamily(FontToUse.FontName);
                    ReportStyle = FontStyle.Regular;
                    if (isBold)
                        ReportStyle = FontStyle.Bold;
                    if (isItalic)
                        ReportStyle = FontStyle.Italic;
                    FontSize = (float)Convert.ToDecimal(FontToUse.FontSize);





                    byte[] ImageFromDisk = tReportImage.Image;
                    MemoryStream FD = new(ImageFromDisk);
                    Image img = Image.FromStream(FD);

                    MemoryStream bmpms = new();
                    img.Save(bmpms, System.Drawing.Imaging.ImageFormat.Bmp);
                    Bitmap bmp = new(new Bitmap(bmpms));
                    using Graphics graphics = Graphics.FromImage(bmp);
                    using (Font font = new(SelectedFont, FontSize, ReportStyle, GraphicsUnit.Pixel))
                    {
                        // cross correlate the fields to print with the data to print
                        foreach (ReportLayoutField RF in FieldList)
                        {
                            foreach (FormDisplayItem fdi in DataToPrint)
                            {
                                if (fdi.ItemName == RF.FieldName)
                                {
                                    string? fieldvalue = fdi.ItemContents;
                                    if (fieldvalue != null)
                                    {
                                        if (fdi.ItemType == FormDisplayItem.ItemText)
                                        {
                                            var TextSize = graphics.MeasureString(fieldvalue, font);
                                            //graphics.FillRectangle(new SolidBrush(bgColor), RF.Xposition, RF.Yposition, TextSize.Width, TextSize.Height);
                                            graphics.DrawString(fieldvalue, font, new SolidBrush(codeColor), RF.Xposition, RF.Yposition);
                                        }
                                        if (fdi.ItemType == FormDisplayItem.ItemBase64Image)
                                        {
                                            if (fieldvalue != "")
                                            {
                                                if (fieldvalue.Contains(','))
                                                {
                                                    string justTheImage = fieldvalue.Split(",")[1];
                                                    byte[] imageBytes = Convert.FromBase64String(justTheImage);
                                                    MemoryStream ims = new(imageBytes);
                                                    Image fldImage = Image.FromStream(ims, true);
                                                    MemoryStream bmms = new();
                                                    fldImage.Save(bmms, System.Drawing.Imaging.ImageFormat.Bmp);
                                                    Bitmap fldBitmap = new(fldImage);
                                                    int imgHeight = fldImage.Height;
                                                    int imgWidth = fldImage.Width;
                                                    Rectangle Rect = new(RF.Xposition, RF.Yposition, RF.Length, RF.Height);
                                                    graphics.DrawImage(fldBitmap, Rect);
                                                }
                                            }
                                        }
                                        if (fdi.ItemType == FormDisplayItem.ItemImage)
                                        {
                                            // This doesn't work as the sig file has the datatype leading the way
                                            byte[] imageBytes = Encoding.Unicode.GetBytes(fieldvalue);
                                            MemoryStream ims = new(imageBytes);
                                            Image fldImage = Image.FromStream(ims, true, false);
                                            MemoryStream bmms = new();
                                            fldImage.Save(bmms, System.Drawing.Imaging.ImageFormat.Bmp);
                                            Bitmap fldBitmap = new(fldImage);    
                                            graphics.DrawImage(fldBitmap, RF.Xposition, RF.Yposition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Save image, image format type is consistent with response content type.
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return (ms.ToArray(), contentType);
                }

            }

            return (Array.Empty<byte>(), "image/bmp");
        }








        public static (byte[] content, string contentType) GenerateMessageImage(string Message)
        {
            // Setup output format
            var contentType = "image/png";
            // print in black
            Color codeColor = Color.FromArgb(0x0, 0x0, 0x0);
            // Background color 
            Color bgColor = Color.FromArgb(0xe9, 0xec, 0xef);
            // Image height
            const int imageHeight = 50;
            const int imageWidth = 700;
            using var ms = new MemoryStream();

            using (Font font = new(FontFamily.GenericMonospace, 32, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel))
            {
                // Create the image
                using Bitmap bitmap = new(imageWidth, imageHeight);
                // Create the graphics 
                using Graphics graphics = Graphics.FromImage(bitmap);
                // Write bg color
                graphics.FillRectangle(new SolidBrush(bgColor), 0, 0, imageWidth, imageHeight);


                var TextSize = graphics.MeasureString(Message, font);

                // Write char to the graphic 
                graphics.DrawString(Message, font, new SolidBrush(codeColor), 10, 10);

                // Save image, image format type is consistent with response content type.
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            }
            return (ms.ToArray(), contentType);
        }

    }
}
