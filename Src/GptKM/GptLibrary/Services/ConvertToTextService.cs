using Domains.Models;
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
    private readonly GptExpertFileService gptExpertFileService;

    public ConvertToTextService(ConvertFileExtensionMatchService convertFileExtensionMatch,
        ConverterToTextFactory converterToTextFactory, BuildFilenameService buildFilenameService,
        ConvertFileModelService convertFileModelService,
        GptExpertFileService gptExpertFileService)
    {
        this.convertFileExtensionMatch = convertFileExtensionMatch;
        this.converterToTextFactory = converterToTextFactory;
        this.buildFilenameService = buildFilenameService;
        this.convertFileModelService = convertFileModelService;
        this.gptExpertFileService = gptExpertFileService;
    }

    /// <summary>
    /// 將指定的檔案名稱，把該檔案的文字內容轉換成為文字檔案
    /// </summary>
    /// <param name="expertFile"></param>
    public async Task<ConvertFileModel> ConvertAsync(ExpertFile expertFile)
    {
        var expertFileResult = await gptExpertFileService.GetAsync(expertFile.FullName);
        if (expertFileResult.Status == false)
        {
            return null;
        }
        expertFile = expertFileResult.Payload;

        var extinsion = System.IO.Path.GetExtension(expertFile.FullName);
        var contentTypeEnum = ContentType.GetContentTypeEnum(extinsion);

        IFileToText fileToText = converterToTextFactory.Create(contentTypeEnum);
        Tokenizer tokenizer = new Tokenizer();

        #region 將檔案內容，轉換成為文字
        string sourceText = await fileToText.ToTextAsync(expertFile.FullName);

        ConvertFileModel convertFile = new ConvertFileModel()
        {
            FileName = buildFilenameService.BuildConvertedText(expertFile.FullName),
            FileSize = expertFile.Size,
            SourceText = sourceText,
            SourceTextSize = sourceText.Length,
            TokenSize = tokenizer.CountToken(sourceText),
        };

        await convertFileModelService.ExportConvertTextAsync(expertFile, convertFile);

        convertFile.SplitContext(expertFile, buildFilenameService);

        #region 計算 TokenSize 與 EmbeddingCost 的成本
        expertFile.TokenSize = convertFile.TokenSize;
        expertFile.EmbeddingCost = convertFile.EmbeddingCost;
        expertFile.ProcessingStatus = CommonDomain.Enums.ExpertFileStatusEnum.ToText;
        expertFile.ChunkSize = convertFile.ConvertFileSplitItems.Count;
        await gptExpertFileService.UpdateAsync(expertFile);
        #endregion

        await convertFileModelService.DeleteExportConvertTextFileAsync(expertFile, convertFile);
        #endregion

        return convertFile;
    }
}
