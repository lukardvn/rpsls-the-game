# Rock Paper Scissors Lizard Spock API

A RESTful API for playing the classic "Rock, Paper, Scissors, Lizard, Spock" game, inspired by _The Big Bang Theory_.  
Following Clean Architecture and built with **.NET Core** and **PostgreSQL**, and fully containerized with **Docker**.

## 🚀 Features

- Play matches against computer using extended [Rock-Paper-Scissors rules](https://www.samkass.com/theories/RPSSL.html)
- Track scores per player
- Reset all the scores
- See the leaderboard: Best players with most Ws
- RESTful design
- Docker support for easy local or cloud deployment

## 📋 Prerequisites

### With Docker

- [Docker](https://www.docker.com/get-started)

### Without Docker

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 17+](https://www.postgresql.org/download)

## 🎬 Getting Started

Clone the repository:

```
git clone https://github.com/lukardvn/rpsls-the-game
cd rpsls-the-game
```

### 🐳 With Docker

1. Make sure your Docker dekstop is open
2. Run:

   ```
   docker-compose up --build -d
   ```

- API is now available on port `8080`
- API can be viewed in Swagger `http://localhost:8080/swagger.index.html`
- Database is available on port `5432`

### 🧪 Without Docker

1. Setup local PostgreSQL database and update your connection string in `appsettings.Development.json`:

   ```
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=rpslsDb;Username=postgres;Password=yourpassword"
   }
   ```

2. Apply migrations:

   ```
    dotnet ef database update --project src/rpsls.Infrastructure --startup-project src/rpsls.Api
   ```

3. Run the API:

   ```
   dotnet run --project src/rpsls.Api
   ```

4. API is now available and can be accessed on port 5059 or viewed in Swagger on `http://localhost:5059/swagger/index.html`

## ⚙️ API Endpoints

### Get choices

```http
GET /game/choices
```

### Get choice

```http
GET /game/choice
```

### Play game

```http
POST /game/play
Content-Type: application/json

{
  "choice": 1,
  "username": "string"
}
```

### Get scoreboard

Returns most recent scores for a specific user.
Count decides max number of scores and is optional and defaults to 10.

```http
GET /game/scoreboard?username=name&count=10
```

### Reset scoreboard

```http
POST /game/scoreboard/reset
Content-Type: application/json
```

### Get leaderboard

Return usernames with best scores and their win rate.
Count decides max number of scores and is optional and defaults to 5.

```http
GET /game/leaderboard?count=5
```

## 🧪 Running Tests

To confirm that everything is running as it should you can run tests.

```
dotnet test
```
