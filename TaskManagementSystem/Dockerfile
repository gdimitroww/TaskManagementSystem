FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TaskManagementSystem.csproj", "./"]
RUN dotnet restore "./TaskManagementSystem.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TaskManagementSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagementSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"] 