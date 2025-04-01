# Tender Management System

## Overview
The **Tender Management System** is a robust and scalable solution designed to streamline the management of investors, bidding companies, equipment, and offers. It provides powerful tools for editing agreements, dynamically binding data in Word documents, and exporting essential records. The system includes advanced user, role, and permission management features, ensuring secure authentication and authorization.

## Features
- **Manage Investors**: Add, update, and monitor investors.
- **Manage Bidding Companies**: Keep track of bidding companies and their participation.
- **Manage Equipment & Equipment Models**: Catalog and maintain detailed records of equipment and models.
- **Manage Tenders & Tender Items**: Create, modify, and review tenders and associated items.
- **Calculate Tender Amounts**: Calculate tender total amounts and compare with another tenders
- **Save Tender Items Prices**: Save each tender items prices on tenders
- **Units Management**: Modify units for assigned each tender items 
- **Edit Agreements**: Update and maintain contractual agreements dynamically.
- **Dynamic Word Documentation Binding & Export**: Seamlessly bind data to Word documents and export them.
- **User, Role and Permission Management**: Implement fine-grained role-based access control (RBAC) for security.
- **Authentication & Authorization**: Secure system access with **ASP.NET Identity**.

## Technologies Used
The system is built using cutting-edge technologies to ensure high performance and scalability:

- **.NET 9 & C#**
- **Entity Framework Core 9** (Code-First Migrations & Data Seeding)
- **ASP.NET Identity** for Authentication & Authorization
- **Razor Pages** for UI Development
- **MsSql** for Database
- **User, Role, and Permission Management** for Access Control

## Installation & Setup

### 1. Clone the Repository
```sh
git clone https://github.com/your-repo/offer-management-system.git
cd offer-management-system
```

### 2. Set Up the Database
- Update the `appsettings.json` file with your database connection string.
- Apply migrations:
```sh
dotnet ef database update
```

### 3. Run the Application
```sh
dotnet run
```

## Contribution
We welcome contributions! Please follow these steps:
1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Submit a pull request.

## License
This project is licensed under the **MIT License** - see the `LICENSE` file for details.

## Contact
For any inquiries or support, feel free to contact us at **mustafaalkan64@gmail.com**.

