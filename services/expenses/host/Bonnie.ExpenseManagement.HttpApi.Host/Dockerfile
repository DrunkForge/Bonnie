#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["services/expenses/NuGet.Config", "services/expenses/"]
COPY ["services/expenses/host/Bonnie.ExpenseManagement.HttpApi.Host/Bonnie.ExpenseManagement.HttpApi.Host.csproj", "services/expenses/host/Bonnie.ExpenseManagement.HttpApi.Host/"]
COPY ["services/expenses/src/Bonnie.ExpenseManagement.HttpApi/Bonnie.ExpenseManagement.HttpApi.csproj", "services/expenses/src/Bonnie.ExpenseManagement.HttpApi/"]
COPY ["services/expenses/src/Bonnie.ExpenseManagement.Application.Contracts/Bonnie.ExpenseManagement.Application.Contracts.csproj", "services/expenses/src/Bonnie.ExpenseManagement.Application.Contracts/"]
COPY ["services/expenses/src/Bonnie.ExpenseManagement.Domain.Shared/Bonnie.ExpenseManagement.Domain.Shared.csproj", "services/expenses/src/Bonnie.ExpenseManagement.Domain.Shared/"]
COPY ["services/expenses/src/Bonnie.ExpenseManagement.Application/Bonnie.ExpenseManagement.Application.csproj", "services/expenses/src/Bonnie.ExpenseManagement.Application/"]
COPY ["services/expenses/src/Bonnie.ExpenseManagement.Domain/Bonnie.ExpenseManagement.Domain.csproj", "services/expenses/src/Bonnie.ExpenseManagement.Domain/"]
COPY ["services/expenses/src/Bonnie.ExpenseManagement.EntityFrameworkCore/Bonnie.ExpenseManagement.EntityFrameworkCore.csproj", "services/expenses/src/Bonnie.ExpenseManagement.EntityFrameworkCore/"]
RUN dotnet restore "services/expenses/host/Bonnie.ExpenseManagement.HttpApi.Host/Bonnie.ExpenseManagement.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/expenses/host/Bonnie.ExpenseManagement.HttpApi.Host"
RUN dotnet build "Bonnie.ExpenseManagement.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bonnie.ExpenseManagement.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bonnie.ExpenseManagement.HttpApi.Host.dll"]
