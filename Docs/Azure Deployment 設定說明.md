# Azure Deployment 設定說明

## 建立 C:\Home 資料夾
mkdir Source
mkdir Convert

## 建立 C:\temp 資料夾

## Azure OpenAI Key
OpenAIConfiguration:AzureOpenAIKey
OpenAIConfiguration:AzureOpenAIEndpoint     https://gpt35tw.openai.azure.com/
OpenAIConfiguration:ChatPromptCompletionModelName     gpt-35-turbo-16k

## Syncfusion License Key
BackendSyncfusion:License

## Enable Web Service Keep alive
BackendSystemAssistant:KeepAliveEndpoint = https://exentricKM.azurewebsites.net/KeepAlive
BackendSystemAssistant:EnableKeepAliveEndpoint = true

## Force to use SQLite
BackendSystemAssistant:UseSQLite = true

## Initialization Seed Number
BackendInitializer:SeedNumber : 987789



