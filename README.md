# ABCRetailManagement System

A cloud-based retail management web application developed using ASP.NET Core MVC and Microsoft Azure Storage services.

The application provides functionality for managing customers, products and orders, while demonstrating the use of Azure Table Storage, Blob Storage, Queue Storage and Azure File Storage.

## Project Overview

ABC Retail Management is a retail management system designed to demonstrate how a web application can integrate multiple Azure Storage services.

The system allows users to:

- Manage customer records
- Manage product records
- Upload and display product images
- Create and manage orders
- Process orders using Azure Queue Storage
- Track product stock
- Store daily application activity logs
- View and download application log files
- Access the application through Azure App Service

## Technologies Used

### Application

- ASP.NET Core MVC
- C#
- .NET 8
- Razor Views
- HTML
- CSS
- Bootstrap

### Azure Services

- Azure Table Storage
- Azure Blob Storage
- Azure Queue Storage
- Azure File Storage
- Azure App Service

### Development Tools

- Visual Studio
- Git
- GitHub

## Azure Storage Architecture

The application uses different Azure Storage services according to the type of information being stored.

| Azure Service | Purpose |
|---|---|
| Azure Table Storage | Stores customers, products and orders |
| Azure Blob Storage | Stores product images |
| Azure Queue Storage | Handles order transactions and processing |
| Azure File Storage | Stores daily application activity logs |
| Azure App Service | Hosts the web application |

### Table Storage

Azure Table Storage is used for structured application data.

The application stores:

- Customer records
- Product records
- Order records

Users can create, view, edit and delete records through the web application.

### Blob Storage

Azure Blob Storage is used for product images.

When a product image is uploaded, the image is stored in an Azure Blob Storage container. The product record stores the image reference, allowing the application to retrieve and display the image.

If a product does not have an associated image, the application displays a default product image.

### Queue Storage

Azure Queue Storage is used to support the order processing workflow.

When an order is created, the transaction can be placed into the queue for processing. The application provides a **Process Next Order** function which processes the next available transaction and updates the relevant order and inventory information.

This demonstrates how queue-based processing can separate the creation of a transaction from its processing.

### File Storage

Azure File Storage is used to store daily application activity logs.

Log files are generated using a daily naming format, for example:

```text
log-20260813.txt
log-20260814.txt
log-20260815.txt
```

### Project Developer
St10355256 Halalisile Mzobe
