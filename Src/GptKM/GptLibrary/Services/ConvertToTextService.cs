using EntityModel.Entities;
using GptLibrary.Converts;
using GptLibrary.Gpt;
using GptLibrary.Models;

namespace GptLibrary.Services;

/// <summary>
/// 將檔案內容轉換成為文字檔案
/// </summary>
public class ConvertToTextService
{
    private readonly ConvertFileExtensionMatchService convertFileExtensionMatch;
    private readonly ConverterToTextFactory converterToTextFactory;
    private readonly BuildFilenameService buildFilenameService;

    public ConvertToTextService(ConvertFileExtensionMatchService convertFileExtensionMatch,
        ConverterToTextFactory converterToTextFactory, BuildFilenameService buildFilenameService)
    {
        this.convertFileExtensionMatch = convertFileExtensionMatch;
        this.converterToTextFactory = converterToTextFactory;
        this.buildFilenameService = buildFilenameService;
    }

    /// <summary>
    /// 將指定的檔案名稱，把該檔案的文字內容轉換成為文字檔案
    /// </summary>
    /// <param name="expertFile"></param>
    public void Convert(ExpertFile expertFile)
    {
        List<ConvertFileModel> convertFiles = new List<ConvertFileModel>();
        var extinsion = System.IO.Path.GetExtension(expertFile.FullName);
        var contentTypeEnum = ContentType.GetContentTypeEnum(extinsion);
        IFileToText fileToText = converterToTextFactory.Create(contentTypeEnum);
        Tokenizer tokenizer = new Tokenizer();

        #region 將檔案內容，轉換成為文字
        string sourceText = fileToText.ToText(expertFile.FullName);
        ConvertFileModel convertFile = new ConvertFileModel()
        {
        };
        convertFile.FileName = buildFilenameService.BuildConvertedText(expertFile.FullName);
        convertFile.FileSize = expertFile.Size;
        convertFile.SourceText = sourceText;
        convertFile.SourceTextSize = sourceText.Length;
        convertFile.TokenSize = tokenizer.CountToken(sourceText);
        convertFile.SplitContext(expertFile,buildFilenameService);
        convertFiles.Add(convertFile);
        #endregion
    }
}
