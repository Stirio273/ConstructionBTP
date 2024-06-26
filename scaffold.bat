dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet new install Microsoft.EntityFrameworkCore.Templates
dotnet new ef-templates
dotnet ef dbcontext scaffold "Server=localhost;Database=tsakitsaky;User Id=postgres;Password=gigabyte;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models