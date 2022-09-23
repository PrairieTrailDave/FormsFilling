namespace FormFilling.Models
{
    public class ReportLayoutField
    {
        public int id { get; set; }
        public string ReportCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = String.Empty;
        public int Xposition { get; set; }
        public int Yposition { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public string FieldType { get; set; } = String.Empty;

        public const string TextType = "text";
        public const string ImageType = "imag";
        public const string Base64ImageType = "bas6";

    }
}
