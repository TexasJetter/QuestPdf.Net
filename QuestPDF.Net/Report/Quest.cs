using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Net.Models;
using System.Net;

namespace QuestPDF.Net.Report
{
    public static class Quest
    {
        public static readonly string colorAccent = Colors.Indigo.Medium;
        public static readonly TextStyle styleTitle = TextStyle.Default.Size(14).SemiBold().Color(colorAccent);
        public static readonly TextStyle styleHeader = TextStyle.Default.Size(11).SemiBold().Color(colorAccent);
        public static readonly TextStyle styleBody = TextStyle.Default.Size(11);

        //---------------------------------------------------------------------
        //  Simple Example
        //---------------------------------------------------------------------
        public static byte[] GetSimpleBytes()
        {
            var qPdf = new SimplePdf();
            using var stream = new MemoryStream();
            qPdf.GeneratePdf(stream);
            return stream.ToArray();
        }

        private class SimplePdf : IDocument
        {

            private readonly DocumentMetadata _meta = DocumentMetadata.Default;
            public DocumentMetadata GetMetadata() => _meta;

            public void Compose(IDocumentContainer container)
            {
                container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Height(100).Background(Colors.Grey.Lighten1);
                    page.Content().Background(Colors.Grey.Lighten3);
                    page.Footer().Height(50).Background(Colors.Grey.Lighten1);
                });
            }
        }

        //---------------------------------------------------------------------
        //  Example using Table structure layout
        //---------------------------------------------------------------------
        public static byte[] GetTableBytes()
        {
            var qPdf = new TablePdf();
            using var stream = new MemoryStream();
            qPdf.GeneratePdf(stream);
            return stream.ToArray();
        }

        private class TablePdf : IDocument
        {

            private readonly DocumentMetadata _meta = DocumentMetadata.Default;
            public DocumentMetadata GetMetadata() => _meta;

            public void Compose(IDocumentContainer container)
            {
                container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            }

            public void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("My Great Report", styleTitle);
                    });

                });
            }

            public void ComposeContent(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        //Column widths can be Relative (proportional)
                        //Or Constant (fixed unit, approx pixel - most pages have about 800px width)
                        columns.RelativeColumn();
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();
                        //You can optionally assign a relative width, but you can't mix
                        //assigned relative columns with constant columns
                        //This works
                        //columns.RelativeColumn(33);
                        //columns.RelativeColumn(33);
                        //columns.RelativeColumn(33);
                        //This will NOT
                        //columns.RelativeColumn(10);
                        //columns.ConstantColumn(50);
                        //columns.RelativeColumn();
                    });

                    uint row = 1;
                    table.Header(header =>
                    {

                        header.Cell().AlignCenter().BorderBottom(2).BorderColor(colorAccent).Text("Header 1", styleHeader);
                        header.Cell().AlignCenter().BorderBottom(2).BorderColor(colorAccent).Text("Header 2", styleHeader);
                        header.Cell().AlignCenter().BorderBottom(2).BorderColor(colorAccent).Text("Header 3", styleHeader);
                        //NOTE: If you don't assign a specific column it will just assign cells
                        //      to each column, wrapping to the next row. Enabeling the next line
                        //      will make create a second row with one cell in it
                        //header.Cell().AlignCenter().Text("Header 4", styleHeader);
                    });
                    row++;
                    uint col = 1;
                    for (var x = 1;x < 11; x++)
                    {
                        //You can optionally assign specific columns if you want to guard against an
                        //unexpected row creation or if you want to skip specific columns
                        col = 1;
                        table.Cell().Row(row).Column(col).AlignLeft().Text($"Row {x} Col {col}", styleBody);
                        col++;
                        table.Cell().Row(row).Column(col).AlignCenter().Text($"Row {x} Col {col}", styleBody);
                        col++;
                        table.Cell().Row(row).Column(col).AlignRight().Text($"Row {x} Col {col}", styleBody);
                        row++;
                    }
                });
            }

            public void ComposeFooter(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Row(1).Column(1).AlignLeft().Text(DateTime.Now);
                    table.Cell().Row(1).Column(2).AlignCenter().Text("");
                    table.Cell().Row(1).Column(3).AlignRight().Text(x =>
                    {
                        //There are build in methods to get page info
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }
        }


        //---------------------------------------------------------------------
        //  Example using a custom component and image
        //---------------------------------------------------------------------
        public static byte[] GetComponentBytes(List<UserInformation> users)
        {
            var qPdf = new ComponentPdf(users);
            using var stream = new MemoryStream();
            qPdf.GeneratePdf(stream);
            return stream.ToArray();
        }

        private class ComponentPdf : IDocument
        {
            private readonly List<UserInformation> _users = new List<UserInformation>();
            private readonly DocumentMetadata _meta = DocumentMetadata.Default;
            public DocumentMetadata GetMetadata() => _meta;
            public ComponentPdf(List<UserInformation> users)
            {
                _users = users;
            }
            public void Compose(IDocumentContainer container)
            {
                container
                .Page(page =>
                {
                    page.Margin(50);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            }

            public void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Report based on components", styleTitle);
                    });

                });
            }

            public void ComposeContent(IContainer container)
            {
                container.Column(column =>
                {
                    var cnt = _users.Count;
                    foreach (var user in _users)
                    {
                        column.Item().Row(row =>
                        {
                            //You can create a reusable component that encapsulates
                            //a complex layout or a layout that is shared between
                            //reports
                            row.RelativeItem().Component(new CustomComponent(user));
                        });
                        //Force a page break between each user, except the last one
                        //(that would generate a trailing blank page
                        cnt--;
                        if (cnt > 0) column.Item().PageBreak();
                    }
                });
            }

            public void ComposeFooter(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Row(1).Column(1).AlignLeft().Text(DateTime.Now);
                    table.Cell().Row(1).Column(2).AlignCenter().Text("");
                    table.Cell().Row(1).Column(3).AlignRight().Text(x =>
                    {
                        //There are build in methods to get page info
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }
        }

        public class CustomComponent : IComponent
        {
            private UserInformation _user { get; }

            public CustomComponent(UserInformation package)
            {
                _user = package;
            }

            public void Compose(IContainer container)
            {
                container.Column(column =>
                {
                    column.Spacing(2);
                    column.Item().Width(100).Image(_user.Image);
                    column.Item().Text(_user.Name, styleHeader);
                    column.Item().ExtendHorizontal().BorderBottom(1).BorderColor(colorAccent);
                    foreach (var email in _user.Emails)
                    {
                        column.Item().Text(email);
                    }
                });
            }
        }
    }
}
