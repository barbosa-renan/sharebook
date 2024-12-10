# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiando os arquivos .csproj
COPY ["Sharebook.API/Sharebook.API.csproj", "Sharebook.API/"]
COPY ["Sharebook.Infra/Sharebook.Infra.csproj", "Sharebook.Infra/"]
COPY ["Sharebook.Services/Sharebook.Services.csproj", "Sharebook.Services/"]
COPY ["Sharebook.Core/Sharebook.Core.csproj", "Sharebook.Core/"]

# Restaurando dependências
RUN dotnet restore "Sharebook.API/Sharebook.API.csproj"

# Copiando o restante do código-fonte
COPY . .

# Compilando o projeto
WORKDIR "/src/Sharebook.API"
RUN dotnet build "Sharebook.API.csproj" -c Release -o /app/build

# Etapa de Publicação
FROM build AS publish
RUN dotnet publish "Sharebook.API.csproj" -c Release -o /app/publish

# Etapa Final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sharebook.API.dll"]
