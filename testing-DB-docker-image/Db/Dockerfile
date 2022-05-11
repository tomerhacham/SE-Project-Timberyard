# Build of top of the latest image
FROM mcr.microsoft.com/mssql/server:2019-latest
USER root
# Create work directory
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

# Copy all scripts into working directory
COPY . /usr/src/app


ENV SA_PASSWORD password123!
ENV ACCEPT_EULA Y
ENV MSSQL_PID Express


# Grant permissions for the import-data script to be executable
#RUN chmod +x /usr/work/import-data.sh
EXPOSE 1433
CMD /bin/bash ./entrypoint.sh