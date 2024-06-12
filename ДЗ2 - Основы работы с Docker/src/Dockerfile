# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln ./aspnetapp/
COPY *.csproj ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet restore

# copy everything else and build app
COPY Attributes/. ./Attributes/
COPY Controllers/. ./Controllers/
COPY Models/. ./Models/
COPY Properties/. ./Properties/
COPY Views/. ./Views/
COPY wwwroot/. ./wwwroot/
COPY appsettings.Development.json ./
COPY appsettings.json ./
COPY Program.cs ./
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_HTTP_PORTS=80
ENTRYPOINT ["dotnet", "src.dll"]
