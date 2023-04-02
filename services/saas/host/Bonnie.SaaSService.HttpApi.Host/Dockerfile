#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["services/saas/NuGet.Config", "services/saas/"]
COPY ["services/admin/NuGet.Config", "services/admin/"]
COPY ["services/saas/host/Bonnie.SaaSService.HttpApi.Host/Bonnie.SaaSService.HttpApi.Host.csproj", "services/saas/host/Bonnie.SaaSService.HttpApi.Host/"]
COPY ["shared/Bonnie.Hosting.Shared/Bonnie.Hosting.Shared.csproj", "shared/Bonnie.Hosting.Shared/"]
COPY ["services/admin/src/Bonnie.AdminService.EntityFrameworkCore/Bonnie.AdminService.EntityFrameworkCore.csproj", "services/admin/src/Bonnie.AdminService.EntityFrameworkCore/"]
COPY ["services/admin/src/Bonnie.AdminService.Domain/Bonnie.AdminService.Domain.csproj", "services/admin/src/Bonnie.AdminService.Domain/"]
COPY ["services/admin/src/Bonnie.AdminService.Domain.Shared/Bonnie.AdminService.Domain.Shared.csproj", "services/admin/src/Bonnie.AdminService.Domain.Shared/"]
COPY ["services/saas/src/Bonnie.SaaSService.HttpApi/Bonnie.SaaSService.HttpApi.csproj", "services/saas/src/Bonnie.SaaSService.HttpApi/"]
COPY ["services/saas/src/Bonnie.SaaSService.Application.Contracts/Bonnie.SaaSService.Application.Contracts.csproj", "services/saas/src/Bonnie.SaaSService.Application.Contracts/"]
COPY ["services/saas/src/Bonnie.SaaSService.Domain.Shared/Bonnie.SaaSService.Domain.Shared.csproj", "services/saas/src/Bonnie.SaaSService.Domain.Shared/"]
COPY ["services/saas/src/Bonnie.SaaSService.Application/Bonnie.SaaSService.Application.csproj", "services/saas/src/Bonnie.SaaSService.Application/"]
COPY ["services/saas/src/Bonnie.SaaSService.Domain/Bonnie.SaaSService.Domain.csproj", "services/saas/src/Bonnie.SaaSService.Domain/"]
COPY ["services/saas/src/Bonnie.SaaSService.EntityFrameworkCore/Bonnie.SaaSService.EntityFrameworkCore.csproj", "services/saas/src/Bonnie.SaaSService.EntityFrameworkCore/"]
RUN dotnet restore "services/saas/host/Bonnie.SaaSService.HttpApi.Host/Bonnie.SaaSService.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/saas/host/Bonnie.SaaSService.HttpApi.Host"
RUN dotnet build "Bonnie.SaaSService.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bonnie.SaaSService.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bonnie.SaaSService.HttpApi.Host.dll"]
