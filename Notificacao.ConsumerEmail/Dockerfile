#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Notificacao.ConsumerEmail/email.html", "Notificacao.ConsumerEmail/"]
COPY ["Notificacao.ConsumerEmail/imgLogoEmail.png", "Notificacao.ConsumerEmail/"]
COPY ["Notificacao.ConsumerEmail/Notificacao.ConsumerEmail.csproj", "Notificacao.ConsumerEmail/"]
RUN dotnet restore "Notificacao.ConsumerEmail/Notificacao.ConsumerEmail.csproj"
COPY . .
WORKDIR "/src/Notificacao.ConsumerEmail"
RUN dotnet build "Notificacao.ConsumerEmail.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notificacao.ConsumerEmail.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notificacao.ConsumerEmail.dll"]