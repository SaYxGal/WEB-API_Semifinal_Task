# base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
EXPOSE 80
WORKDIR /app

#build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
RUN dotnet publish -c release -o /app --no-restore


# final image
FROM base AS final
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]