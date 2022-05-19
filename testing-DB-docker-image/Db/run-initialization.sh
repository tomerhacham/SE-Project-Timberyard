# Wait to be sure that SQL Server came up
echo 'Sleeping for 90 sec'
sleep 90
echo 'Start building scheme and inserting data'

# Run the setup script to create the DB and the schema in the DB
# Note: make sure that your password matches what is in the Dockerfile
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password123! -d master -i setup.sql