# wait for the SQL Server to come up
sleep 90s

#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Password1!" -i setup.sql

# wait for scheme to build
sleep 90s

#run the setup script to populate DB with data
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Password1!" -i dataScript.sql

# import the data from the csv file
#/opt/mssql-tools/bin/bcp heroes.dbo.HeroValue in "/usr/work/heroes.csv" -c -t',' -S localhost -U SA -P "Password1!" -d heroes