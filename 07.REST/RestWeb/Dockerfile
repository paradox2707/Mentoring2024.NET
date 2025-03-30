# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN echo "Debugging: Listing files in /src" && ls -al /src
WORKDIR /src
RUN dotnet build "RestWeb/RestWeb.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "RestWeb/RestWeb.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestWeb.dll"]