param([string] $migration = 'CreateDatabase', [string] $targetContext = 'All')

$migratorProject = "shared\Bonnie.DbMigrator\Bonnie.DbMigrator.csproj";

#Initialze db context and define the target directory
$targetContexts = @{ 
    AdminServiceDbContext       = "services\admin\src\Bonnie.AdminService.EntityFrameworkCore\Bonnie.AdminService.EntityFrameworkCore.csproj";
    AuthServiceDbContext        = "services\auth\src\Bonnie.AuthService.EntityFrameworkCore\Bonnie.AuthService.EntityFrameworkCore.csproj";
    ExpenseManagementDbContext  = "services\expenses\src\Bonnie.ExpenseManagement.EntityFrameworkCore\Bonnie.ExpenseManagement.EntityFrameworkCore.csproj";
    SaaSServiceDbContext        = "services\saas\src\Bonnie.SaaSService.EntityFrameworkCore\Bonnie.SaaSService.EntityFrameworkCore.csproj";
}

# #Fix issue when the tools is not installed and the nuget package does not work see https://github.com/MicrosoftDocs/azure-docs/issues/40048
# Write-Host "Updating donet ef tools"
# $env:Path += "%USERPROFILE%\.dotnet\tools";
# dotnet tool update --global dotnet-ef

Write-Host "Start migrate projects" -ForegroundColor Green

foreach ($context in $targetContexts.Keys) {
            
    if ($targetContext -eq 'All' -or $context -eq $targetContext) {

        $migrationProject = $targetContexts[$context];
        $migrationDir = Join-Path -Path (Get-Item $migrationProject ).Directory.FullName -ChildPath "Migrations"

        Write-Host "Adding Migration:     $migration"  -ForegroundColor Green
        Write-Host "Using Context:        $context"  -ForegroundColor Green
        Write-Host "Migration Project:    $migrationProject"  -ForegroundColor Green
        Write-Host "Startup Project:      $migratorProject"  -ForegroundColor Green
        Write-Host "Migrations Directory: $migrationDir" -ForegroundColor Green

        if (($migrationDir | Test-Path) -and ($migration -eq "CreateDatabase")) {
            Write-Host "Removing Existing Migrations: $migrationDir" -ForegroundColor Yellow
            Remove-Item -Path $migrationDir -Recurse -Force
        }

        dotnet ef migrations add $migration -c $context -p $migrationProject -s $migratorProject
    }
} 

docker stop postgres
docker rm postgres
docker volume rm bonnie_postgres_data
docker-compose -f docker-compose.infrastructure.yml up -d postgres
