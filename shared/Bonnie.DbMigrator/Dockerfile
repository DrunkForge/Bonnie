#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["services/admin/NuGet.Config", "services/admin/"]
COPY ["services/auth/NuGet.Config", "services/auth/"]
COPY ["services/saas/NuGet.Config", "services/saas/"]
COPY ["shared/Bonnie.DbMigrator/Bonnie.DbMigrator.csproj", "shared/Bonnie.DbMigrator/"]
COPY ["services/admin/src/Bonnie.AdminService.Application.Contracts/Bonnie.AdminService.Application.Contracts.csproj", "services/admin/src/Bonnie.AdminService.Application.Contracts/"]
COPY ["services/admin/src/Bonnie.AdminService.Domain.Shared/Bonnie.AdminService.Domain.Shared.csproj", "services/admin/src/Bonnie.AdminService.Domain.Shared/"]
COPY ["services/admin/src/Bonnie.AdminService.EntityFrameworkCore/Bonnie.AdminService.EntityFrameworkCore.csproj", "services/admin/src/Bonnie.AdminService.EntityFrameworkCore/"]
COPY ["services/admin/src/Bonnie.AdminService.Domain/Bonnie.AdminService.Domain.csproj", "services/admin/src/Bonnie.AdminService.Domain/"]
COPY ["services/auth/src/Bonnie.AuthService.Application.Contracts/Bonnie.AuthService.Application.Contracts.csproj", "services/auth/src/Bonnie.AuthService.Application.Contracts/"]
COPY ["services/auth/src/Bonnie.AuthService.Domain.Shared/Bonnie.AuthService.Domain.Shared.csproj", "services/auth/src/Bonnie.AuthService.Domain.Shared/"]
COPY ["services/auth/src/Bonnie.AuthService.EntityFrameworkCore/Bonnie.AuthService.EntityFrameworkCore.csproj", "services/auth/src/Bonnie.AuthService.EntityFrameworkCore/"]
COPY ["services/auth/src/Bonnie.AuthService.Domain/Bonnie.AuthService.Domain.csproj", "services/auth/src/Bonnie.AuthService.Domain/"]
COPY ["services/saas/src/Bonnie.SaaSService.Application.Contracts/Bonnie.SaaSService.Application.Contracts.csproj", "services/saas/src/Bonnie.SaaSService.Application.Contracts/"]
COPY ["services/saas/src/Bonnie.SaaSService.Domain.Shared/Bonnie.SaaSService.Domain.Shared.csproj", "services/saas/src/Bonnie.SaaSService.Domain.Shared/"]
COPY ["services/saas/src/Bonnie.SaaSService.EntityFrameworkCore/Bonnie.SaaSService.EntityFrameworkCore.csproj", "services/saas/src/Bonnie.SaaSService.EntityFrameworkCore/"]
COPY ["services/saas/src/Bonnie.SaaSService.Domain/Bonnie.SaaSService.Domain.csproj", "services/saas/src/Bonnie.SaaSService.Domain/"]
COPY ["shared/Bonnie.Microservice.Shared/Bonnie.Microservice.Shared.csproj", "shared/Bonnie.Microservice.Shared/"]
COPY ["shared/Bonnie.Hosting.Shared/Bonnie.Hosting.Shared.csproj", "shared/Bonnie.Hosting.Shared/"]
RUN dotnet restore "shared/Bonnie.DbMigrator/Bonnie.DbMigrator.csproj"
COPY . .
WORKDIR "/src/shared/Bonnie.DbMigrator"
RUN dotnet build "Bonnie.DbMigrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bonnie.DbMigrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bonnie.DbMigrator.dll"]
