FROM mcr.microsoft.com/mssql/server:2017-latest

RUN -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=abcDEF123#" \
    -p 1433:1433 --SnackDB sqlsv \
    -d mcr.microsoft.com/mssql/server:2017-latest