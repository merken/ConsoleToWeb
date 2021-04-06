FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build-env
WORKDIR /app

COPY src/ConsoleToWeb/*.csproj ./
RUN dotnet restore

COPY src/ConsoleToWeb/Program.cs ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 5005
RUN useradd --create-home dotnet
USER dotnet
ENTRYPOINT ["dotnet", "ConsoleToWeb.dll"]