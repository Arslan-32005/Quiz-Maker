using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using System.Text.RegularExpressions;
using System.Text;
namespace Quiz_Maker.Services
{
    public class DocumentTextExtractor : IDocumentTextExtractor
    {
        private const int MaxFileSize = 5 * 1024 * 1024;
        private const int MinCharacters = 200;
        private const int MaxCharacters = 30000;
        private static DocumentTextResult FinalizeResult(DocumentTextResult result)
        {
            if (!result.Success)
            {
                return result;
            }

            var text = result.Text ?? string.Empty;

            text = Regex.Replace(text, @"[\u0000-\u0008\u000B\u000C\u000E-\u001F]", string.Empty);
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"(\r?\n){3,}", "\n\n");
            text = text.Trim();

            if (text.Length == 0)
            {
                return Fail("No readable text found. Scanned or image-based documents are not supported.");
            }

            if (text.Length < MinCharacters)
            {
                return Fail("The extracted text is too short. Please provide a document with at least 200 characters.");
            }

            var truncated = false;
            if (text.Length > MaxCharacters)
            {
                text = text.Substring(0, MaxCharacters);
                truncated = true;
            }

            return new DocumentTextResult
            {
                Success = true,
                Text = text,
                Error = null,
                WasTruncated = truncated
            };
        }
        private static DocumentTextResult ExtractTextFromPdf(Stream stream)
        {
            try
            {
                using var pdf = PdfDocument.Open(stream);
                var sb = new StringBuilder();
                foreach(var page in pdf.GetPages())
                {
                    var text = ContentOrderTextExtractor.GetText(page);
                    var line = text.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        sb.AppendLine(line);
                    }
                    if(sb.Length > MaxCharacters)
                    {
                        break;
                    }
                }
                return new DocumentTextResult
                {
                    Success = true,
                    Text = sb.ToString(),
                    Error = null
                };
            }
            catch
            {
                return Fail("could not extract text from .pdf file.");
            }
        }
        private static DocumentTextResult ExtractTextFromDocx(Stream stream)
        {
            try
            {
                using var doc = WordprocessingDocument.Open(stream, false);
                var body = doc.MainDocumentPart?.Document?.Body;
                if (body == null)
                {
                    return Fail("The .docx file is empty or corrupted.");
                }


                var sb = new System.Text.StringBuilder();
                foreach (var p in body.Descendants<Paragraph>())
                {
                    var line = p.InnerText.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        sb.AppendLine(line);
                    }
                }
                return new DocumentTextResult
                {
                    Success = true,
                    Text = sb.ToString(),
                    Error = null
                };
            }
            catch (Exception )
            {
                return Fail("could not extract text from .docx file.");
            }
        }



        private static DocumentTextResult Fail(string errorMessage)
        {
            return new DocumentTextResult
            {
                Success = false,
                Text = string.Empty,
                Error = errorMessage
            };
        }

        public async Task<DocumentTextResult> ExtractAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Fail("No file uploaded.");
            }

            if (file.Length > MaxFileSize)
            {
                return Fail("File size exceeds the maximum limit of 5 MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".docx" && extension != ".pdf")
            {
                return Fail("Invalid file type. Only .docx and .pdf files are allowed.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var header = new byte[4];
            var bytesRead = await stream.ReadAsync(header, 0, 4);
            stream.Position = 0;

            if (bytesRead < 4)
            {
                return Fail("File is too small or corrupted.");
            }

            bool isPdf = header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46;
            bool isZip = header[0] == 0x50 && header[1] == 0x4B;

            if ((extension == ".pdf" && !isPdf) || (extension == ".docx" && !isZip))
            {
                return Fail("File content does not match its extension.");
            }
            var result = extension switch
            {
                ".docx" => ExtractTextFromDocx(stream),
                ".pdf" => ExtractTextFromPdf(stream),
                _ => Fail("Unsupported file type.")
            };

            return FinalizeResult(result);
        }
    }
}