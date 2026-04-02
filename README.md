# 🦷 Majestic Golden Dental Clinic API 🦷
### *واجهة برمجة عيادة ماجستيك غولدن لطب الأسنان*

![Majestic Golden Banner](https://github.com/QusayBider/MAJESTIC_GOLDEN_Api/blob/main/MAJESTIC_GOLDEN_Api/wwwroot/Files/banner.png?raw=true)

A professional, high-performance, and bilingual (Arabic/English) **Dental Clinic Management System API**. Built with .NET 9 and designed using a robust 3-Layer Architecture to handle multi-branch clinics, patient treatment cases, billing, and specialized dental lab integrations.

---

## 🌟 Key Features

The system is designed to provide a comprehensive backend for modern dental clinics, including:

-   **Multi-Branch Support**: Manage multiple clinic locations within a single system.
-   **Patient Management**: Full treatment history, tooth-specific documentation (PatientTooth), and attachment management.
-   **Treatment Case Management**: Detailed tracking of dental cases from initial diagnosis to completion.
-   **Appointment Scheduling**: Seamless booking and management of patient visits.
-   **Financial Management**:
    -   Invoicing and payment tracking.
    -   **Stripe Integration** for digital payments.
    -   Patient debt management.
-   **Laboratory Requests**: Specialized module for managing dental lab work and case transfers.
-   **Auditing**: Comprehensive AuditLog system for tracking all administrative and clinical actions.
-   **Bilingual Support**: Full localization for both Arabic and English languages.

---

## 🏗️ Architecture

The project follows the **Clean 3-Layer Architecture** pattern to ensure scalability, maintainability, and separation of concerns:

1.  **Presentation Layer (API)**: Handles HTTP requests, JWT authentication, and Swagger/OpenAPI exposure.
2.  **Business Logic Layer (BLL)**: Contains services, business rules, and object mapping (AutoMapper).
3.  **Data Access Layer (DAL)**: Manages database interactions, migrations, repositories, and domain models.

---

## 🛠️ Tech Stack

-   **Runtime**: .NET 9
-   **Database**: SQL Server
-   **ORM**: Entity Framework Core
-   **Identity**: ASP.NET Core Identity for user and role management.
-   **Security**: JWT (JSON Web Token) authentication with Bearer support.
-   **Documentation**: Swagger/OpenAPI & Scalar for interactive API references.
-   **Mapping**: AutoMapper for DTO and Domain Model transitions.
-   **Payment Gateway**: Stripe.
-   **Localization**: Bilingual (English & Arabic) resource-based localization.

---

## 🚀 Getting Started

### Prerequisites

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer)

### Setup & Configuration

1.  **Clone the project**:
    ```bash
    git clone https://github.com/your-repo/majestic-golden-api.git
    cd majestic-golden-api
    ```

2.  **Configure Database & Secret Keys**:
    Open `appsettings.json` in the API project and provide your connection string and security keys:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=...;Database=MajesticGoldenDB;..."
      },
      "JwtConnection": {
        "securityKey": "your-very-strong-32-char-key-here"
      },
      "Stripe": {
        "SecretKey": "sk_test_...",
        "WebhookSecret": "whsec_..."
      }
    }
    ```

3.  **Update Database**:
    Run migrations to create the database schema:
    ```bash
    dotnet ef database update --project MAJESTIC_GOLDEN_Api.DAL --startup-project MAJESTIC_GOLDEN_Api
    ```

4.  **Run the Project**:
    ```bash
    dotnet run --project MAJESTIC_GOLDEN_Api
    ```

---

## 📖 API Documentation

Once the project is running (usually on `https://localhost:7198`), you can access the interactive documentation:

-   **Swagger UI**: `https://localhost:7198/swagger/index.html`
-   **Scalar API Reference**: `https://localhost:7198/scalar/v1`

---

## 📂 Project Structure

```text
├── MAJESTIC_GOLDEN_Api/        # Presentation Layer (Controllers, Middlewares, Resources)
├── MAJESTIC_GOLDEN_Api.BLL/    # Business Logic Layer (Services, Mapping Profiles)
├── MAJESTIC_GOLDEN_Api.DAL/    # Data Access Layer (Models, Repositories, Migrations)
└── assets/                     # Media & Documentation Assets
```

---

## 📞 Contact & Support

For support or business inquiries:
-   **Organization**: Majestic Golden Dental Clinic
-   **Email**: QusayBdier@gmail.com.com

