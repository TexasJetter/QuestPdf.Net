using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Net.Report;

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
            return File(Quest.GetComponentBytes(), "application/pdf", "Table.pdf");
        }
    }
}