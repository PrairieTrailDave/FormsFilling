namespace FormFilling.Models
{
    public class ReportFont
    {
        public int ID { get; set; }
        public string ReportCode { get; set; } = string.Empty;
        public string FontName { get; set; } = string.Empty;
        public int FontSize { get; set; }
        public int FontStyle { get; set; }


        public const int BoldStyle = 1;
        public const int ItalicStyle = 2;
    }
}
