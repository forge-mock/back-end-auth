FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY Auth.Api/*.csproj ./Auth.Api/
COPY Auth.Application/*.csproj ./Auth.Application/
COPY Auth.Domain/*.csproj ./Auth.Domain/
COPY Auth.Persistence/*.csproj ./Auth.Persistence/

RUN dotnet restore

COPY Auth.Api/ ./Auth.Api/
COPY Auth.Application/ ./Auth.Application/
COPY Auth.Domain/ ./Auth.Domain/
COPY Auth.Persistence/ ./Auth.Persistence/

WORKDIR /src/Auth.Api
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
CMD ["sh", "/app/healthcheck.sh"]

ENTRYPOINT ["dotnet", "Auth.Api.dll"]