#!/bin/bash

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."

# Try to connect to SQL Server with retries
for i in {1..30}; do
    echo "Attempt $i: Checking if SQL Server is ready..."
    
    # Use a simple TCP connection test to check if SQL Server is accepting connections
    # -z flag checks if the port is open, without sending any data
    if timeout 5 bash -c "cat < /dev/null > /dev/tcp/sqlserver/1433"; then
        echo "SQL Server is ready!"
        break
    fi
    
    echo "SQL Server is not ready yet. Waiting 5 seconds..."
    sleep 5
    
    # If we've tried 30 times and still can't connect, just continue anyway
    if [ $i -eq 30 ]; then
        echo "Warning: Could not confirm SQL Server is ready after multiple attempts. Continuing anyway..."
    fi
done

# Run the application
echo "Starting the application..."
exec dotnet TaskManagementSystem.dll 