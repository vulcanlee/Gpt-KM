# Azure Deployment 設定說明

## 建立 C:\Home 資料夾
mkdir Source
mkdir Convert

## Azure OpenAI Key
OpenAIConfiguration:AzureOpenAIKey

## Syncfusion License Key
BackendSyncfusion:License

## Enable Web Service Keep alive
BackendSystemAssistant:KeepAliveEndpoint = https://exentricKM.azurewebsites.net/KeepAlive
BackendSystemAssistant:EnableKeepAliveEndpoint = true

## Force to use SQLite
BackendSystemAssistant:UseSQLite = true



