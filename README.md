# 🍽️ Restaurant POS System

**PROG7311 – Enterprise Systems Project**

---

## 📌 Project Overview

The **Restaurant POS System** is an enterprise-style web application designed to digitally manage key restaurant operations. The system provides a structured and role-based workflow for waiters, kitchen staff, and managers to improve order processing, kitchen communication, sales tracking, table management, and administrative monitoring.

The application follows a modular architecture, making it easier to maintain, scale, and manage. It also includes secure role-based access control so that each user can only access the sections of the system that are relevant to their role.



---

## 🎥 Demonstration Video

YouTube demonstration link:

https://www.youtube.com/watch?v=PPjNkf_bHOI 

---


## ✅ Main System Features

The Restaurant POS System includes the following core features:

- Order creation and management
- Sales, invoicing, and receipt generation
- Table booking and table allocation
- Kitchen order tracking dashboard
- Administrative dashboard for stock, staff, and sales monitoring
- Role-based login and user access control
- Payment processing for completed orders
- Restaurant table status tracking
- Reservation management
- Inventory and stock monitoring

---

## 👥 Development Team

| Name | Student Number | Role |
|------|---------------|------|
| Kaehil Indurjeeth | ST10438880 | Order Creation Module |
| Gregory Luyckfasseel | ST10441344 | Booking and Authentication |
| Zario Di Paolo | ST10441349 | Admin Dashboard |
| Kyra Naidoo | ST10448414 | Kitchen Dashboard |
| Diya Lakha | ST10439176 | Sales System |

---

## 🧩 Application Modules

### 1. Order Creation Module

The order creation module allows waiters to create customer orders, assign them to restaurant tables, select menu items, add quantities, and send the order to the kitchen for preparation.

### 2. Kitchen Dashboard

The kitchen dashboard allows kitchen staff to view active orders that have been submitted by waiters. Kitchen staff can track each order, update item progress, and mark orders as ready once preparation is complete.

### 3. Sales System

The sales system manages completed customer orders, payment processing, invoice generation, and receipt printing. Once an order is ready, the waiter can complete the sale and generate a receipt for the customer.

### 4. Booking and Authentication

This module handles user login, role-based access, and table reservations. It ensures that users are directed to the correct dashboard based on their assigned role.

### 5. Admin Dashboard

The admin dashboard allows managers to monitor restaurant activity. This includes viewing sales, managing stock, checking staff details, viewing orders, and managing reservations.

---

## 🔐 User Roles

The system includes three main user roles:

| Role | Description |
|------|-------------|
| Manager | Accesses the admin dashboard, stock, staff, sales, orders, and reservations |
| Waiter | Manages tables, creates orders, sends orders to the kitchen, and processes payments |
| Kitchen | Views incoming kitchen orders and updates order preparation status |

---

## 🔑 Default Login Details

The application seeds default users into the database when it runs.

| Role | Username | PIN |
|------|----------|-----|
| Manager | manager | 1234 |
| Waiter | waiter | 2345 |
| Kitchen | kitchen | 3456 |

---

## 🛠️ Technologies Used

- C#
- ASP.NET Core MVC
- .NET
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server LocalDB
- Razor Views
- Visual Studio
- GitHub

---

🗄️ Database Script

<img width="416" height="894" alt="image" src="https://github.com/user-attachments/assets/323b9364-87b2-41f2-aad9-88aa299131ba" />

