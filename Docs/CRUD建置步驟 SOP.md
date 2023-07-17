# Entity Model
建立存取資料庫會用到的項目資料模型

# AdaperModel

在[AdapterModels] 依據 Entity Model 來建立需要用到的轉接器資料模型

# 資料模型自動轉換對應

* 開啟 [Helpers] > [AutoMapping.cs]
* 在 `Blazor AdapterModel` region 區段內，加入 Entity Model <-> Adapter Model 轉換對應

# Sorting Model

在 [SortModels] 資料夾內，建立此頁面會用到的可以排序的條件與排序欄位

# Repository Service

在 [Services] 目錄下，建立會用到的 Server 類別

# Extract Interface from Service

從服務類別抽取出服務介面，並且將介面放到適當資料夾( 在 [Services] > [Interfaces] 目錄下)

# Register Service type

開啟 [Helpers] > [CustomDependency.cs] 檔案，註冊剛剛建立的介面與類別服務

# Data Adapters

在 [Adapters] 建立需要用到的轉換器類別

# View Model

在 [ViewModels] 資料夾內，建立需要用到的檢視模型

# Razor Component

在 [Components] > [Views] 資料夾內，建立此檢視可以用到的 Razor 元件

# Pages

在 [Pages] 資料架下，建立該服務可以用到的頁面
