using System;
using System.Text;

namespace DocumentGenerator.Core
{
    public class PdfConverter
    {
        public Document Convert(Document source)
        {
            if (source.ContentType != "text/html")
                throw new InvalidOperationException(
                    $"Can't convert document of type {source.ContentType} to pdf. Only html source is supported");

            var htmlText = Encoding.Default.GetString(source.Content);
            var htmlToPdf = new HtmlToPDFCore.HtmlToPDF();
            var pdf = htmlToPdf.ReturnPDF(htmlText);

            return new Document("application/pdf", pdf);
        }
    }
}