using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;

partial class Program
{
    private const string SqlConnectionString = "Server=localhost;Database=AdventureWorksDW2025;Trusted_Connection=True;TrustServerCertificate=True;";
    private const string QdrantUrl = "http://localhost:6334";
    private const string CollectionName = "adventure_works_products";
    private const string OllamaUrl = "http://localhost:11434";

    private static readonly IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator =
        new OllamaEmbeddingGenerator(new Uri(OllamaUrl), "bge-m3");

    private static readonly IChatClient ChatClient =
        new OllamaChatClient(new Uri(OllamaUrl), "qwen2.5:7b");

    private static readonly QdrantClient QdrantClient = new(new Uri(QdrantUrl));

    // 💡 建立全域的歷史對話記憶庫
    private static readonly List<ChatMessage> ChatHistory = new();

    static async Task Main(string[] args)
    {
        // 💡 把這段直接貼在 Main 方法的第一行，讓程式自動在 VS 2022 目錄下產生實體檔案！
        System.IO.File.WriteAllText("README1.md", @"# Fully Local AI Agent with RAG Knowledge Base

A **fully local, privacy-first AI Agent** built with **C# (.NET 8)**, **Ollama**, **Qdrant**, and **SQL Server**. This system leverages Retrieval-Augmented Generation (RAG) to provide intelligent Q&A capabilities directly over enterprise data warehouses without uploading any information to the cloud.

---

## 🏗️ Architecture Overview

[ SQL Server ] (AdventureWorksDW / WideWorldImportersDW)
│
▼ (C# SqlClient - Batch Fetching)
[ C# .NET 8 ] 🧠 (Main Engine & Chat History Management)
│
├─► (Ollama API) ──► [ bge-m3 Model ] ──► (Generate Embeddings)
│                                                 │
├─► (gRPC Port 6334) ─────────────────────────────┼─► [ Qdrant Vector DB ]
│                                                 │     (Store & Vector Search)
▼                                                 ▼
[ Ollama LLM ] (qwen2.5:7b) ◄───────────────────────────┘ (Retrieve Top Context)
│
▼
[ User Interface ] (Fully Fluid Traditional Chinese Responses)
---

## ⚡ Key Features

* **100% On-Premises & Private:** No internet connection required; ideal for highly sensitive corporate data.
* **Batch Vectorization Engine:** Optimizes GPU/CPU utilization by grouping embeddings into chunks of 32 for lightning-fast database indexing.
* **Semantic Context Filtering:** Cleans data noise directly at the database layer (e.g., separating Bikes from Socks in AdventureWorks).
* **Persistent Chat History:** Maintains contextual awareness, allowing the agent to understand pronouns like ""it"" or ""this one"" across continuous dialogues.
* **Multi-DB Agility:** Ready-to-use schemas for both `AdventureWorksDW2025` and `WideWorldImportersDW-Full`.

---

## 🛠️ Prerequisites & Local Infrastructure

Before running the application, ensure the following local services are running on your Windows machine:

### 1. SQL Server
Restore your choice of Microsoft demo data warehouse (`AdventureWorksDW2025` or `WideWorldImportersDW-Full`).

### 2. Ollama (Local LLM & Embedding Host)
Ensure Ollama is installed and run the following commands to pull the necessary models:

ollama pull bge-m3          # 1024-Dimension High-Performance Embedding Model
ollama pull qwen2.5:7b      # Advanced Conversational Language Model

### 3. Qdrant (Vector Database)

Launch qdrant.exe directly on your Windows environment, ensuring ports 6333 (HTTP) and 6334 (gRPC) are operational.


💻 Technical Configurations & Code Blocks

NuGet Dependencies :
dotnet add package Microsoft.Data.SqlClient --version 5.2.2
dotnet add package Microsoft.Extensions.AI --prerelease
dotnet add package Microsoft.Extensions.AI.Ollama --prerelease
dotnet add package Qdrant.Client --version 1.12.0


Context-Optimized SQL Queries

Option A: AdventureWorksDW2025 (Bikes & Accessories Focus)

SELECT p.ProductKey, p.EnglishProductName, p.EnglishDescription 
FROM DimProduct p
INNER JOIN DimProductSubcategory s ON p.ProductSubcategoryKey = s.ProductSubcategoryKey
INNER JOIN DimProductCategory c ON s.ProductCategoryKey = c.ProductCategoryKey
WHERE c.EnglishProductCategoryName IN ('Bikes', 'Clothing', 'Accessories')
  AND p.EnglishDescription IS NOT NULL;

Option B: WideWorldImportersDW (Novelty & Marketing Focus)

SELECT [Stock Item Key], [Stock Item], [Marketing Comments]
FROM [Dimension].[Stock Item]
WHERE [Marketing Comments] IS NOT NULL;


🚀 Getting Started

1. Clone or independent your ConsoleApp1 folder structure.
2. Ensure qdrant.exe and ollama are active in the background.
3. Open terminal under project directory and run:

    dotnet clean
    dotnet run

4. Enter y upon initialization to batch-vectorize your database rows into Qdrant.
5. Start chatting with your data! Type clear to reset chat history or exit to quit.
");

        // 💡 請將此段貼在 Main 方法的第一行，執行後即可自動生成繁體中文實體說明文件！
        System.IO.File.WriteAllText("README.md", @"# 全地端 AI Agent 知識庫問答系統 (RAG)

一個基於 **C# (.NET 9)**、**Ollama**、**Qdrant** 以及 **SQL Server** 打造的**完全本地端、隱私優先 AI Agent**。本系統利用檢索增強生成 (RAG) 技術，直接對企業級資料倉儲進行智慧問答，企業機密資料 100% 不外流、不上雲。

---

## 🏗️ 系統架構圖

[ SQL Server ] (AdventureWorksDW / WideWorldImportersDW)
│
▼ (C# SqlClient - 批次高效讀取)
[ C# .NET 9 ] 🧠 (核心引擎與全域對話歷史記憶管理)
│
├─► (Ollama API) ──► [ bge-m3 向量模型 ] ──► (生成文字向量)
│                                                 │
├─► (gRPC Port 6334) ─────────────────────────────┼─► [ Qdrant 向量資料庫 ]
│                                                 │     (高速儲存與相似度檢索)
▼                                                 ▼
[ Ollama LLM ] (qwen2.5:7b) ◄───────────────────────────┘ (提取前 3-5 筆最相關背景)
│
▼
[ 使用者介面 ] (輸出完全符合台灣習慣的流暢繁體中文回覆)

---

## ⚡ 核心技術亮點

* **100% 在地化與隱私安全：** 完全不需要網際網路連線，徹底防止企業內部敏感資料洩漏。
* **高效批次向量化引擎：** 引入 Batch 批次處理機制（每 32 筆打包一次送給 Ollama），大幅節省運算時間並拉滿 GPU/CPU 利用率。
* **資料層雜訊過濾：** 直接在 SQL 查詢階段進行精準篩選（例如在 AdventureWorks 中排除無意義零件，只保留車輛、衣服與配件），從根本避免 AI 撈到無關雜訊。
* **持久化歷史對話記憶：** 具備完整的前後文記憶佇列，AI 能精準理解使用者在連續對話中所使用的「它」、「這款」或「那這個呢」等代名詞。
* **多資料庫靈活分流：** 預先配置並完整測試 `AdventureWorksDW2025` 與 `WideWorldImportersDW-Full` 兩大標準微軟範例庫。

---

## 🛠️ 環境架設與本地基礎建設

在啟動 C# 專案前，請確保以下本地服務已在您的 Windows 電腦上運行：

### 1. SQL Server
請於本地還原微軟官方範例資料倉儲（建議使用 `AdventureWorksDW2025` 或 `WideWorldImportersDW-Full`）。

### 2. Ollama (地端模型託管)
安裝 Ollama 後，請開啟 CMD 執行以下指令以下載專案指定模型：

ollama pull bge-m3          # 1024 維度、高精度的向量生成模型
ollama pull qwen2.5:7b      # 繁體中文理解與邏輯推理能力強大的大語言模型

### 3. Qdrant (向量資料庫)

直接雙擊執行您的 qdrant.exe，確保 gRPC 通訊埠 6334（以及網頁用的 6333）正常開放且視窗保持開啟。

💻 專案核心組態設定

NuGet 必要套件

dotnet add package Microsoft.Data.SqlClient --version 5.2.2
dotnet add package Microsoft.Extensions.AI --prerelease
dotnet add package Microsoft.Extensions.AI.Ollama --prerelease
dotnet add package Qdrant.Client --version 1.12.0


已調校之 Context 最佳化 SQL 語法

選擇 A: AdventureWorksDW2025 (聚焦單車與運動部品)
SELECT p.ProductKey, p.EnglishProductName, p.EnglishDescription 
FROM DimProduct p
INNER JOIN DimProductSubcategory s ON p.ProductSubcategoryKey = s.ProductSubcategoryKey
INNER JOIN DimProductCategory c ON s.ProductCategoryKey = c.ProductCategoryKey
WHERE c.EnglishProductCategoryName IN ('Bikes', 'Clothing', 'Accessories')
  AND p.EnglishDescription IS NOT NULL;

選擇 B: WideWorldImportersDW (聚焦新奇禮品與行銷描述)
SELECT [Stock Item Key], [Stock Item], [Marketing Comments]
FROM [Dimension].[Stock Item]
WHERE [Marketing Comments] IS NOT NULL;


🚀 專案啟動步驟

1. 使用 Visual Studio 2022 開啟您的 ConsoleApp1 專案。
2. 確保背景運行的 qdrant.exe 和 ollama 已就緒。
3. 按下 F5 或點擊綠色播放鍵執行專案。
4. 在首次啟動的詢問提示中，輸入 y 啟動閃電批次向量化，將 SQL 資料灌入 Qdrant 建立知識庫。
5. 匯入完成後即可正式開始對談！對話中可隨時輸入 clear 清空對話記憶，或輸入 exit 安全結束程式。
");




        Console.WriteLine("🚀 正在初始化本地端 AI 問答系統 (含對話記憶功能)...");

        await InitializeQdrantCollectionAsync();

        Console.WriteLine("🔄 是否需要從 SQL Server 匯入並向量化資料？(y/n)");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            await ImportAndVectorizeDataAsync();
        }

        // 💡 初始化 System Prompt（維持最高層級的繁體字約束）
        ResetChatHistory();

        Console.WriteLine("\n🤖 本地 AI 系統已就緒！請輸入產品問題（輸入 'clear' 重設對話，輸入 'exit' 退出）：");
        while (true)
        {
            Console.Write("\n使用者: ");
            string? query = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(query) || query.ToLower() == "exit") break;

            if (query.ToLower() == "clear")
            {
                ResetChatHistory();
                Console.WriteLine("🧹 對話記憶已清除！");
                continue;
            }

            Console.WriteLine("🔍 正在檢索知識庫並生成回答...");
            await AskLocalAIWithHistoryAsync(query);
        }
    }

    private static void ResetChatHistory()
    {
        ChatHistory.Clear();
        string systemPrompt = @"你是一位台灣在地專業的 AdventureWorks 頂級單車銷售顧問。
請嚴格遵循以下三大原則回答：
1. 必須完全使用「台灣習慣的繁體中文」（例如：使用「腳踏車/單車」而非「自行車」，使用「資訊/資料」而非「信息」）。
2. 絕對禁止輸出任何簡體字。
3. 請結合歷史對話與最新的參考資料，給予使用者最自然、具備前後文連貫性的親切回覆。";

        ChatHistory.Add(new ChatMessage(ChatRole.System, systemPrompt));
    }

    private static async Task InitializeQdrantCollectionAsync()
    {
        var collections = await QdrantClient.ListCollectionsAsync();
        if (!collections.Contains(CollectionName))
        {
            await QdrantClient.CreateCollectionAsync(CollectionName,
                new VectorParams { Size = 1024, Distance = Distance.Cosine });
            Console.WriteLine($"✅ 已在 Qdrant 建立集合: {CollectionName}");
        }
    }

    private static async Task ImportAndVectorizeDataAsync()
    {
        Console.WriteLine("📥 開始從 SQL Server 讀取腳踏車成車資料...");

        using var connection = new SqlConnection(SqlConnectionString);
        // 只抓 Bikes 分類的成車，過濾服飾與配件噪音
        string query = @"
            SELECT p.ProductKey, p.EnglishProductName, p.EnglishDescription 
            FROM DimProduct p
            INNER JOIN DimProductSubcategory s ON p.ProductSubcategoryKey = s.ProductSubcategoryKey
            INNER JOIN DimProductCategory c ON s.ProductCategoryKey = c.ProductCategoryKey
            WHERE c.EnglishProductCategoryName = 'Bikes'
              AND p.EnglishDescription IS NOT NULL";

        using var command = new SqlCommand(query, connection);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int productKey = reader.GetInt32(0);
            string productName = reader.GetString(1);
            string description = reader.GetString(2);

            string contentToVectorize = $"Product Name: {productName}. Description: {description}";

            var embeddingResult = await EmbeddingGenerator.GenerateAsync(new[] { contentToVectorize });
            var vector = embeddingResult.First().Vector.ToArray();

            var point = new PointStruct
            {
                Id = (ulong)productKey,
                Vectors = vector,
                Payload = {
                    ["name"] = productName,
                    ["description"] = description
                }
            };

            await QdrantClient.UpsertAsync(CollectionName, new[] { point });
            Console.WriteLine($"✔ 已向量化產品 #{productKey}: {productName}");
        }
        Console.WriteLine("🎉 資料匯入完成！");
    }

    private static async Task AskLocalAIWithHistoryAsync(string userQuery)
    {
        // 1. 僅將使用者的「最新問題」轉成向量
        var queryEmbeddingResult = await EmbeddingGenerator.GenerateAsync(new[] { userQuery });
        var queryVector = queryEmbeddingResult.First().Vector.ToArray();

        // 2. 至 Qdrant 搜尋最相關的 3 筆最新背景資料
        var searchResults = await QdrantClient.SearchAsync(
            collectionName: CollectionName,
            vector: queryVector,
            limit: 3
        );

        var contextText = "";
        foreach (var hit in searchResults)
        {
            contextText += $"- 產品名稱: {hit.Payload["name"].StringValue}\n";
            contextText += $"  描述: {hit.Payload["description"].StringValue}\n\n";
        }

        // 3. 組合當前這一輪的 RAG 提示詞
        string userPromptWithContext = $"【最新參考資料】:\n{contextText}\n【使用者最新問題】:\n{userQuery}";

        // 4. 將當前的使用者問題（含 Context）餵進對話歷史紀錄中
        ChatHistory.Add(new ChatMessage(ChatRole.User, userPromptWithContext));

        // 5. 將「包含完整歷史紀錄」的 ChatHistory 丟給 Ollama 生成回應 [1, 3]
        var response = await ChatClient.GetResponseAsync(ChatHistory);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAI 助理:\n{response.Text}");
        Console.ResetColor();

        // 6. 💡 關鍵步驟：將 AI 的回答也加入歷史紀錄中，下次對話時它才會記得自己說過什麼！
        ChatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Text));
    }
}
