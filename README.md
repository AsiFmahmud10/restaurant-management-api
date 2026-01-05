
# Shop api

An e-commerce backend API built using ASP.NET with EF Core, PostgreSQL, and Hangfire. Provides endpoints for both **Admin** and **Customer** operations.

- Designed and developed a modular **three-layer architecture** (Controller–Service–Repository) ensuring clean separation of concerns and maintainability.
- Implemented **authentication and role-based authorization with JWT tokens** for secure access control.
- Developed a Unit of Work pattern with **generic repository** to reduce redundant EF Core queries.Enhanced API performance using **pagination**.
- Built **custom middleware for transaction handling**, ensuring database consistency across API requests.
- Created password reset and **forget password functionality with email verification using SMTP**, improving account security and used **Hangfire background jobs to send emails asynchronously**, resulting in faster API responses. 
- Managed **database migrations and schema versioning** with Entity Framework Core.
Add **unit tests using xUnit and Moq**. Add swagger for api documentation
Improved codebase readability by **extracting service registrations into extension methods**, keeping startup file clean and well-structured.


`ASP.net` `EF Core` `Hangfire` `POSTgresql` `Moq` `xunit` `Swagger`



## Auth
- Uses JSON Web Tokens to authenticate users and secure API requests.
- Restricts access to endpoints based on user roles

### Admin
- Seed Super Admin credentials
- Admin can add,view products by category
- Admin can add,view,delete categories,
- Update order status: `Paid` / `Shipped` / `Completed`.
- View orders by status: `Confirmed`, `Paid`, `Shipped`, `Completed`.

### Customer
- Register, Login, Change Password, Reset Password with Email Verification.
- View products by category
- Manage shopping cart:
  - Add products to cart.
  - Update cart item quantity.
  - Clear cart.
- Place orders as a guest or logged-in user.
- Track order status: `Confirmed`, `Paid`, `Shipped`, `Completed`.

## Order-Payment Process
In case of payment, customer give transaction id when confirming order. After validating, admin will add tracking url and update order status. Using this tracking url custoemr can track the order. The order status will be updated to Completed after the order is delivered to the customer.    

## Setup & Run

### Prerequisites
- .NET SDK
- PostgreSQL
- SMTP email account (for password reset emails) # if missing email will not be sent 

---

### Steps to Run the Project

1. **Clone the repository**
```bash
git clone <repository-url>
cd shop-api
```
2. **Configure application settings**
```bash
Update appsettings.Development.json
```
3. ***Apply database migrations***
```bash
dotnet ef database update
```
This will create the database schema and seed the Super Admin credentials.

4. ***Run the application***


```bash 
cd ProductManagement
dotnet run
```


5. ***Access Swagger API documentation***

Open your browser and navigate to:
```bash
https://localhost:[port]/swagger/index.html
```
6. ***Running Unit Tests***

```bash
cd ../ 
dotnet test
```


Runs unit tests implemented using xUnit and Moq.
