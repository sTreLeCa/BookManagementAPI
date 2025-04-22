# Book Management API (.NET 9 & MongoDB)

This project implements a robust **RESTful API** for managing book information, built on the **ASP.NET Core** framework (`.NET 9`) and utilizing **MongoDB** as the data store. It demonstrates common API practices including dependency injection, configuration management, and a layered architecture (Controller-Service-Repository).

## Key Features

*   **Full CRUD Operations:** Create, retrieve, update, and delete book records via standard HTTP methods (POST, GET, PUT, DELETE).
*   **MongoDB Integration:** Leverages the official `MongoDB.Driver` C# driver for efficient and type-safe database interactions.
*   **Pagination:** Retrieves books in manageable pages (`GET /api/book?page=1&pageSize=10`), sorted by view count to show popular books first.
*   **Soft Deletes:** Implements soft delete logic (`DELETE /api/book/{id}`). Records are marked as `IsDeleted` and retain a `DeletedDate` rather than being physically removed, preserving data history.
*   **Bulk Operations:** Supports creating (`POST /api/book/many`) and soft-deleting (`DELETE /api/book` with list of IDs) multiple books in a single request for efficiency.
*   **View Count Tracking:** Automatically increments a `ViewsCount` field when a specific book's details are fetched (`GET /api/book/{id}`).
*   **Popularity Score Calculation:** Includes logic within the `Books` model to calculate a basic `PopularityScore` based on view count and publication year.
*   **Duplicate Handling:** The create operation checks for existing non-deleted titles. If a book with the same title exists but is soft-deleted, it reactivates and updates the existing record instead of creating a new one.
*   **Swagger/OpenAPI Documentation:** Integrated Swashbuckle (`Swashbuckle.AspNetCore`) to automatically generate interactive API documentation. Accessible via the `/swagger` endpoint for easy testing and exploration.
*   **JWT Authentication:** Configured JWT Bearer authentication scheme (`Microsoft.AspNetCore.Authentication.JwtBearer`). Requires `appsettings.json` configuration for `Jwt:SecretKey`, `Jwt:Issuer`, and `Jwt:Audience` to function. (Note: Actual token generation/login endpoint is not part of this core CRUD API).
*   **Layered Architecture:** Clearly separates concerns:
    *   `Controllers`: Handle HTTP requests and responses.
    *   `Services`: Contain business logic and orchestrate operations.
    *   `Repositories`: Abstract data access logic, interacting directly with MongoDB.
    *   `Models`: Define data structures (e.g., `Books`).
    *   `Interfaces`: Define contracts for services and repositories, enabling dependency injection and testability.

## Technology Stack

*   **Framework:** .NET 9 / ASP.NET Core
*   **Database:** MongoDB (using `MongoDB.Driver`)
*   **API Documentation:** Swashbuckle.AspNetCore (Swagger/OpenAPI)
*   **Authentication:** JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)
