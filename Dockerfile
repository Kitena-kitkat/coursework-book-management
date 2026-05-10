FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BookManagement.csproj", "./"]
RUN dotnet restore "BookManagement.csproj"

COPY . .
RUN dotnet publish "BookManagement.csproj" -c Release -o /app/publish /p:Configuration=Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000
ENTRYPOINT ["dotnet", "BookManagement.dll"]