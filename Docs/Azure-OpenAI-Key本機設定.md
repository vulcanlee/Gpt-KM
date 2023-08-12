# Azure-OpenAI-Key本機設定

## 取得 Azure OpenAI Key 並且儲存為系統環境變數

同樣的，這裡還是使用 Azure 上提供的 OpenAI 服務來進行 API 呼叫

* 打開 Azure 網頁，並且登入該服務
* 切換到你自己建立 [Azure OpenAI] 服務
* 在 Overview 儀表板頁面中，將會看到 [Manage keys] 欄位
* 點選該欄位名稱右邊的 [Click here to manage keys] 文字
* 現在將會看到 [Keys and Endpoint] 這個頁面
* 你可以點選 [Show Keys] 來看到 API Key 的內容，又或者點選最右方的複製按鈕，將 API Key 複製到剪貼簿內
* 開啟命令提示字元視窗
* 使用底下命令將建立 OpenAI Key 永久性的環境變數

```
setx OpenAIKey "剪貼簿內的 OpenAI Key 值" /M
```

## 進行刪除

使用 /M 選項設定的環境變數可以使用以下命令刪除：

delete an environment variable from Command Prompt, type one of these two commands, depending on what type that variable is:

REG delete "HKCU\Environment" /F /V "variable_name" if it’s a user environment variable

REG delete "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /F /V "variable_name" if it’s a system environment variable.

刪除 OpenAI Key 的環境變數

```
REG delete "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /F /V OpenAIKey
```

