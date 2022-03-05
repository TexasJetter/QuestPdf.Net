namespace QuestPDF.Net.Models
{
    public class UserInformation
    {
        public string Name { get; set; } = string.Empty;
        public byte[] Image { get; set; } = null;
        public List<string> Emails { get; set; } = new List<string>();
    }
}
