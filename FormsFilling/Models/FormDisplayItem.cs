namespace FormFilling.Models
{
    public class FormDisplayItem
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemContents { get; set; } = string.Empty;
        public int ItemType { get; set; } = 0;

        public const int ItemText = 0;
        public const int ItemImage = 1;
        public const int ItemBase64Image = 2;
    }
}
