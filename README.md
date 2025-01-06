# Discount Code System

## Overview
1. **API**: A SignalR-based server that handles requests for generating and using discount codes. It employs clean architecture principles with three layers:
   - **Core**: Contains business logic, request/response models, and validation rules.
   - **Infrastructure**: Handles in-memory caching and persistent storage.
   - **API**: Exposes endpoints via SignalR.

2. **Client**: A console application that interacts with the API to generate and use discount codes. It also supports offline operations by queueing requests when the server is unavailable.

---

## Features
- Generate discount codes with customizable lengths (7-8 characters).
- Use discount codes to mark them as used.
- Persist generated codes across service restarts.
- Offline operation with automatic synchronization.
- Low latency and parallel request handling.

---

## Prerequisites
- .NET 8 SDK

## Setup Instructions

### API
1. **Navigate to the API project folder:**
   ```bash
   cd DiscountCodeSystem.API
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the API:**
   ```bash
   dotnet run
   ```

4. **Access the SignalR Hub:**
   The hub is hosted at `http://localhost:5225` by default. Configure the port in `appsettings.json` if needed.

---

### Client
1. **Navigate to the Client project folder:**
   ```bash
   cd DiscountCodeSystem.Client
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the Client:**
   ```bash
   dotnet run
   ```

4. **Using the Client:**
   - When prompted, enter the desired operation (`generate` or `use`).
   - For `generate`, specify the number of codes and their length (e.g., 10 codes of length 8).
   - For `use`, provide a previously generated discount code.

---

## Offline Support
The client queues operations when the server is unavailable and synchronizes them automatically when the connection is re-established.

- Offline operations are stored in a local file (`offline_operations.json`).
- Synchronization occurs automatically upon reconnection to the API.

---

## Unit Tests
The solution includes unit tests for key business logic:

1. **Navigate to the test project folder:**
   ```bash
   cd DiscountCodeSystem.Tests
   ```

2. **Run tests:**
   ```bash
   dotnet test
   ```
