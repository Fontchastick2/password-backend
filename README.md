# Passman Backend: C# .NET API for Password Management

This repository contains the backend for the **Passman** password management tool. Built with C# and ASP.NET Core, it provides secure endpoints for password storage, retrieval, and management, while ensuring data is encrypted when stored.

## Installation

### Prerequisites:
- .NET 6 or higher
- Visual Studio or another C# development environment

### Steps to Run the Backend:

1. Clone the repository.
2. Navigate to the `backend` directory.
3. Install dependencies and build the project:
   ```bash
   dotnet restore
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
   The server will be available at `http://localhost:5051`.

## API Endpoints

### 1. **POST /api/passwords**
   Add a new password to the store. The request body should include:
   ```json
   {
     "category": "Work",
     "app": "Zoom",
     "userName": "user@example.com",
     "password": "password123"
   }
   ```
   - Encrypts the password before saving it.

### 2. **GET /api/passwords**
   Retrieve all stored passwords in the system.

### 3. **GET /api/passwords/{id:int}**
   Retrieve a password by its ID.

### 4. **GET /api/passwords/{id:int}/decrypted**
   Retrieve a decrypted password by its ID.

### 5. **PUT /api/passwords/{id:int}**
   Update an existing password item by ID. The request body should include the updated fields:
   ```json
   {
     "category": "Updated Category",
     "app": "Updated App",
     "userName": "updateduser@example.com",
     "password": "newpassword123"
   }
   ```

### 6. **DELETE /api/passwords/{id:int}**
   Delete a password by its ID.

### 7. **POST /api/verify**
   Verify if the entered passkey matches the expected value (`jean@groupm`).

## File Storage

Password data is saved in a local file `passwordStore.json`. Ensure this file exists in the root of the project. If it doesn't exist, it will be automatically created.

## Technologies Used

- **ASP.NET Core** (C#)
- **System.Text.Json** for serialization and deserialization
- **Base64** encryption for storing passwords securely

## Security Considerations

- Passwords are encrypted using Base64 encoding before storage to ensure they are not saved in plain text.
