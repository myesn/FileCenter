cd ..
rmdir /S /Q Migrations

dotnet ef migrations add InitialFileCenterDbContextMigration -c FileCenterDbContext -o Migrations/FileCenterDb

dotnet ef migrations script -c FileCenterDbContext -o Migrations/FileCenterDb.sql

cd ef-cli