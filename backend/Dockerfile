FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/src/SSMS.Core/SSMS.Core.csproj ./SSMS.Core/
COPY backend/src/SSMS.Application/SSMS.Application.csproj ./SSMS.Application/
COPY backend/src/SSMS.Infrastructure/SSMS.Infrastructure.csproj ./SSMS.Infrastructure/
COPY backend/src/SSMS.API/SSMS.API.csproj ./SSMS.API/

RUN dotnet restore ./SSMS.API/SSMS.API.csproj

COPY backend/src/SSMS.Core/. ./SSMS.Core/
COPY backend/src/SSMS.Application/. ./SSMS.Application/
COPY backend/src/SSMS.Infrastructure/. ./SSMS.Infrastructure/
COPY backend/src/SSMS.API/. ./SSMS.API/

WORKDIR /src/SSMS.API
RUN dotnet publish SSMS.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

RUN mkdir -p /app/logs /app/uploads /app/uploads/submissions \
    && chown -R app:app /app/logs /app/uploads \
    && chmod -R 755 /app/logs /app/uploads

USER app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Asia/Ho_Chi_Minh

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SSMS.API.dll"]
