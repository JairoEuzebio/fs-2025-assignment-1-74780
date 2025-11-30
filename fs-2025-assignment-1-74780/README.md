DublinBikes API – Assignment 1 - Jairo Euzebio de Souza - 74780.

This project provides a simple Web API with two versions:

V1 – Reads data from a local JSON file

V2 – Reads data from Azure Cosmos DB Emulator

Both versions expose the same endpoints.

Run

dotnet restore
dotnet run --project fs-2025-assignment-1-74780

Base URL: https://localhost:7259

V1 (File)

Uses Data/dublinbike.json. No setup needed.

V2 (Cosmos DB)

Requires Azure Cosmos DB Emulator.
Create the following:

Database: DublinBikesDb
Container: Stations
Partition Key: /number

Import the flat JSON dataset into the Stations container.

Endpoints

GET /api/v1/stations
GET /api/v1/stations/{number}
GET /api/v1/stations/summary

GET /api/v2/stations
GET /api/v2/stations/{number}
GET /api/v2/stations/summary

Tests

Run all tests with:
dotnet test

You can also use the Postman collection included in the project.

Notes

A background service simulates real-time updates to bike availability.