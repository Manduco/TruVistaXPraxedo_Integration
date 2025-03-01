## Use Cases

### 1. **Use Case: Synchronizing Accounts & Contacts**
**Scenario:** Syncing account and contact data from CHR_Omnia360 to Praxedo.  
**Description:** The system extracts account and contact details from CHR and pushes them to Praxedo, ensuring both systems contain the latest customer records.  
**Actors:** System Administrator, CHR API, Praxedo API  
**Steps:**
1. Scheduled task retrieves account/contact data from CHR.
2. Data is formatted and validated.
3. The system checks for existing records in Praxedo.
4. New records are created, and updates are applied.
5. Sync results are logged (success or failure).
**Expected Result:** Accounts and contacts are updated in Praxedo, ensuring consistency between the two systems.

---

### 2. **Use Case: Work Order Synchronization (CHR → Praxedo)**
**Scenario:** Sending work orders from CHR to Praxedo.  
**Description:** Work orders created in CHR should be automatically pushed to Praxedo for field service management.  
**Actors:** Operations Manager, CHR API, Praxedo API  
**Steps:**
1. The system detects new or updated work orders in CHR.
2. Data is transformed to match Praxedo's format.
3. The system sends the work order to Praxedo via API.
4. API response is validated and logged.
5. If an error occurs, the work order is marked for retry.
**Expected Result:** Work orders are successfully created in Praxedo without manual input.

---

### 3. **Use Case: Work Order Update (Praxedo → CHR)**
**Scenario:** Updating CHR when a work order is completed in Praxedo.  
**Description:** When a technician completes a work order in Praxedo, the status must be reflected in CHR.  
**Actors:** Field Technician, Praxedo API, CHR API  
**Steps:**
1. The system retrieves completed work orders from Praxedo.
2. Data is checked against CHR to find matching records.
3. Work order status is updated in CHR.
4. The system logs success or failure.
**Expected Result:** Work order statuses in CHR are updated based on technician activity in Praxedo.

---

### 4. **Use Case: Error Logging & Retry Mechanism**
**Scenario:** Handling failures during synchronization.  
**Description:** If a sync operation fails, the system logs the error and retries automatically.  
**Actors:** DevOps Engineer, Logging System, .NET Middleware  
**Steps:**
1. The sync process runs a scheduled task.
2. If an error occurs, the system logs the failure.
3. The failed sync item is added to the retry queue.
4. The system retries the sync at the next scheduled interval.
5. If retries fail multiple times, an alert is triggered.
**Expected Result:** Errors are logged, and failed operations are retried without manual intervention.

---

### 5. **Use Case: Viewing Sync Logs**
**Scenario:** Reviewing data synchronization history.  
**Description:** Administrators need visibility into sync operations, including success rates and failure details.  
**Actors:** System Administrator, Logging System  
**Steps:**
1. The administrator accesses the sync log database.
2. The system displays recent sync attempts with timestamps.
3. Filters allow searching by status (Success, Failed, Pending).
4. Detailed error messages are available for failed attempts.
**Expected Result:** Administrators can review past sync operations and troubleshoot issues effectively.
