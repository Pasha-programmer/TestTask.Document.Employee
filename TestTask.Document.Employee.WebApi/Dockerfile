# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestTask.Document.Employee.WebApi/TestTask.Document.Employee.WebApi.csproj", "TestTask.Document.Employee.WebApi/"]
COPY ["TestTask.Document.Employee.Endpoints/TestTask.Document.Employee.Endpoints.csproj", "TestTask.Document.Employee.Endpoints/"]
COPY ["TestTask.Document.Employee.Common/TestTask.Document.Employee.Common.csproj", "TestTask.Document.Employee.Common/"]
COPY ["TestTask.Document.Employee.Contract/TestTask.Document.Employee.Contract.csproj", "TestTask.Document.Employee.Contract/"]
COPY ["TestTask.Document.Employee.Infrastructure/TestTask.Document.Employee.Infrastructure.csproj", "TestTask.Document.Employee.Infrastructure/"]
COPY ["TestTask.Document.Employee.Database/TestTask.Document.Employee.Database.csproj", "TestTask.Document.Employee.Database/"]
RUN dotnet restore "./TestTask.Document.Employee.WebApi/TestTask.Document.Employee.WebApi.csproj"
COPY . .
WORKDIR "/src/TestTask.Document.Employee.WebApi"
RUN dotnet build "./TestTask.Document.Employee.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}
RUN dotnet publish "./TestTask.Document.Employee.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
ARG ASPNETCORE_ENVIRONMENT=Production
ARG ASPNETCORE_URLS="http://*:80"
ARG BUILD_CONFIGURATION=release
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_URLS=$ASPNETCORE_URLS
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestTask.Document.Employee.WebApi.dll"]