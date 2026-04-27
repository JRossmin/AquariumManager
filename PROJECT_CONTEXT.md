# Aquarium Manager - Project Context & Business Rules

> This document contains comprehensive context for the Aquarium Manager project, including business rules, architecture decisions, and implementation details. Use it for creating diagrams, understanding the domain, and future development.

---

## 1. PROJECT OVERVIEW

### What It Is
**Aquarium Manager** is a full-stack web application designed to help aquarium store owners manage their business operations, including:
- **Inventory Management**: Track aquatic creatures (fish, invertebrates, plants, corals) from arrival to sale
- **Supplier Management**: Manage relationships with suppliers and breeders
- **Sales Tracking**: Record sales with automatic FIFO stock deduction
- **Reporting**: Generate reports on stock levels, mortality, sales performance, and inventory valuation

### Tech Stack
**Backend:**
- ASP.NET Core 10.0 (API REST)
- Entity Framework Core (ORM)
- SQL Server LocalDB (Database)
- Clean Architecture (Domain → Application → Infrastructure → API)

**Frontend:**
- React 19 + Vite
- Mantine UI (Component Library)
- React Router DOM (Routing)
- Axios (HTTP Client)

---

## 2. BUSINESS RULES & DOMAIN LOGIC

### 2.1 Species Management
**Species** represents types of aquatic life that can be sold. Each species has:
- **CommonName**: Commercial name (e.g., "Neon Tetra")
- **ScientificName**: Biological classification (e.g., "Paracheirodon innesi")
- **Category**: Classification (Fish, Invertebrate, Plant, Coral, etc.)
- **MinTemperature / MaxTemperature**: Required water temperature range (°C)
- **MinPH / MaxPH**: Required pH range
- **ImageUrl**: Optional image for visual reference

**Business Rules:**
- A species can have multiple inventory lots
- Species must exist before creating inventory lots
- Category is used for grouping and reporting

### 2.2 Supplier Management
**Supplier** represents entities that provide aquatic life (suppliers, breeders, wholesalers). Each supplier has:
- **Name**: Company or person name
- **ContactInfo**: Address or general contact details
- **Phone**: Phone number
- **Email**: Email address
- **Notes**: Additional information

**Business Rules:**
- A supplier can provide multiple inventory lots
- Suppliers can be updated or deleted (if not referenced by active lots)
- Contact information can be updated using the `UpdateContact()` domain method

### 2.3 Inventory Lots
**InventoryLot** represents a batch of aquatic creatures received from a supplier. This is the core inventory tracking entity.

**Key Properties:**
- **SpeciesId**: Which species this lot contains
- **SupplierId**: Who provided this lot
- **ArrivalDate**: When the lot was received
- **InitialQuantity**: How many creatures arrived
- **DeadOnArrival**: Creatures that died during transport
- **UnitCost**: Cost per creature
- **Notes**: Additional information

**Critical Business Rules:**

1. **Current Stock Calculation:**
   ```
   CurrentStock = InitialQuantity - DeadOnArrival - TotalMortality
   ```
   Where `TotalMortality` is the sum of all mortality records for this lot.

2. **Mortality Tracking:**
   - Every death must be recorded with: Date, Quantity, Cause, Notes
   - Common causes: "Disease", "Water Quality", "Transport", "Sold", "Unknown"
   - When creatures are sold, mortality is recorded with cause = "Sold" (for FIFO tracking)

3. **FIFO Stock Deduction:**
   - When a sale is created, stock is deducted from the **oldest lots first** (by ArrivalDate)
   - This is implemented by creating mortality records with cause = "Sold"
   - Ensures older inventory is sold first (reduces losses from natural mortality)

4. **Stock Validation:**
   - Cannot sell more than available stock
   - Sale creation fails if insufficient stock for any species

### 2.4 Sales Management
**Sale** represents a transaction where creatures are sold to customers.

**Structure:**
- **Sale**: Header with Date, CustomerName
- **SaleItem**: Line items with Species, Quantity, UnitPrice

**Business Rules:**

1. **Sale Creation Process:**
   ```
   1. Validate all items have quantity > 0 and price >= 0
   2. Check species exists for each item
   3. Verify sufficient biological stock for each species
   4. Begin transaction
   5. Create Sale record
   6. For each SaleItem:
      - Deduct stock from oldest lots first (FIFO)
      - Create MortalityRecord with cause = "Sold"
   7. Commit transaction
   ```

2. **Stock Deduction Logic:**
   - Lots are ordered by ArrivalDate (oldest first)
   - Stock is deducted from each lot until the sale quantity is fulfilled
   - If a lot runs out, move to the next oldest lot

3. **Transaction Safety:**
   - All operations must succeed or all rollback
   - Prevents partial sales that would corrupt inventory

