# TODO Lab 13 (Sectiunea 01 - Dockerfile pentru .NET API)
#
# Scrie un Dockerfile multi-stage care:
#  1. STAGE 1 (build): foloseste imagine SDK 8.0-alpine pentru `dotnet restore`
#     si `dotnet publish` (output -> /out)
#  2. STAGE 2 (runtime): foloseste imagine aspnet:8.0-alpine si copy-aza /out din build
#  3. Adauga user non-root pentru a rula procesul
#  4. EXPOSE 8080 + ENV ASPNETCORE_URLS=http://+:8080
#  5. ENTRYPOINT ["dotnet", "Lab12.dll"]
#
# Hint: numele DLL-ului final = numele csproj-ului fara extensie
# Hint: layer caching - copiaza *.csproj inainte de COPY . (restore in cache)

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# TODO: layer cache pentru NuGet restore
# COPY *.csproj ./
# RUN dotnet restore Lab12.csproj

# TODO: copy + publish
# COPY . .
# RUN dotnet publish Lab12.csproj -c Release -o /out --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

# TODO: copiaza output-ul publish
# COPY --from=build /out .

# TODO: user non-root
# RUN addgroup -S app && adduser -S -G app app && chown -R app:app /app
# USER app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# TODO: ENTRYPOINT
# ENTRYPOINT ["dotnet", "Lab12.dll"]
