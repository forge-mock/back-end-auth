## Pre-Installation process

Install needed tools:

1. .NET SDK from [Microsoft Official Page](https://dotnet.microsoft.com/en-us/download/dotnet/9.0). Need to be version
   9.0 or above.

## Installation process

1. Clone the repository to your local machine.
2. Open you IDE or Code Editor and navigate to the project folder.
3. Create environment variables for DB Connection and JWT Secret:

- For macOS in Terminal:

```bash
export JWT_SECRET=random_secret >> ~/.zshrc
export AUTH_DB_CONNECTION_STRING="Host=database_ip;Port=5432;Database=auth;Username=username;Password=password;" >> ~/.zshrc
source ~/.zshrc
```

- For Linux in Terminal:

```bash
export JWT_SECRET=random_secret >> ~/.bash_profile
export AUTH_DB_CONNECTION_STRING="Host=database_ip;Port=5432;Database=auth;Username=username;Password=password;" >> ~/.bash_profile
source ~/.bash_profile
```

- For Windows in Command Prompt:

```bash
setx JWT_SECRET "random_secret" /M
setx AUTH_DB_CONNECTION_STRING "Host=database_ip;Port=5432;Database=auth;Username=username;Password=password;" /M
```

4. It's recommended to reload the terminal and restart the IDE.

## Important notes

1. Use SonarQube for IDE to keep the code clean and consistent.