### 2.5 Mortality Records
**MortalityRecord** tracks deaths in inventory lots for analysis and reporting.

**Properties:**
- **InventoryLotId**: Which lot experienced the mortality
- **Date**: When the death occurred
- **Quantity**: How many creatures died
- **Cause**: Reason (Disease, Sold, Water Quality, etc.)
- **Notes**: Additional context

**Business Rules:**
- Mortality reduces available stock
- "Sold" mortality is created automatically during sales
- Other causes are manually recorded
- Mortality reports help identify problematic suppliers or species

---

## 3. ARCHITECTURE PATTERNS

### 3.1 Clean Architecture Layers

```
┌─────────────────────────────────────────────────┐
│              AquariumManager.Api                │  ← Controllers, DI, Middleware
├─────────────────────────────────────────────────┤
│         AquariumManager.Application             │  ← Services, DTOs, Business Logic
├─────────────────────────────────────────────────┤
│          AquariumManager.Domain                 │  ← Entities, Interfaces (zero deps)
├─────────────────────────────────────────────────┤
│       AquariumManager.Infrastructure            │  ← EF Core, Repositories, UnitOfWork
└─────────────────────────────────────────────────┘
```

**Dependency Rule:** Inner layers know nothing about outer layers. Outer layers depend on inner layers.

### 3.2 Repository Pattern

**Characteristics:**
- One repository per aggregate root (not generic)
- Interface in Domain layer, implementation in Infrastructure
- Specific query methods (e.g., `GetBySpeciesAsync`, `GetByDateRangeAsync`)

**Repositories:**
1. `ISpeciesRepository`: CRUD for species
2. `ISupplierRepository`: CRUD for suppliers
3. `IInventoryLotRepository`: Lots, mortality, stock queries
4. `ISaleRepository`: Sales queries

### 3.3 Unit of Work Pattern

**Purpose:** Ensure multiple repository operations execute in a single transaction.

**Implementation:**
```csharp
using (var uow = new UnitOfWork(dbContext))
{
    await uow.BeginTransactionAsync();
    try
    {
        // Multiple repository operations
        await uow.SaveChangesAsync();
        await uow.CommitTransactionAsync();
    }
    catch
    {
        await uow.RollbackTransactionAsync();
        throw;
    }
}
```

### 3.4 DTO Pattern

**Purpose:** Decouple API contracts from domain entities.

**DTO Categories:**
1. **Create DTOs**: For POST requests (no Id)
2. **Update DTOs**: For PUT requests (with Id)
3. **Response DTOs**: For GET requests (computed properties included)

### 3.5 OperationResult Pattern

**Purpose:** Handle expected failures without exceptions.

```csharp
public class OperationResult
{
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; }
    
    public static OperationResult Ok() => new() { Success = true };
    public static OperationResult Fail(string error) => new() { Success = false, ErrorMessage = error };
}

public class OperationResult<T> : OperationResult
{
    public T? Data { get; private set; }
    public static OperationResult<T> Ok(T data) => new() { Success = true, Data = data };
}
```

---

## 4. DATABASE SCHEMA

### 4.1 Entity Relationships

```
Species (1) ←──→ (∞) InventoryLot (∞) ←──→ (1) Supplier
                        │
                        │
                        ↓ (∞)
                MortalityRecord
                
Sale (1) ←──→ (∞) SaleItem (∞) ←──→ (1) Species
```

### 4.2 Tables

**Species:**
```sql
Id (PK, int)
CommonName (nvarchar(200))
ScientificName (nvarchar(200))
Category (nvarchar(100))
MinTemperature (decimal(5,2))
MaxTemperature (decimal(5,2))
MinPH (decimal(3,2))
MaxPH (decimal(3,2))
ImageUrl (nvarchar(500))
```

**Supplier:**
```sql
Id (PK, int)
Name (nvarchar(200))
ContactInfo (nvarchar(500))
Phone (nvarchar(50))
Email (nvarchar(200))
Notes (nvarchar(1000))
```

**InventoryLot:**
```sql
Id (PK, int)
SpeciesId (FK → Species.Id)
SupplierId (FK → Supplier.Id)
ArrivalDate (datetime2)
InitialQuantity (int)
DeadOnArrival (int)
UnitCost (decimal(18,2))
Notes (nvarchar(1000))
```

**MortalityRecord:**
```sql
Id (PK, int)
InventoryLotId (FK → InventoryLot.Id)
Date (datetime2)
Quantity (int)
Cause (nvarchar(100))
Notes (nvarchar(500))
```

**Sale:**
```sql
Id (PK, int)
Date (datetime2)
CustomerName (nvarchar(200))
```

**SaleItem:**
```sql
Id (PK, int)
SaleId (FK → Sale.Id, CASCADE DELETE)
SpeciesId (FK → Species.Id, RESTRICT)
Quantity (int)
UnitPrice (decimal(18,2))
```

