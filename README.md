# ToDoListAPI

## Overview

The ToDoList application is a simple task management system that allows users to create, read, update, and delete to-do items. It uses ASP.NET Core for the backend and Entity Framework Core for data access.

### Things that went well
I feel pleased with the layout of the project in that it follows a Domain Driven Design for its layers. Also I am pleased with the tests although maybe some more true end-to-end tests wouldnt go amiss. As mentioned in the front-end readme this API is very naive in that there is light security and no concept of Users and thier associated accounts. Again some sort of integration with Okta or Entra would be my choice to have some sort of token and claims based authentication to vlaidate users. 

### Things to improve
The logging on this project is deliberately poor, I took the approach of allowing errors to bubble up to the controller layer and then logging them to the console and sending back a generic 500 http response. In real life I would be looking at some centralised logging like Sentry or Logrocket, also more nuanced http returns could be used. Plugging in Application Insights would be a good shout as well. The sub project layers are also harder to reuse as a result as they do not perform any logging or error handling, so any application reusing them out of context would have to account for that. 

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
```
git clone https://github.com/yourusername/ToDoList.git
cd ToDoList
```
   
2. Update the database connection string in `appsettings.json`:
```
"ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=ToDoListDb;User Id=your_user;Password=your_password;"
}
```
3. Apply migrations and create the database:
```
dotnet ef database update
```

5. Run the application:
```
dotnet run --project ToDoList.API
```
   
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

