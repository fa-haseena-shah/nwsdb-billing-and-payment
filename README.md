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

## Screenshots
<img width="666" height="397" alt="Screenshot 2026-11-18 at 21 47 13" src="https://github.com/user-attachments/assets/dc22e148-ac02-4645-9be5-d0b17b42602c" />
<img width="669" height="395" alt="Screenshot 2026-11-18 at 21 47 03" src="https://github.com/user-attachments/assets/7e1e3776-5b50-42dc-82c1-d0cb6b18ea88" />
<img width="672" height="429" alt="Screenshot 2026-11-18 at 21 46 17" src="https://github.com/user-attachments/assets/55ed0af4-8720-4b0c-a340-cec26a733c43" />
<img width="669" height="394" alt="Screenshot 2026-11-18 at 21 46 26" src="https://github.com/user-attachments/assets/344bf91a-1d1e-4449-9abf-83d19d077af1" />
<img width="666" height="393" alt="Screenshot 2026-11-18 at 21 46 34" src="https://github.com/user-attachments/assets/e69c670c-9988-422e-b225-ed262a137985" />
<img width="667" height="394" alt="Screenshot 2026-11-18 at 21 46 44" src="https://github.com/user-attachments/assets/3f36af9e-12a2-4be0-9770-be201806323b" />
<img width="672" height="396" alt="Screenshot 2026-11-18 at 21 46 54" src="https://github.com/user-attachments/assets/1ccb8696-810c-4f38-a86a-cf1e1914baa6" />
<img width="673" height="424" alt="Screenshot 2026-11-18 at 21 46 05" src="https://github.com/user-attachments/assets/0c830c5b-5ad6-428d-a91b-c83418ac28fc" />
<img width="667" height="393" alt="Screenshot 2026-11-18 at 21 45 56" src="https://github.com/user-attachments/assets/88061535-3fe4-494e-9b20-5b1fc19c3e61" />
<img width="667" height="393" alt="Screenshot 2026-11-18 at 21 45 48" src="https://github.com/user-attachments/assets/15841a24-e959-41f4-ae7e-6e1df0d3b828" />
<img width="670" height="394" alt="Screenshot 2026-11-18 at 21 45 33" src="https://github.com/user-attachments/assets/0fd065a3-0621-4fd7-803f-bc1bbbfaddf3" />
<img width="670" height="394" alt="Screenshot 2026-11-18 at 21 45 21" src="https://github.com/user-attachments/assets/f03fb5e5-f20c-49f7-a98f-a648e9b3dd0d" />

###### * It is worth noting that I did not put much effort into the user interface designs since learning this new architecture and developing the backend was my priority with this project.


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

### Contact
[Email](mailto:fa.haseenashah@gmail.com) ⋆ [LinkedIn](https://www.linkedin.com/in/haseena-shah-1a0a11444) ⋆ [Design](https://www.behance.net/haseenashah2d)
