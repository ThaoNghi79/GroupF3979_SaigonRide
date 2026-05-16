# SaigonRide – Distributed Vehicle Rental System
Group F3979 | Liêu Thảo Nghi & Nguyễn Thị Như Quỳnh

---

## Prerequisites

Make sure you have the following installed before running the project:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET and web development workload)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (optional, for viewing database)

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ThaoNghi79/GroupF3979_SaigonRide.git
cd GroupF3979_SaigonRide
```

### 2. Configure the Database Connection

Open `appsettings.json` and update the connection string to match your local SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SaigonRideDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

> If you are using SQL Server Express, replace `(localdb)\\mssqllocaldb` with `YOUR_PC_NAME\\SQLEXPRESS`

### 3. Apply Database Migrations

Open the **Package Manager Console** in Visual Studio (Tools → NuGet Package Manager → Package Manager Console) and run:

```powershell
Update-Database
```

This will automatically create the database and all required tables.

### 4. Run the Application

Press **F5** in Visual Studio or run:

```bash
dotnet run
```

The application will start at `https://localhost:7xxx` (port number shown in terminal).

---

## Default Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@saigonride.com | Admin@123 |
| Local Commuter | user@saigonride.com | User@123 |
| Foreign Tourist | tourist@saigonride.com | Tourist@123 |

---

## Live Deployment

The application is also deployed on Microsoft Azure:

🔗 [https://f3979-saigonride-drdrc7emfecaeybf.southeastasia-01.azurewebsites.net/Auth](https://f3979-saigonride-drdrc7emfecaeybf.southeastasia-01.azurewebsites.net/Auth)

---

## Project Structure

```
SaigonRide/
├── Controllers/        # MVC Controllers (Vehicle, Station, Rental, Payment, Report)
├── Models/             # Entity models (User, Vehicle, Station, Rental, Payment)
├── Services/           # Business logic (PricingService, InventoryService, PaymentService...)
├── Views/              # Razor Views with Bootstrap 5.3
├── Migrations/         # Entity Framework Code First migrations
├── wwwroot/            # Static files (CSS, JS)
└── appsettings.json    # Configuration (connection string)
```

---

## Troubleshooting

### ❌ Error: `Method 'Identifier' in type 'CSharpHelper' does not have an implementation`

This error occurs when EF Core packages have mismatched versions.

**Fix:**

Open **Package Manager Console** and run:

```powershell
Uninstall-Package Microsoft.EntityFrameworkCore.Design
Uninstall-Package Microsoft.EntityFrameworkCore.Tools
```

Then reinstall with the correct version:

```powershell
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.0
```

Then run again:

```powershell
Update-Database
```

Make sure all EF Core packages in your `.csproj` are the **same version**:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
```

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core (Code First) |
| Database | SQL Server / Azure SQL |
| Front-End | Bootstrap 5.3 + Razor Views |
| Deployment | Microsoft Azure App Service |
