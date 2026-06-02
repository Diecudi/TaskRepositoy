# Etapa 1: Construcción (Usa el SDK de .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["Tasks.csproj", "./"]
RUN dotnet restore "Tasks.csproj"

# Copiar el resto del código y publicarlo
COPY . .
RUN dotnet publish "Tasks.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Ejecución (Usa una imagen ligera solo para correr la app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tasks.dll"]