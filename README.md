# Web ERP

Web ERP is a small web application for customers, invoices, vendors, and purchase orders. It is a 2026 portfolio recreation of an academic ERP project from 2023.

The 2023 project covered this same kind of work. This repository is a new implementation. The source code, database, screenshots, and features shown here were built for this portfolio and are not the original 2023 source.

All customers, vendors, invoices, and amounts in the application are fictional sample data.

## Technology

- C#
- ASP.NET Core MVC on .NET 10
- Entity Framework Core
- Microsoft SQL Server 2022
- HTML, CSS, and JavaScript

## Architecture

The site is an ASP.NET Core MVC application.

- Razor views in `src/WebErp/Views` render the pages.
- Controllers in `src/WebErp/Controllers` handle the dashboard, customers, invoices, vendors, and purchase orders.
- `AppDbContext` maps the models to SQL Server.
- `wwwroot/css/site.css` and `wwwroot/js/site.js` provide the layout, the mobile menu, and invoice and purchase-order line editing.
- On startup the application creates the `WebErp` database if it does not exist, then loads sample data when the customer table is empty.

There is no login. The pages are available as soon as the site starts.

## Features

- Dashboard with customer and vendor counts, open invoice and purchase-order totals, and recent documents
- Customer list, detail, create, and edit, including an open balance
- Invoice list, detail, create, and edit, with line items, quantities, prices, and a total
- Vendor list, detail, create, and edit
- Purchase-order list, detail, create, and edit, with line items and a total
- Invoice statuses: Draft, Sent, Paid, Overdue
- Purchase-order statuses: Draft, Ordered, Received, Cancelled

## Database

SQL Server stores six tables.

| Table | Purpose |
| --- | --- |
| Customers | Name, email, phone, city, and state |
| Invoices | Number, customer, issue date, due date, and status |
| InvoiceLines | Description, quantity, and unit price for an invoice |
| Vendors | Name, email, phone, city, and category |
| PurchaseOrders | Number, vendor, order date, expected date, and status |
| PurchaseOrderLines | Description, quantity, and unit price for a purchase order |

Invoice numbers and purchase-order numbers are unique. Deleting an invoice or purchase order also deletes its lines. Money and quantity columns use `decimal(18, 2)`.

## Screenshots

These pictures were taken from the running application.

![Dashboard](docs/screenshots/dashboard.png)

![Customers](docs/screenshots/customers.png)

![Invoices](docs/screenshots/invoices.png)

![Invoice detail](docs/screenshots/invoice-detail.png)

![Vendors](docs/screenshots/vendors.png)

![Purchase orders](docs/screenshots/purchase-orders.png)

## Setup

Install the .NET 10 SDK and Docker.

The SQL Server container is an x64 image. On Apple Silicon, start it with Rosetta emulation. Docker Desktop can do that when Rosetta is enabled for x64 containers. With Colima:

```bash
softwareupdate --install-rosetta --agree-to-license
colima start --profile weberp --cpu 2 --memory 4 --disk 20 --vm-type vz --vz-rosetta
docker context use colima-weberp
```

From this folder, start the database:

```bash
docker-compose up -d
docker logs weberp-sql
```

Wait until the log says `SQL Server is now ready for client connections`.

The local database login is `sa` / `WebErp_Local1!`. That password is only for this sample database on your own machine. It is set in `docker-compose.yml` and `src/WebErp/appsettings.json`.

Start the site:

```bash
cd src/WebErp
dotnet run --project WebErp.csproj
```

Open [http://localhost:5088](http://localhost:5088).

The first start creates the database and inserts the sample rows. Later starts keep the data that is already there.

## Testing

This repository does not include an automated test project. The pages below were checked in the running application against the fictional sample data.

- Dashboard totals match the open invoices and open purchase orders.
- Customer, invoice, vendor, and purchase-order lists open and show the sample records.
- An invoice detail page shows its lines and the line total.
- Saving a new customer opens that customer's detail page.
- Saving a new invoice with a line stores the description, quantity, price, and total.

To repeat those checks, start SQL Server and the site, then open each page and save one customer and one invoice.
