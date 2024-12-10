FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Sharebook.API/Sharebook.API.csproj", "Sharebook.API/"]
COPY ["Sharebook.Infra/Sharebook.Infra.csproj", "Sharebook.Infra/"]
COPY ["Sharebook.Services/Sharebook.Services.csproj", "Sharebook.Services/"]
COPY ["Sharebook.Core/Sharebook.Core.csproj", "Sharebook.Core/"]

RUN dotnet restore "Sharebook.API/Sharebook.API.csproj"

COPY . .

WORKDIR "/src/Sharebook.API"
RUN dotnet build "Sharebook.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sharebook.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sharebook.API.dll"]
