# Azure Deployment 設定說明

## 建立 C:\Home 資料夾
mkdir Source
mkdir Convert

## 建立 C:\temp 資料夾

## Azure OpenAI Key
OpenAIConfiguration:AIEngine                Twcs   AzureOpenAI
OpenAIConfiguration:AzureOpenAIKey
OpenAIConfiguration:AzureOpenAIEndpoint     https://gpt35tw.openai.azure.com/
OpenAIConfiguration:ChatPromptCompletionModelName     gpt-35-turbo-16k
OpenAIConfiguration:Ada002MaxRequestTokens     2500   8191
OpenAIConfiguration:IncrementStringAmount      100    500
OpenAIConfiguration:TwcsAPI_KEY              9114aa97-8ede-4f8b-8c65-d713223fe090
OpenAIConfiguration:TwcsGPTEndpoint          https://ffm-trial05.twcc.ai/text-generation/api/models/generate
OpenAIConfiguration:TwcsEmbeddingEndpoint    https://ffm-trial05.twcc.ai/embeddings/api/embeddings
OpenAIConfiguration:AzureOpenAIKey

## Syncfusion License Key
BackendSyncfusion:License

## Enable Web Service Keep alive
BackendSystemAssistant:KeepAliveEndpoint = https://exentricKM.azurewebsites.net/KeepAlive
BackendSystemAssistant:EnableKeepAliveEndpoint = true

## Force to use SQLite
BackendSystemAssistant:UseSQLite = true

## Initialization Seed Number
BackendInitializer:SeedNumber : 987789

## 授權資訊
ProductLicense:CompanyName : 耀瑄科技
ProductLicense:ProductName : 智慧知識庫


## 使用 達文西 模型所用的參數

```json
  "OpenAIConfiguration": {
    "AzureOpenAIKey": "",
    "TextEmbeddingAdaModelName": "text-embedding-ada-002",
    "AzureOpenAIEndpoint": "https://openailabtw.openai.azure.com/",
    "ChatPromptCompletionModelName": "text-davinci-003",
    "ChatPromptCompletionTemperature": 0.3,
    "DefaultExpertDirectoryName": "本機測試用",
    "DefaultSourcePath": "C:\\Home\\Source",
    "DefaultConvertPath": "C:\\Home\\Convert",
    "EmbeddingModelTextEmbeddingAda002MaxRequestTokens": 8191,
    "EmbeddingModelTextEmbeddingAda002ResponseTokens": 500,
    "EmbeddingModelTextEmbeddingAda002RealRequestTokens": 2500,
    "EmbeddingModelTextEmbeddingAda002RealRequestPatchTokens": 1000,
    "IncrementStringAmount": 800
  }
```

## 使用 GPT35 16K 模型所用的參數

```json
  "OpenAIConfiguration": {
    "AzureOpenAIKey": "",
    "TextEmbeddingAdaModelName": "text-embedding-ada-002",
    "AzureOpenAIEndpoint": "https://openailabtw.openai.azure.com/",
    "ChatPromptCompletionModelName": "text-davinci-003",
    "ChatPromptCompletionTemperature": 0.3,
    "DefaultExpertDirectoryName": "本機測試用",
    "DefaultSourcePath": "C:\\Home\\Source",
    "DefaultConvertPath": "C:\\Home\\Convert",
    "EmbeddingModelTextEmbeddingAda002MaxRequestTokens": 8191,
    "EmbeddingModelTextEmbeddingAda002ResponseTokens": 500,
    "EmbeddingModelTextEmbeddingAda002RealRequestTokens": 2500,
    "EmbeddingModelTextEmbeddingAda002RealRequestPatchTokens": 1000,
    "IncrementStringAmount": 800
  }
```

