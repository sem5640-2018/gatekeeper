FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN /root/.dotnet/tools/libman restore
RUN dotnet publish -c Release -o out

# We want to run with HTTPS inside the container due to IS4 requirements.  The security of the cert isn't a major issue as
# there will be a reverse proxy in front of it with a proper TLS cert, so we'll just use a self-signed cert with a known password
RUN openssl genrsa -des3 -passout pass:containertls -out server.key 2048
RUN openssl rsa -passin pass:containertls -in server.key -out server.key
RUN openssl req -sha256 -new -key server.key -out server.csr -subj '/CN=localhost'
RUN openssl x509 -req -sha256 -days 365 -in server.csr -signkey server.key -out server.crt
RUN openssl pkcs12 -export -out cert.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:containertls

FROM microsoft/dotnet:2.1-aspnetcore-runtime
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=https://+:443;http://+:80;
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/cert.pfx .
ENTRYPOINT ["dotnet", "Gatekeeper.dll"]