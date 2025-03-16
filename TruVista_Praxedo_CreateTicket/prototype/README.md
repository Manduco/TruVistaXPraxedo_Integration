# TruVista Praxedo Ticket Creation Prototype

## Overview
This prototype is a **C# console application** designed to facilitate the creation and submission of work order tickets from **TruVista** to **Praxedo**. The program initializes configurations, retrieves or generates work order data, constructs a Praxedo-compatible ticket, and sends the request.

## Functionality

### 1. **Configuration Setup**
- Loads runtime configurations via the `RunTimeConfigs` class.
- Supports **debug mode**, **fake data generation**, and **Praxedo evaluation environment** settings.
- If `use_fakedata` is enabled, it generates **mock work orders** for testing.

### 2. **Data Handling**
- Uses the `Data` class to store work order details like **customer name, email, and work order number**.
- If real data is required, `GetTickets()` fetches work orders from the database.

### 3. **Ticket Creation**
- The `BuildTicket()` function constructs a **businessEvent** object with:
  - **Customer contact details**
  - **Work order description and priority**
  - **Qualification and contract data**
- Contacts are processed through `Build_Contacts()`, formatting phone numbers and emails.

### 4. **Sending the Request**
- The generated ticket is sent to **Praxedo** via `SendReqest_ToPraxedo()`.
- It authenticates with **Praxedo's API** using credentials stored in the configuration.
- The response from Praxedo determines if the ticket was **created successfully** or **encountered errors** (e.g., duplicates or missing data).

## Prototype Expectations
- This is a **prototype**, meaning **not all functionalities are complete**.
- The `prototype/` folder will contain:
  - Mockup screenshots
  - Sample work order data
  - Example output logs

### Example Output (Debug Mode)
```plaintext
Fake Name: John Doe
Fake Email: johndoe@example.com
Fake Work Order: WO12345
Ticket data prepared
Sending request to Praxedo...
Update successful in Praxedo.
```

## Future Enhancements
- **Database Integration:** Complete the `GetTickets()` function to pull real work orders.
- **Error Handling:** Improve response handling for failed API requests.
- **UI Improvements:** Possibly transition from a console app to a web or desktop interface.

## Repository Structure
```
/
├── prototype/            # Folder for prototype elements (mockups, test data, etc.)
│   ├── mockups/         # Images or sketches outlining UI
│   ├── sample_data/     # Example work order data
│   ├── logs/            # Sample output logs
│   ├── prototype.html   # HTML-based visualization (if applicable)
├── src/                 # C# source code files
├── README.md            # Project description (this file)
├── .gitignore           # Git ignore file
└── LICENSE              # License information (if applicable)
```


##  YouTube Prototype Demo
