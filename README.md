# Fully Local AI Agent with RAG Knowledge Base

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
* **Persistent Chat History:** Maintains contextual awareness, allowing the agent to understand pronouns like "it" or "this one" across continuous dialogues.
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
