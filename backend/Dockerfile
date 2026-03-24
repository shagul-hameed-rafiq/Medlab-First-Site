# Use the official .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY *.sln ./
COPY MedLabAInsights.Api/MedLabAInsights.Api.csproj MedLabAInsights.Api/
COPY MedLabAInsights.Data/MedLabAInsights.Data.csproj MedLabAInsights.Data/
COPY MedLabAInsights.DataGenerator/MedLabAInsights.DataGenerator.csproj MedLabAInsights.DataGenerator/
COPY MedLabAInsights.Models/MedLabAInsights.Models.csproj MedLabAInsights.Models/

RUN dotnet restore

# Copy all the remaining source code and build the project
COPY . ./
RUN dotnet publish MedLabAInsights.Api/MedLabAInsights.Api.csproj -c Release -o /app/out

# Use the official .NET ASP.NET 8.0 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose port 8080 (Render's default for web services starting from recent versions, but Render can detect the port)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# For SQLite to persist on Render, you may need a path for a mounted disk, e.g. /data
# If using EF Core SQLite, ensure your connection string points to this persistent folder
# Example: "Data Source=/data/MedlabAInsight.db"

ENTRYPOINT ["dotnet", "MedLabAInsights.Api.dll"]
