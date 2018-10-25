FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 57887
EXPOSE 44338

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Gatekeeper/Gatekeeper.csproj", "Gatekeeper/"]
RUN dotnet restore "Gatekeeper/Gatekeeper.csproj"
COPY . .
WORKDIR "/src/Gatekeeper"
RUN dotnet build "Gatekeeper.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Gatekeeper.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Gatekeeper.dll"]