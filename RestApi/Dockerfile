# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything and restore as distinct layers
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Build a runtime image using the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set environment variables and expose the port
ENV ASPNETCORE_URLS=http://*:6000
EXPOSE 6000

# Set the entry point for the container
ENTRYPOINT ["dotnet", "MyRestApi.dll"]
