# README

## Prerequisites
1. .NET 6 SDK
2. EF Core Tool
3. Postgres (Tested on 14, other version not tested)

## Setup and run
0. For DB setup, replace the connection strings in `appsettings.json`, then execute `dotnet ef migrations add init1 -c LotteryContext` and `dotnet ef database update` to create the db
1. To run, execute `dotnet run`
2. Endpoints methods are located at `TicketController.cs`, it is mostly self explanatory.

## Discussion
1. I am not using testing framework here (not so familiar with all of those stuff).
2. Limitations including ±1-2 seconds for db operations before it really times the next draw, no authentication and authorization have been applied.
3. This program could be deployed in docker instance if needed. Current setup should not require superuser privilege.