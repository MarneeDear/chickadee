FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./

# Build app
RUN dotnet tool restore
RUN dotnet paket install
RUN dotnet fake build target MigrateUp
RUN dotnet fake build target Build
RUN dotnet fake build RunService
RUN dotnet fake build target RunWeb

ENTRYPOINT bash