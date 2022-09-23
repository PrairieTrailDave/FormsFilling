namespace FormFilling.Models
{
    public class ReportField
    {
        public int id { get; set; }
        public string FieldName { get; set; } = String.Empty;
        public string ContentsName { get; set; } = String.Empty;
        public int Xposition { get; set; }
        public int Yposition { get; set; }
        public int Length { get; set; }
    }
}
