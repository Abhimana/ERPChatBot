using AI.Data;
using AI.Repository;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using IronXL;
using System.Text;

namespace AI.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task SaveFileAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var uploadedFile = new UploadedFile
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                FileType = System.IO.Path.GetExtension(file.FileName),
                FileData = memoryStream.ToArray()
            };

            await _fileRepository.SaveFileAsync(uploadedFile);
        }

        public async Task<List<UploadedFile>> GetFilesAsync()
        {
            return await _fileRepository.GetFilesAsync();
        }

        public async Task<string> ExtractTextFromFilesAsync()
        {
            var files = await _fileRepository.GetFilesAsync();
            StringBuilder extractedText = new();

            foreach (var file in files)
            {
                using var memoryStream = new MemoryStream(file.FileData);

                if (file.FileName.Contains("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    extractedText.AppendLine(ExtractTextFromPdf(memoryStream));
                }
                else if (file.FileName.Contains("docx", StringComparison.OrdinalIgnoreCase))
                {
                    extractedText.AppendLine(ExtractTextFromDocx(memoryStream));
                }
                else if (file.FileName.Contains("xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    extractedText.AppendLine(ExtractTextFromXlsx(memoryStream));
                }
                else if (file.FileName.Contains("txt", StringComparison.OrdinalIgnoreCase))
                {
                    extractedText.AppendLine(await ExtractTextFromTxt(memoryStream));
                }
            }
            return extractedText.ToString();
        }


        // Extracts text from PDF
        private string ExtractTextFromPdf(Stream pdfStream)
        {
            StringBuilder text = new();
            var pdfReader = new iText.Kernel.Pdf.PdfReader(pdfStream);
            var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader);
            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text.AppendLine(iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy));
            }

            return text.ToString();
        }


        // Extracts text from DOCX (Word)
        private string ExtractTextFromDocx(Stream docxStream)
        {
            StringBuilder text = new();
            using var doc = WordprocessingDocument.Open(docxStream, false);
            var body = doc.MainDocumentPart?.Document.Body;

            if (body != null)
            {
                foreach (var para in body.Elements<Paragraph>())
                {
                    text.AppendLine(para.InnerText);
                }
            }

            return text.ToString();
        }


        // Extracts text from XLSX (Excel)
        private string ExtractTextFromXlsx(Stream xlsxStream)
        {
            StringBuilder text = new();
            WorkBook workbook = WorkBook.Load(xlsxStream); // Load the workbook

            foreach (var sheet in workbook.WorkSheets)
            {
                foreach (var row in sheet.Rows)
                {
                    List<string> rowData = new();

                    foreach (var cell in row)
                    {
                        rowData.Add(cell?.StringValue ?? "");
                    }

                    text.AppendLine(string.Join("\t", rowData));
                }
                text.AppendLine("--------");
            }

            return text.ToString();
        }




        // Extracts text from TXT file
        private async Task<string> ExtractTextFromTxt(Stream txtStream)
        {
            using StreamReader reader = new(txtStream);
            return await reader.ReadToEndAsync();
        }


    }

}
