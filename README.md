# ToDoListAPI

## Overview

The ToDoList application is a simple task management system that allows users to create, read, update, and delete to-do items. It uses ASP.NET Core for the backend and Entity Framework Core for data access.

## Features

- Add new to-do items
- View all to-do items
- Update existing to-do items
- Delete to-do items
- Caching for improved performance

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- Microsoft.Extensions.Caching.Memory
- SQL Server (or any other supported database)
- Dependency Injection

## Project Structure
```
ToDoList 
├── ToDoList.Domain 
│   ├── Interfaces 
│   │   └── IToDoRepository.cs 
│   ├── Models 
│   │   └── ToDoItem.cs 
├── ToDoList.Application 
│   ├── Services 
│   │   └── ToDoService.cs 
├── ToDoList.Infrastructure 
│   ├── Repositories 
│   │   └── ToDoRepository.cs 
│   ├── Data 
│   │   └── AppDbContext.cs 
├── ToDoList.API 
│   ├── Controllers 
│   │   └── ToDoController.cs 
│   ├── Program.cs 
│   ├── Startup.cs
```
## Getting Started

### Prerequisites

- .NET 6 SDK or later
- SQL Server (or any other supported database)

### Setup

1. Clone the repository:
    git clone https://github.com/yourusername/ToDoList.git
    cd ToDoList

   
2. Update the database connection string in `appsettings.json`:
    "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=ToDoListDb;User Id=your_user;Password=your_password;"
}

3. Apply migrations and create the database:
    dotnet ef database update

4. Run the application:
   dotnet run --project ToDoList.API

   
### Usage

- The API will be available at `https://localhost:5001` or `http://localhost:5000`.
- Use tools like Postman or Swagger to interact with the API endpoints.

## API Endpoints

- `GET /api/todo` - Get all to-do items
- `GET /api/todo/{id}` - Get a to-do item by ID
- `POST /api/todo` - Add a new to-do item
- `PUT /api/todo/{id}` - Update an existing to-do item
- `DELETE /api/todo/{id}` - Delete a to-do item by ID

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
