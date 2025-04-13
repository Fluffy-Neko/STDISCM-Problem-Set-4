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

To run:
1. Go to root directory of solution
2. Run "docker-compose up --build"
3. Open ViewsNode's port through the docker (http://localhost:5000)
