#!/bin/bash

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
sleep 10

# Run migrations
echo "Running migrations..."
dotnet TaskManagementSystem.dll --migrate

# Start the application
echo "Starting the application..."
dotnet TaskManagementSystem.dll 