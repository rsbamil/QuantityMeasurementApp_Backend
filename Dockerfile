FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore QuantityMeasurementAPI/QuantityMeasurementAPI.csproj
RUN dotnet publish QuantityMeasurementAPI/QuantityMeasurementAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 10000
ENV PORT=10000
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "QuantityMeasurementAPI.dll"]