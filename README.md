# GymnasieProjekt – Attendance Overview

A WPF prototype that gives a better overview of student absence (fravær) at a Danish gymnasium, based on data from Lectio.
2nd semester exam project.

The app only **reads** data. All data in this repository is made up test data – no real student data is used (GDPR).

## Solution structure

```
GymnasieProjekt.sln
├── GymnasieProjekt.Core      Class library: models and repositories (SQL Server via ADO.NET)
│   ├── Models/
│   └── Repositories/
├── GymnasieProjekt.WPF       WPF app using MVVM
│   ├── MVVM/                 ViewModelBase, RelayCommand
│   ├── ViewModels/
│   ├── Views/
│   └── appsettings.json      Connection string
└── Database/
    └── CreateDatabase.sql    Creates the database, tables and test data
```

## Requirements

- Visual Studio 2026 or newer, with the **.NET desktop development** workload (the projects target .NET 10)
- .NET 10 SDK
- SQL Server (Developer or Express edition) or SQL Server LocalDB
- SQL Server Management Studio (SSMS), or another tool that can run SQL scripts

## 1. Create the database

The script creates the database `GymnasieProjektDB`, the tables and some test data.
It can be run again at any time – it drops and recreates the tables, so **all existing data is reset**.

### Using SSMS

1. Open SSMS and connect to your SQL Server.
   The server name you connect to here is the one you need in the connection string (see step 2).
2. Choose **File → Open → File…** and open `Database/CreateDatabase.sql`.
3. Click **Execute** (or press **F5**).
4. Check that `GymnasieProjektDB` appears under **Databases** (right-click → Refresh if needed).

### Using the command line (alternative)

```bash
sqlcmd -S YOUR_SERVER_NAME -E -i Database/CreateDatabase.sql
```

`-E` uses your Windows login. Replace `YOUR_SERVER_NAME` with e.g. `localhost` or `.\SQLEXPRESS`.

## 2. Set the connection string

The connection string is in **`GymnasieProjekt.WPF/appsettings.json`**:

```json
{
  "ConnectionStrings": {
    "GymnasieProjektDB": "Server=DESKTOP-APV603A;Database=GymnasieProjektDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> **Each developer must change `Server=`** to their own SQL Server. `DESKTOP-APV603A` is one developer's machine and will not work on yours.

Only the value needs to change – the name `GymnasieProjektDB` is what the code looks up, so keep it.

### Common examples

| Setup | Connection string |
|---|---|
| Local default instance | `Server=localhost;Database=GymnasieProjektDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server Express | `Server=.\\SQLEXPRESS;Database=GymnasieProjektDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| LocalDB | `Server=(localdb)\\MSSQLLocalDB;Database=GymnasieProjektDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL login (username/password) | `Server=SERVERNAME;Database=GymnasieProjektDB;User Id=USERNAME;Password=PASSWORD;TrustServerCertificate=True;` |

Note: backslashes must be written as `\\` inside JSON.

### Pointing the app at the actual system

When the app should use the school's real database instead of the test database:

1. Get the following from the school's IT department:
   - the **server name** (and instance name, if any)
   - the **database name**
   - how to log in: **Windows authentication** (`Trusted_Connection=True`) or a **SQL login** (`User Id` / `Password`)
2. Replace the value in `appsettings.json` with the new connection string.
3. Use an account with **read-only access** (e.g. the `db_datareader` role). The app never writes data, and it should not be able to.
4. `TrustServerCertificate=True` skips certificate validation. That is fine for development, but on a real server with a valid certificate it should be removed (or set to `False`), so the connection is encrypted *and* verified.
5. The real database must have the same tables and columns as the ones in `Database/CreateDatabase.sql`. If it does not, the SQL in the repositories in `GymnasieProjekt.Core/Repositories` has to be adjusted.

### Editing an already built/installed app

`appsettings.json` is copied next to the `.exe` when the project is built (e.g. `GymnasieProjekt.WPF/bin/Debug/net10.0-windows/`).
On a machine where the app is already installed, the connection string can be changed by editing that copy – no rebuild needed. Restart the app afterwards.

### ⚠️ Never commit real credentials

If the real connection string contains a password, **do not commit it to GitHub**. Keep the test connection string in the repository and only enter the real one on the machine that runs the app.

## 3. Run the app

1. Open `GymnasieProjekt.sln` in Visual Studio.
2. Right-click **GymnasieProjekt.WPF** → **Set as Startup Project**.
3. Press **F5**.

The window shows the students from the database and can filter by class.

## Troubleshooting

| Problem | Likely cause |
|---|---|
| The window takes 15–30 seconds to open, then shows an error at the bottom | The app cannot reach the database. Check the server name in `appsettings.json` and that SQL Server is running. |
| `Cannot open database "GymnasieProjektDB"` | The database has not been created – run `Database/CreateDatabase.sql`. |
| `Login failed for user …` | Your Windows user (or SQL login) has no access to the server or database. |
| `The certificate chain was issued by an authority that is not trusted` | `TrustServerCertificate=True` is missing from the connection string. |
| `Connection string 'GymnasieProjektDB' mangler i appsettings.json` | The key was renamed or `appsettings.json` is missing from the build folder. Rebuild the project. |
