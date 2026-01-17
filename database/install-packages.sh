#!/bin/bash

# Script to install required NuGet packages for PostgreSQL support

cd "$(dirname "$0")/../JwtBackendApi"

echo "Installing Entity Framework Core packages..."

# Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0

# PostgreSQL provider
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0

# Entity Framework Core Tools (for migrations)
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

# Entity Framework Core Design (for migrations)
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0

echo "Packages installed successfully!"
echo ""
echo "To create and apply migrations, run:"
echo "  dotnet ef migrations add InitialCreate"
echo "  dotnet ef database update"
echo ""
echo "Or use the SQL script:"
echo "  psql -U postgres -d jwtbackendapi -f ../database/init.sql"