---

## 5. API ENDPOINTS

### 5.1 Species Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Species` | Get all species |
| GET | `/api/Species/{id}` | Get species by ID |
| POST | `/api/Species` | Create new species |
| PUT | `/api/Species/{id}` | Update species |
| DELETE | `/api/Species/{id}` | Delete species |

### 5.2 Suppliers Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Suppliers` | Get all suppliers |
| GET | `/api/Suppliers/{id}` | Get supplier by ID |
| POST | `/api/Suppliers` | Create new supplier |
| PUT | `/api/Suppliers/{id}` | Update supplier |
| DELETE | `/api/Suppliers/{id}` | Delete supplier |

### 5.3 Inventory Lots Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/InventoryLots/{id}` | Get lot by ID |
| GET | `/api/InventoryLots/by-species/{speciesId}` | Get lots by species |
| GET | `/api/InventoryLots/biological-stock/{lotId}` | Get biological stock |
| POST | `/api/InventoryLots` | Create new lot |
| POST | `/api/InventoryLots/register-mortality` | Register mortality |

### 5.4 Sales Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Sales` | Create sale (FIFO deduction) |
| GET | `/api/Sales` | Get all sales |
| GET | `/api/Sales/{id}` | Get sale by ID |
| GET | `/api/Sales/by-date-range` | Get sales by date range |

### 5.5 Catalog Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Catalog` | Get species with stock > 0 |

### 5.6 Reports Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Reports/stock` | Current stock report |
| GET | `/api/Reports/mortality` | Mortality report (filterable) |
| GET | `/api/Reports/sales` | Sales report (date range) |
| GET | `/api/Reports/valuation` | Inventory valuation |

---

## 6. FRONTEND STRUCTURE

### 6.1 Pages to Implement

1. **DashboardPage** (`/`)
   - KPIs: Total Species, Total Stock, Recent Sales, Mortality Alerts
   - Calls: Catalog API, Sales API, Reports API

2. **SpeciesPage** (`/species`)
   - Table with search/filter
   - Full CRUD (Create, Read, Update, Delete)
   - Calls: Species API

3. **SuppliersPage** (`/suppliers`)
   - Table with CRUD operations
   - Calls: Suppliers API

4. **InventoryLotsPage** (`/inventory`)
   - Existing: Create lot form
   - Add: List view with stock status
   - Add: Register mortality form
   - Calls: Inventory Lots API

5. **SalesPage** (`/sales`)
   - Create sale form (multi-item)
   - Recent sales list
   - Calls: Sales API, Catalog API

6. **CatalogPage** (`/catalog`)
   - View species with available stock
   - Read-only display
   - Calls: Catalog API

7. **ReportsPage** (`/reports`)
   - Tabbed interface for 4 report types
   - Filters (date range, species, etc.)
   - Calls: Reports API

### 6.2 Component Structure

```
src/
├── api/
│   ├── apiClient.js              (Axios instance)
│   ├── species.js                (Species API calls)
│   ├── suppliers.js              (Suppliers API calls)
│   ├── inventoryLots.js          (Inventory Lots API calls)
│   ├── sales.js                  (Sales API calls)
│   ├── catalog.js                (Catalog API calls)
│   └── reports.js                (Reports API calls)
├── components/
│   ├── layout/
│   │   └── AppLayout.jsx         (Shell with sidebar)
│   ├── species/
│   │   ├── SpeciesForm.jsx
│   │   └── SpeciesTable.jsx
│   ├── suppliers/
│   │   ├── SupplierForm.jsx
│   │   └── SupplierTable.jsx
│   ├── inventory/
│   │   ├── InventoryLotForm.jsx  (EXISTS)
│   │   ├── MortalityForm.jsx
│   │   └── InventoryLotsTable.jsx
│   ├── sales/
│   │   ├── CreateSaleForm.jsx
│   │   └── SalesList.jsx
│   └── reports/
│       ├── StockReport.jsx
│       ├── MortalityReport.jsx
│       ├── SalesReport.jsx
│       └── ValuationReport.jsx
├── pages/
│   ├── DashboardPage.jsx
│   ├── SpeciesPage.jsx
│   ├── SuppliersPage.jsx
│   ├── InventoryLotsPage.jsx     (EXISTS)
│   ├── SalesPage.jsx
│   ├── CatalogPage.jsx
│   └── ReportsPage.jsx
├── App.jsx                       (Router + Layout)
├── main.jsx                      (Entry point)
└── index.css                     (Global styles)
```

---

## 7. DESIGN DECISIONS

### 7.1 Why FIFO?
- Older stock is sold first to minimize natural mortality losses
- Matches real-world aquarium business practices
- Provides better cost tracking accuracy

### 7.2 Why Mortality Records for Sales?
- Unified tracking system for all stock reductions
- Enables accurate "sold" vs "died" reporting
- Maintains single source of truth for stock calculations

