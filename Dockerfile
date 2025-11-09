# etapa 1 la build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

#2 copiar los archivos del proyecto
COPY src/ComparadorTasasCambio.App/*.csproj ./src/ComparadorTasasCambio.App/
RUN dotnet restore src/ComparadorTasasCambio.App/ComparadorTasasCambio.App.csproj

#3 copiar todo el codigo fuente
COPY . .
RUN dotnet publish src/ComparadorTasasCambio.App/ComparadorTasasCambio.App.csproj \
    -c Release \
    -o /app/publish

#4 run
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .


ENV API1_URL="https://api1.ejemplo.com"
ENV API1_KEY="key1"
ENV API1_TIMEOUT="5"
ENV API2_URL="https://api2.ejemplo.com"
ENV API2_KEY="key2"
ENV API2_TIMEOUT="5"
ENV API3_URL="https://api3.ejemplo.com"
ENV API3_KEY="key3"
ENV API3_TIMEOUT="5"

ENTRYPOINT ["dotnet", "ComparadorTasasCambio.App.dll"]
CMD ["USD", "DOP", "100"]
