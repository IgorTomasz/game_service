# Games Microservice

## 📝 Opis projektu
Mikroserwis Games jest częścią większego systemu kasyna online, odpowiedzialny za obsługę logiki gier hazardowych. Projekt został zrealizowany jako praca dyplomowa, wykorzystując nowoczesne technologie w tworzeniu skalowalnych aplikacji.

Serwis jest jednym z czterech mikroserwisów tworzących kompletny system:
- 🎮 Game Service (ten projekt) - obsługa logiki gier
- 👤 Account Service - zarządzanie kontami użytkowników
- 💰 Payment Service - obsługa płatności
- 🔀 Gateway Service - zarządzanie komunikacją między serwisami

## 🎮 Dostępne gry
- Plinko
- BlackJack
- Mines
- Dice
- Frog Game

## 🛠 Technologie
- ASP.NET Core
- Entity Framework Core
- MS SQL Server
- Docker
- REST API

## 🔒 Zabezpieczenia
System implementuje wielopoziomowe zabezpieczenia:
1. **IP Whitelist**
   - Filtrowanie requestów na podstawie dozwolonych adresów IP
   - Konfiguracja w pliku appsettings.json

2. **Custom Header Validation**
   - Walidacja specjalnego nagłówka w każdym żądaniu
   - Wartość nagłówka porównywana z konfiguracją w appsettings.json

## 📋 Wymagania systemowe
- .NET 6.0 lub nowszy
- Docker Desktop
- MS SQL Server (opcjonalnie, jeśli nie używamy Dockera)

## ⚙️ Konfiguracja i uruchomienie

### Przy użyciu Dockera:
```bash
# Sklonuj repozytorium
git clone https://github.com/your-username/casino-games-service.git

# Przejdź do katalogu projektu
cd games_service

# Zbuduj i uruchom kontenery
docker-compose up --build
```

### Lokalne uruchomienie:
1. Sklonuj repozytorium
2. Zaktualizuj connection string w `appsettings.json`
3. Wykonaj migracje bazy danych:
```bash
dotnet ef database update
```
4. Uruchom aplikację:
```bash
dotnet run
```

## 🚀 Endpointy API

### Zarządzanie grami

# Pobieranie wszystkich dostępnych gier
```http
GET /Game/games
```

# Pobieranie gier według kategorii
```http
GET /Game/games/{category}
```
# gdzie category to:
# 0 - Card
# 1 - Arcade
# 2 - Random
# 3 - Strategy

# Dodawanie nowej gry
```http
POST /Game/addGame
```

### Zarządzanie sesjami gry
```http
# Utworzenie nowej sesji dla użytkownika
POST /Game/getSession
```
Request body:
```json
{
    "userId": "guid",
    "userSessionId": "guid",
    "gameType": "string"
}
```

```http
# Sprawdzenie statusu zakończenia gry
GET /Game/games/ended/{gameSessionId}
```

### Proces gry
```http
POST /Game/process
```

#### Start gry
```json
{
    "type": "Plinko",
    "userId": "guid",
    "userSessionId": "guid",
    "action": "Start",
    "betAmount": 100.00,
    "data": {
        // Specyficzne parametry dla danej gry
    }
}
```

#### Ruch w grze
```json
{
    "type": "Mines",
    "gameSessionId": "guid",
    "userId": "guid",
    "userSessionId": "guid",
    "action": "Move",
    "betAmount": 100.00,
    "data": {
        // Parametry ruchu specyficzne dla danej gry
    }
}
```

#### Wcześniejsze zakończenie gry
```json
{
    "type": "Mines",
    "gameSessionId": "guid",
    "userId": "guid",
    "userSessionId": "guid",
    "action": "End",
    "betAmount": 100.00,
    "data": {}
}
```

## 👨‍💼 Panel Administracyjny

### Endpointy administracyjne
```http
# Pobieranie wszystkich gier (włącznie z nieaktywnymi)
GET /Game/adm/games

# Aktualizacja statusu aktywności gry
PUT /Game/adm/games/update
```

Request body dla aktualizacji statusu:
```json
{
    "gameId": "guid",
    "isActive": true
}
```

## 📤 Struktura odpowiedzi API
Większość endpointów w systemie zwraca ujednoliconą strukturę odpowiedzi w formacie (wyjątkiem są endpointy games i games/{category}, które zamiast object Message zwracają List<Games> Games):

```csharp
public class HttpResponseModel
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public object? Message { get; set; }
}
```

Przykładowa odpowiedź sukcesu:
```json
{
    "success": true,
    "error": null,
    "message": {
        "gameId": "123e4567-e89b-12d3-a456-426614174000",
        "status": "in_progress",
        "currentBet": 100.00
    }
}
```

Przykładowa odpowiedź błędu:
```json
{
    "success": false,
    "error": "Insufficient funds",
    "message": null
}
```

## ⚖️ Walidacja i obsługa błędów
- Wszystkie endpointy wymagają autoryzacji przez nagłówek
- Walidacja IP dla każdego żądania
- Standardowe kody odpowiedzi HTTP
- Szczegółowe komunikaty błędów w formacie JSON

## 🔄 Integracja z pozostałymi serwisami
- Wykorzystanie Gateway Service jako punkt wejścia do systemu

## 📈 Możliwości rozwoju
- Dodanie nowych gier
- Implementacja systemu websockets do wysyłania aktualizacji o ostatnich wygranych
- Rozbudowa systemu statystyk

## 👨‍💻 Autor
Igor Tomaszewski
