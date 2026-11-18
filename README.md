# NWSDB Billing & Payment System 🐳

A full-stack web system for a hypothetical Water Supply and Drainage Board that lets customers and third-party partners view bills, daily water usage and payment history and pay bills online, while staff generate bills and manage partners. 
Built with C# using .NET framework, Entity Framework Core ORM, SQL Server and a vanilla HTML, CSS, JavaScript frontend.

## Requirements
NWSDB wished to reduce operating costs and improve customer service by:
* Allowing third-party partners and customers to make online bill payments
* Providing updated usage and payment details
* Do all these despite organizational boundaries and varying languages/technologies

## Functional Overview
**User Management**
* Customer self-registration and secure login functionality
* Partner login using an auto-generated password sent via email
* Staff login with authorized access to system management features
* Role-based access control for customers, partners, and staff

**Account & Information Management**
* Customers can view bills, usage details, and payment history for accounts they own
* Partners can view bills for all their active accounts
* Customers and partners can view account addresses after successful NIC verification
* Staff can search and view complete details of any account
* Staff can browse paginated lists of customers, partners, and connections

**Bill & Payment**
* Customers and partners can make bill payments through the system
* Customers receive an email receipt upon successful payment
* Partners can view their own payment history across all accounts
* Staff generate bills for customer accounts based on meter readings

**Partner Management**
* Staff can create new partner accounts
* Staff can activate or deactivate partner accounts
* Partners receive auto-generated login credentials via email

## Architecture & Tech Stack
Since NWSDB had these requirements, service-oriented architecture was most ideal:
- Loosely-coupled services keep platform maintainable at any scale because changes to individual service can easily be made without much modifications to others.
- Better scalability as individual services can be scaled as per demand, ideally with horizontal scaling. E.g.: if payment processing is needed more, only that is given additional resources.

Clean REST APIs were implemented, using SOA, delegating one responsibility per layer across the functions: Models, DTOs, Data, Services, Controllers

### Technologies
* Programming Languages: C#
* Framework: ASP.NET
* Front-end Technologies: HTML, CSS, JavaScript
* Database: MSSQL Server, Entity Frame Core [ORM]

## Security Mechanisms
* BCrypt password hashing,
* For deactivated partner accounts, password is checked before account status so a deactivated account isn’t revealed to someone guessing,
* NIC check before addresses are shown,
* Staff-created partner passwords are generated cryptographically and emailed

