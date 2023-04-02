#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["services/admin/NuGet.Config", "services/admin/"]
COPY ["services/auth/NuGet.Config", "services/auth/"]
COPY ["services/saas/NuGet.Config", "services/saas/"]
COPY ["services/admin/host/Bonnie.AdminService.HttpApi.Host/Bonnie.AdminService.HttpApi.Host.csproj", "services/admin/host/Bonnie.AdminService.HttpApi.Host/"]
COPY ["shared/Bonnie.Hosting.Shared/Bonnie.Hosting.Shared.csproj", "shared/Bonnie.Hosting.Shared/"]
COPY ["services/auth/src/Bonnie.AuthService.Application.Contracts/Bonnie.AuthService.Application.Contracts.csproj", "services/auth/src/Bonnie.AuthService.Application.Contracts/"]
COPY ["services/auth/src/Bonnie.AuthService.Domain.Shared/Bonnie.AuthService.Domain.Shared.csproj", "services/auth/src/Bonnie.AuthService.Domain.Shared/"]
COPY ["services/auth/src/Bonnie.AuthService.EntityFrameworkCore/Bonnie.AuthService.EntityFrameworkCore.csproj", "services/auth/src/Bonnie.AuthService.EntityFrameworkCore/"]
COPY ["services/auth/src/Bonnie.AuthService.Domain/Bonnie.AuthService.Domain.csproj", "services/auth/src/Bonnie.AuthService.Domain/"]
COPY ["services/saas/src/Bonnie.SaaSService.Application.Contracts/Bonnie.SaaSService.Application.Contracts.csproj", "services/saas/src/Bonnie.SaaSService.Application.Contracts/"]
COPY ["services/saas/src/Bonnie.SaaSService.Domain.Shared/Bonnie.SaaSService.Domain.Shared.csproj", "services/saas/src/Bonnie.SaaSService.Domain.Shared/"]
COPY ["services/admin/src/Bonnie.AdminService.HttpApi/Bonnie.AdminService.HttpApi.csproj", "services/admin/src/Bonnie.AdminService.HttpApi/"]
COPY ["services/admin/src/Bonnie.AdminService.Application.Contracts/Bonnie.AdminService.Application.Contracts.csproj", "services/admin/src/Bonnie.AdminService.Application.Contracts/"]
COPY ["services/admin/src/Bonnie.AdminService.Domain.Shared/Bonnie.AdminService.Domain.Shared.csproj", "services/admin/src/Bonnie.AdminService.Domain.Shared/"]
COPY ["services/admin/src/Bonnie.AdminService.Application/Bonnie.AdminService.Application.csproj", "services/admin/src/Bonnie.AdminService.Application/"]
COPY ["services/admin/src/Bonnie.AdminService.Domain/Bonnie.AdminService.Domain.csproj", "services/admin/src/Bonnie.AdminService.Domain/"]
COPY ["services/admin/src/Bonnie.AdminService.EntityFrameworkCore/Bonnie.AdminService.EntityFrameworkCore.csproj", "services/admin/src/Bonnie.AdminService.EntityFrameworkCore/"]
RUN dotnet restore "services/admin/host/Bonnie.AdminService.HttpApi.Host/Bonnie.AdminService.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/admin/host/Bonnie.AdminService.HttpApi.Host"
RUN dotnet build "Bonnie.AdminService.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bonnie.AdminService.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bonnie.AdminService.HttpApi.Host.dll"]
