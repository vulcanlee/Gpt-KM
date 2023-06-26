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
    private readonly ConvertFileModelService convertFileModelService;

    public ConvertToTextService(ConvertFileExtensionMatchService convertFileExtensionMatch,
        ConverterToTextFactory converterToTextFactory, BuildFilenameService buildFilenameService,
        ConvertFileModelService convertFileModelService)
    {
        this.convertFileExtensionMatch = convertFileExtensionMatch;
        this.converterToTextFactory = converterToTextFactory;
        this.buildFilenameService = buildFilenameService;
        this.convertFileModelService = convertFileModelService;
    }

    /// <summary>
    /// 將指定的檔案名稱，把該檔案的文字內容轉換成為文字檔案
    /// </summary>
    /// <param name="expertFile"></param>
    public async Task<ConvertFileModel> ConvertAsync(ExpertFile expertFile)
    {
        ConvertFileModel convertFiles = new();
        var extinsion = System.IO.Path.GetExtension(expertFile.FullName);
        var contentTypeEnum = ContentType.GetContentTypeEnum(extinsion);
        IFileToText fileToText = converterToTextFactory.Create(contentTypeEnum);
        Tokenizer tokenizer = new Tokenizer();

        #region 將檔案內容，轉換成為文字
        string sourceText = await fileToText.ToTextAsync(expertFile.FullName);
        ConvertFileModel convertFile = new ConvertFileModel()
        {
        };
        convertFile.FileName = buildFilenameService.BuildConvertedText(expertFile.FullName);
        convertFile.FileSize = expertFile.Size;
        convertFile.SourceText = sourceText;
        convertFile.SourceTextSize = sourceText.Length;
        convertFile.TokenSize = tokenizer.CountToken(sourceText);
        await convertFileModelService.ExportConvertTextAsync(expertFile, convertFile);
        convertFile.SplitContext(expertFile, buildFilenameService);
        #endregion

        return convertFiles;
    }
}
