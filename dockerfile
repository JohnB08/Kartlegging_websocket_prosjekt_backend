FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:8ab06772f296ed5f541350334f15d9e2ce84ad4b3ce70c90f2e43db2752c30f6 AS build

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6159cf66274cf52730d7a0c7bb05cf0af94b79370176886ac58286ab6cbb7faf AS runtime

WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT [ "dotnet", "kartlegging_websocket_prosjekt_backend.dll" ]