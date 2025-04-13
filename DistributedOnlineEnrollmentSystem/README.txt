Prerequisites:
1. Docker (Docker-Desktop)
2. Visual Studio (Recommended)
3. WSL2 (if Windows)
4. Dotnet 9.0 Framework
5. MySQL Workbench

Database Setup
1. Set up database server in MySQL Workbench
2. Populate schema using PopulateP4.sql
3. Configure appsettings.json of every node accordingly

Appsettings.json Setup
1. appsettings.json is not in this repository due to it being in the .gitignore
2. Add appsettings.json to every node
3. Paste the following code inside appsettings.json:
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "DefaultConnection": "server=host.docker.internal;port=3306;database=onlineenrollmentsystem;user=root;password=root;"
    },
    "Jwt": {
        "SecretKey": "ThisIsTheSuperSecretKeyOfLilyThatIsOnlyHereForTestingPurposes",
        "Issuer": "http://localhost:5001",
        "Audience": "http://localhost:5002"
    }
}

To run:
1. Go to root directory of solution
2. Run "docker-compose up --build"
3. Open ViewsNode's port through the docker (http://localhost:5000)
