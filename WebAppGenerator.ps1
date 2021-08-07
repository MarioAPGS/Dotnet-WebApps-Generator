$name = "Cellarium"
$logic = "Logic"
$infra = "Infrastructure"
$core = "Core"

$namePr = "./" + $name + "/" + $name + ".csproj"
$logicPr = "./" + $logic + "/" + $logic + ".csproj"
$infraPr = "./" + $infra + "/" + $infra + ".csproj"
$corePr = "./" + $core + "/" + $core + ".csproj"

cd D:\PROYECTOS\WebApps\CliWebAppsCreator
mkdir $name
cd $name

# Create web api asp net
dotnet new webapi --language "C#" -f "net5.0" --name $name
dotnet new sln 
dotnet sln $name.sln add --in-root $namePr
# Create Logic
dotnet new classlib --language "C#" -f "net5.0" --name $logic
rm ./$logic/Class1.cs
dotnet add $namePr reference $logicPr
dotnet sln $name.sln add --in-root $logicPr
# Create Infrastructure
dotnet new classlib --language "C#" -f "net5.0" --name $infra
rm ./$infra/Class1.cs
dotnet sln $name.sln add --in-root $infraPr
dotnet add $logicPr reference $infraPr
# Create Core
dotnet new classlib --language "C#" -f "net5.0" --name $core
rm ./$core/Class1.cs
dotnet sln $name.sln add --in-root $corePr
dotnet add $infraPr reference $corePr
# Install nuget packages
copy "..\Utils\log4net.config" ./$name
dotnet add $corePr package Newtonsoft.Json
dotnet add $corePr package log4net
dotnet add $corePr package Microsoft.AspNetCore.Http.Abstractions
dotnet add $corePr package RestSharp
dotnet add $infraPr package Microsoft.EntityFrameworkCore
dotnet add $infraPr package Microsoft.EntityFrameworkCore.Design
dotnet add $infraPr package Microsoft.EntityFrameworkCore.Sqlite
dotnet add $infraPr package Microsoft.EntityFrameworkCore.Tools
dotnet add $infraPr package Microsoft.Extensions.Configuration
dotnet add $infraPr package Microsoft.Extensions.Configuration.Json
dotnet add $namePr package Microsoft.AspNetCore.Mvc.NewtonsoftJson
dotnet add $namePr package Microsoft.EntityFrameworkCore.Design
dotnet add $namePr package Microsoft.Extensions.Logging.Log4Net.AspNetCore
# Add DbContext
copy "..\Utils\DbContext.cs" ./$infra
$DbContextPath = "./" + $infra + "/" + "DbContext.cs";
(Get-Content $DbContextPath).replace('$$$', $name) | Set-Content $DbContextPath
$DbContextNewName = $name + "DbContext.cs"
Rename-Item -Path $DbContextPath -NewName $DbContextNewName
# Copy folder's structures
copy "..\Utils\Core" ./$core
Copy-item -Force -Recurse -Verbose "..\Utils\Core" -Destination ./
Copy-item -Force -Recurse -Verbose "..\Utils\Infrastructure" -Destination ./
Copy-item -Force -Recurse -Verbose "..\Utils\Logic" -Destination ./
# Configure Setup.cs
$StartupPath = "./" + $name + "/" + "Startup.cs";
(Get-Content "..\Utils\Startup.cs") | Set-Content $StartupPath
(Get-Content $StartupPath).replace('$$$', $name) | Set-Content $StartupPath