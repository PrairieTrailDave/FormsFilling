namespace FormFilling.Models
{
    public class ReportImage
    {
        public int ID { get; set; }
        public string ReportCode { get; set; } = String.Empty;
        public string ImageFormat { get; set; } = String.Empty;
        public int Height { get; set; }
        public int Width { get; set; }
        public byte[] Image { get; set; } = Array.Empty<byte>();
    }
}
