using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System.Text;

namespace GptLibrary.Converts
{
    public class PptToText : IFileToText
    {
        public Task<string> ToTextAsync(string filePath)
        {
            var task = Task.Run(() =>
            {
                StringBuilder sb = new StringBuilder();

                using (PresentationDocument presentationDocument = PresentationDocument.Open(filePath, false))
                {
                    PresentationPart presentationPart = presentationDocument.PresentationPart;
                    if (presentationPart != null)
                    {
                        foreach (SlideId slideId in presentationPart.Presentation.SlideIdList)
                        {
                            SlidePart slidePart = (SlidePart)presentationPart.GetPartById(slideId.RelationshipId);

                            ExtractTextFromSlide(slidePart, sb);
                        }
                    }
                }

                return sb.ToString();
            });
            return task;
        }

        void ExtractTextFromSlide(SlidePart slidePart, StringBuilder sb)
        {
            if (slidePart == null)
            {
                return;
            }

            CommonSlideData slideData = slidePart.Slide.CommonSlideData;

            foreach (var shape in slideData.ShapeTree)
            {
                if (shape is Shape && ((Shape)shape).TextBody != null)
                {
                    foreach (var paragraph in ((Shape)shape).TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            sb.AppendLine(text.Text);
                        }
                    }
                }
            }
        }
    }
}