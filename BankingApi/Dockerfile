# Dockerfile para BankingApi
# Usar a imagem oficial do .NET 8.0 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definir diretório de trabalho
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["BankingApi/BankingApi.csproj", "BankingApi/"]
RUN dotnet restore "BankingApi/BankingApi.csproj"

# Copiar todo o código fonte
COPY BankingApi/ BankingApi/

# Compilar e publicar a aplicação
WORKDIR "/src/BankingApi"
RUN dotnet build "BankingApi.csproj" -c Release -o /app/build
RUN dotnet publish "BankingApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Usar imagem runtime otimizada
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Definir diretório de trabalho
WORKDIR /app

# Criar usuário não-root para segurança
RUN groupadd -r bankingapi && useradd -r -g bankingapi bankingapi

# Criar diretórios necessários para relatórios
RUN mkdir -p /app/SimulationReports && \
    chown -R bankingapi:bankingapi /app

# Copiar arquivos publicados do estágio de build
COPY --from=build /app/publish .

# Definir usuário não-root
USER bankingapi

# Expor porta padrão
EXPOSE 8080
EXPOSE 8443

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Ponto de entrada
ENTRYPOINT ["dotnet", "BankingApi.dll"]