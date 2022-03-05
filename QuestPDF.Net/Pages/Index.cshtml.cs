using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Net.Models;
using QuestPDF.Net.Report;
using System.Net;

namespace QuestPDF.Net.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public FileResult OnGetDownloadSimplePDF()
        {
            return File(Quest.GetSimpleBytes(), "application/pdf", "Simple.pdf");
        }
        public FileResult OnGetDownloadTablePDF()
        {
            return File(Quest.GetTableBytes(), "application/pdf", "Table.pdf");
        }
        public FileResult OnGetDownloadComponentPDF()
        {
            byte[] imageBytes;
            using (var wc = new WebClient())
            {
                imageBytes = wc.DownloadData("https://via.placeholder.com/100");
            }
            var users = new List<UserInformation> 
            {
                new UserInformation{ Name="Jack", Image=imageBytes, Emails= new List<string>{"jack@outlook.com", "jack@gmail.com"}},
                new UserInformation{ Name="Ted", Image=imageBytes, Emails= new List<string>{"ted@outlook.com", "ted@gmail.com"}},
            };
            return File(Quest.GetComponentBytes(users), "application/pdf", "Table.pdf");
        }
    }
}