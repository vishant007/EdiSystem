# EDISystem 

---

## 🛠️ Project Overview

### Components
1. **EDI315Parser** (Console App)  
   - Parses EDI315 `.txt` files.
   - Converts the parsed data into JSON format.
   - Stores the JSON data in **Azure Cosmos DB**.

2. **EDI315WatchlistApi** (ASP.NET Core API)  
   - **User Authentication**: Uses JWT tokens for secure user access.
   - Fetches container data from **Cosmos DB**.
   - Allows users to add containers to their **Watchlist**.
   - Stores Watchlist data in **MSSQL**.

3. **EDI315PaymentApi** (ASP.NET Core API)  
   - Subscribes to **Azure Service Bus** for messages when containers are added to the watchlist.
   - Processes payments for containers.
   - Generates a unique transaction ID.
   - Updates the `fee` column in **Cosmos DB** to mark the payment as **Paid**.
   - Stores payment transaction details in **MSSQL**.
   - Sends a message via **Azure Service Bus** that payment is complete.

4. **EDIUi** (Angular 18 Frontend)  
   - Provides a user-friendly interface for interacting with:
     - **EDI315WatchlistApi**: Managing container watchlists.
     - **EDI315PaymentApi**: Viewing and processing payments.
   - Built with **Angular 18**.

---

## 🚀 Features
- **File Parsing**: Parses EDI315 files and stores structured data in Cosmos DB.
- **Authentication**: Secure JWT-based user authentication.
- **Watchlist Management**: Add, view, and manage container watchlists.
- **Payment Processing**: Seamlessly process payments with Azure Service Bus integration.
- **Dockerized Deployment**: MSSQL and APIs are dockerized for easier deployment.

---

## 🧰 Technologies Used
- **Backend**:  
  - .NET Core (EDI315Parser, EDI315WatchlistApi, EDI315PaymentApi)
- **Frontend**:  
  - Angular 18 (EDIUi)
- **Databases**:  
  - Azure Cosmos DB (Container Data)  
  - MSSQL (Watchlist & Payment Data)
- **Messaging**:  
  - Azure Service Bus
- **Deployment**:  
  - Docker (APIs and MSSQL)




