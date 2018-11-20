# .NET Core - Using Gatekeeper

## Get your application Client ID/Secret

You need a Client ID and Secret to use Gatekeeper.  These will be different for each development, staging, and production environment.  Join `#dev-gatekeeper` in Slack to get a client ID/secret for development.  For staging/production, our docker-compose file will take care of setting these as environment variables inside the container running your app.

Your application should load the client ID and secret from environment variables, as shown later.  You can set environment variables for your development environment in Visual Studio by right clicking your project, opening Properties -> Debug, and entering them in the Environment Variables section.  In order for .NET Core to read these values automatically, you should prefix them with your application name and two underscores.  For example, if your application is called HealthData, your environment variables should start with `HealthData__`.

![Setting environment variables in Visual Studio][VS_EnvironmentVariables]

[VS_EnvironmentVariables]: ./vs_envvars.png

---

## My app has a UI and I want to use Gatekeeper to log in

### I want to restrict certain routes/pages to coordinators/administrators

---

## My app has an API I want to protect

---

## I want to get information about the currently logged in User

---

## I need to make requests to another API, identifying myself as the currently logged in User

---

## I need to make requests to another API, identifying myself as my application

---