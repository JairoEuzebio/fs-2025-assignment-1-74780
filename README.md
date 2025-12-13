Dublin Bikes – Web Application

In this project, I built a simple web application using ASP.NET Minimal API and Blazor Web App. The application displays Dublin Bikes stations and allows users to view station details, apply filters, and perform basic CRUD operations such as creating, editing, and deleting stations. I implemented two API versions: v1, using in-memory data, and v2, using Azure Cosmos DB, with the Blazor application consuming the v2 API.

I used the Azure Cosmos DB Emulator for data persistence and implemented a background service that periodically updated bike availability to simulate real-time data. The frontend includes filtering, sorting, pagination, station detail view, and occupancy visualization. Through this project, I practiced working with REST APIs, dependency injection, background services, and state management in Blazor, keeping a clear separation between backend and frontend responsibilities.