### 7.3 Why No AutoMapper?
- Manual mapping is more transparent
- Easier to debug and customize
- Keeps project lightweight

### 7.4 Why Non-Generic Repositories?
- Each aggregate has specific query needs
- Avoids leaky abstractions
- Enables optimized queries per entity

### 7.5 Why OperationResult Instead of Exceptions?
- Expected failures (validation, stock issues) shouldn't be exceptions
- Better performance (no stack trace overhead)
- Clearer API responses

---

## 8. FUTURE ENHANCEMENTS (Layers 2 & 3)

### Layer 2: Advanced Management
- **Water Quality Tracking**: Log pH, temperature, ammonia levels
- **Health Records**: Track treatments, medications, quarantines
- **Breeding Management**: Track breeding pairs, spawns, fry survival
- **Automated Alerts**: Low stock, high mortality, water parameter warnings
- **Barcode/QR Scanning**: Quick lot identification

### Layer 3: Sales & E-commerce
- **Point of Sale (POS)**: In-store sales with receipt printing
- **Online Store**: Customer-facing e-commerce
- **Customer Management**: CRM with purchase history
- **Pricing Strategies**: Tiered pricing, wholesale discounts
- **Invoice Generation**: PDF invoices with business logo
- **Payment Processing**: Credit card, cash, account tracking

### Reporting Enhancements
- **Profit/Loss Analysis**: Revenue vs. cost per species/lot
- **Supplier Performance**: Mortality rates, cost comparisons
- **Seasonal Trends**: Sales patterns by month/season
- **Customer Analytics**: Top customers, purchase frequency
- **Inventory Turnover**: How fast stock sells

---

## 9. DEVELOPMENT WORKFLOW

### Backend Commands
```bash
# Run API
dotnet run --project src/AquariumManager.Api

# Add migration
dotnet ef migrations add MigrationName --project src/AquariumManager.Infrastructure --startup-project src/AquariumManager.Api

# Update database
dotnet ef database update --project src/AquariumManager.Infrastructure --startup-project src/AquariumManager.Api

# Build solution
dotnet build AquariumManager.slnx
```

### Frontend Commands
```bash
# Development server
npm run dev

# Build for production
npm run build

# Lint code
npm run lint
```

---

## 10. KEY BUSINESS FLOWS

### 10.1 Receiving New Inventory
```
1. Create Species (if new type)
2. Create Supplier (if new source)
3. Create InventoryLot:
   - Species, Supplier, ArrivalDate
   - InitialQuantity, DeadOnArrival, UnitCost
4. System calculates: CurrentStock = Initial - DOA - Mortality
```

### 10.2 Recording Mortality
```
1. Select InventoryLot
2. Create MortalityRecord:
   - Date, Quantity, Cause, Notes
3. System recalculates: CurrentStock
4. Report updates: Mortality rate increases
```

### 10.3 Making a Sale
```
1. Customer selects species and quantities
2. Create Sale with SaleItems
3. System validates stock for each species
4. For each item:
   - Find oldest lots with stock (FIFO)
   - Deduct quantity from each lot
   - Create MortalityRecord (cause = "Sold")
5. Sale saved in transaction
6. Reports update: Revenue, stock levels
```

### 10.4 Generating Reports
```
1. Stock Report:
   - For each species, sum stock from open lots
   - Group by supplier, calculate cost value
   
2. Mortality Report:
   - Aggregate mortality by species, cause
   - Separate "Sold" from other causes
   
3. Sales Report:
   - Sum revenue, items sold in date range
   - Identify top-selling species
   
4. Valuation Report:
   - Calculate total cost of current stock
   - Group by category
```

---

## 11. COMMON CAUSES FOR MORTALITY

| Cause | Description | When to Use |
|-------|-------------|-------------|
| **Sold** | Creature was sold | Automatic during sales |
| **Disease** | Death from illness | Infection, parasites |
| **Water Quality** | Parameter issues | Ammonia spike, wrong pH |
| **Transport** | Shipping stress | During arrival from supplier |
| **Aggression** | Killed by other creatures | Tank mate conflicts |
| **Unknown** | Cause unclear | When reason is unknown |
| **Old Age** | Natural lifespan | For long-held inventory |

---

## 12. SPECIES CATEGORIES

Common categories for classification:
- **Fish**: Freshwater fish (tetras, cichlids, guppies)
- **Fish**: Saltwater fish (clownfish, tangs, wrasses)
- **Invertebrate**: Shrimp, snails, crabs, crayfish
- **Plant**: Live aquatic plants (anubias, java fern)
- **Coral**: Soft corals, LPS, SPS
- **Other**: Algae, bacteria cultures, feeder fish

---

*Created: April 15, 2026*
*Last Updated: April 15, 2026*
