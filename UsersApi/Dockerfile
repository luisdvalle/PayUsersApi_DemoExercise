#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-stretch AS build
WORKDIR /src
COPY ["UsersApi.csproj", ""]
RUN dotnet restore "./UsersApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "UsersApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UsersApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/app