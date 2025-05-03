## Pre-Installation process

Install needed tools:

1. .NET SDK from [Microsoft Official Page](https://dotnet.microsoft.com/en-us/download/dotnet/9.0). Need to be version
   9.0 or above.

## Installation process

1. Clone the repository to your local machine.
2. Open you IDE or Code Editor and navigate to the project folder.
3. Create .env file in the root folder and add environment variables:

```
JWT_SECRET=random_secret
USER_IDENTITY_DB_CONNECTION_STRING=Host=database_ip;Port=5432;Database=user_identity;Username=username;Password=password;
GOOGLE_CLIENT_ID=client_id
```

## Important notes

1. Use SonarQube for IDE to keep the code clean and consistent.
