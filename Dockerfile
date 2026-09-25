# ---------- Сборка ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# сначала только проект — restore кешируется, пока не меняются зависимости
COPY UserManagerApi.csproj ./

RUN dotnet restore UserManagerApi.csproj

COPY . .

RUN dotnet publish UserManagerApi.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false


# ---------- Запуск ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

# файлы принадлежат непривилегированному пользователю app — ему нужно писать в wwwroot/images
COPY --from=build --chown=app:app /app/publish .

USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "UserManagerApi.dll"]
