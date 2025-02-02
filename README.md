
# TruVista & Praxedo Integration

## <ins>Overview</ins>
  This project provides basic data integration between **TruVista’s CHR_Omnia360 ERP/CRM** application and **Praxedo’s WorkOrder and Dispatch application**. The solution is an **OnPremise .NET Application** that extracts, transforms, and loads (ETL) data between the two systems, ensuring accurate synchronization.

### Solution Architecture
![Solution Architecture](images/architecture-diagram.jpg)  
*This diagram outlines the major components of the system integration.*

### Wireframes (Draft)
![Wireframe Example](images/wireframe-home.jpg)  
*Preliminary wireframe sketch of the application UI workflow.*

###  <ins>User Stories</ins>
1. **As a system administrator**, I want to configure integration settings so that I can define API authentication securely.
2. **As an operations manager**, I need automated synchronization of work orders between CHR and Praxedo so that I don't have to enter data manually.
3. **As a customer service representative**, I want account and contact information synchronized in real time so that I have accurate records.
4. **As a technician**, I need work orders in Praxedo to update back in CHR after completion so that my progress is tracked correctly.
5. **As an IT engineer**, I need logging and error-handling mechanisms so that I can troubleshoot any synchronization issues quickly.

##  <ins>Use Cases</ins>
### **Use Case 1: Synchronizing Accounts/Contacts**
- **Actor:** System Administrator
- **Precondition:** API credentials are configured
- **Steps:**
  1. The scheduled task runs at a defined interval.
  2. The integration extracts account and contact data from CHR_Omnia360.
  3. The data is transformed into Praxedo’s expected format.
  4. The data is sent to Praxedo via API.
  5. Logging captures success or failure.
- **Postcondition:** Accounts and contacts are updated in Praxedo.

### **Use Case 2: Work Order Sync**
- **Actor:** Field Technician
- **Steps:**
  1. A work order is updated in Praxedo.
  2. The scheduled task detects the update.
  3. The integration retrieves the updated work order.
  4. The transformed data is sent back to CHR.
  5. The system logs the sync result.

(Include 3 more use cases here)

## Use Case Diagram
![Use Case Diagram](images/use-case-diagram.jpg)  
*This diagram visualizes the actors and system interactions.*

## Requirements
- **Programming Language:** VB.NET
- **Database:** SQL Server
- **API Integration:** REST/SOAP (depending on Praxedo’s API)
- **Authentication:** Secure API token-based authentication
- **Logging & Error Handling:** Event logs, retry mechanisms

## Repository Contents
- `README.md` (This file)
- `docs/` (Design documents, including user stories, use cases, and architecture diagrams)
- `images/` (Diagrams for architecture, wireframes, and use case models)
- `src/` (Project source code)
- `logs/` (Error and execution logs)

---

## **Next Steps**
- [ ] Finalize wireframe sketches using **Draw.io** or **Pencil Project**.
- [ ] Create architecture and use-case diagrams.
- [ ] Populate repository with initial `.NET` project structure.
- [ ] Set up API credentials and data mapping in CHR and Praxedo.

---

We are off!🚀
