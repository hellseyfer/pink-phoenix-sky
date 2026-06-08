# Flight Aggregator Backend

A backend REST API for a flight price aggregation service, built with **.NET 10** and **Minimal APIs**. It aggregates flight offers from multiple airline providers and exposes a unified API for search and booking.

Provider integrations are currently **mocked**, but the architecture (Adapter Pattern + dependency injection) makes it trivial to swap mocks for real external HTTP APIs without changing the aggregation logic.

> A future Angular 21 frontend will consume this API.

## How It Works

1. A client sends a search request (`origin`, `destination`, `departureDate`, `passengers`, `cabinClass`).
2. The `FlightSearchService` aggregator fans the request out to **every** registered `IFlightProvider` concurrently.
3. Each provider acts as an **adapter**: it translates its own external schema into the internal `FlightOffer` domain model and applies its own pricing rules.
4. Results from all providers are flattened into a single normalized list and returned.
5. The client can later book a specific flight via the booking endpoint, which returns a unique booking reference.

Because the aggregator depends **only** on the `IFlightProvider` interface, there are no provider-specific `switch`/`if` chains or parsing logic outside the adapters.

## Architecture

```
+-----------------------------+
|     FlightAggregator.Api     |   Minimal API endpoints + DI wiring
|  POST /api/flights/search    |
|  POST /api/bookings          |
+--------------+--------------+
               |
               v
+-----------------------------+
|    FlightAggregator.Core     |   Domain model + abstractions + services
|  - FlightOffer / requests    |
|  - IFlightProvider (contract)|
|  - FlightSearchService        |  <- aggregator, depends only on IFlightProvider
|  - BookingService             |
+--------------+--------------+
               | implemented by
               v
+-----------------------------+
| FlightAggregator.Providers   |   Adapter implementations (mocked)
|  - GlobalAirProvider          |
|  - BudgetWingsProvider        |
+-----------------------------+
```

### Projects

- **`src/FlightAggregator.Core`** — Domain models (`FlightOffer`, `FlightSearchRequest`, `CabinClass`, booking records), service abstractions (`IFlightProvider`, `IFlightSearchService`, `IBookingService`), and the `FlightSearchService` aggregator + `BookingService`.
- **`src/FlightAggregator.Providers`** — Provider adapters implementing `IFlightProvider`. Each adapter owns its external-schema mapping **and** its pricing rule.
- **`src/FlightAggregator.Api`** — Minimal API host, request validation, and DI registration.
- **`tests/FlightAggregator.Tests`** — xUnit tests for pricing rules and aggregation behavior.

### Key design decisions

- **Adapter Pattern** — Providers may expose completely different request/response formats. Each provider normalizes its data into the shared `FlightOffer` model.
- **Aggregator depends only on the contract** — `FlightSearchService` receives `IReadOnlyList<IFlightProvider>` and never knows which concrete providers exist.
- **Pricing stays local to each provider** — There is no shared pricing engine, since the rules differ per provider. Each adapter exposes a static `CalculatePerPassengerPrice` method (unit-tested directly).
- **Easy to go real** — Replacing a mock with a real HTTP client only requires a new `IFlightProvider` implementation; the aggregator and endpoints stay unchanged.

## Provider Pricing Rules

### GlobalAir
`Final Price Per Passenger = Base Fare + 15% Fuel Surcharge`, rounded to 2 decimals.

| Base Fare | Per Passenger |
|-----------|---------------|
| 100.00    | 115.00        |
| 123.45    | 141.97        |

### BudgetWings
`Final Price Per Passenger = Base Fare - 10% Promotional Discount`, with a **minimum of 29.99 USD**, rounded to 2 decimals.

| Base Fare | Per Passenger |
|-----------|---------------|
| 100.00    | 90.00         |
| 40.00     | 36.00         |
| 20.00     | 29.99         |

### Total price
`Total Price = Price Per Passenger × Number Of Passengers`

## Adding a New Provider

Adding a provider requires **only** a new adapter — no changes to the aggregator or endpoints.

1. Create a class in `src/FlightAggregator.Providers` implementing `IFlightProvider`:

```csharp
using FlightAggregator.Core;

public sealed class SkyHighProvider : IFlightProvider
{
    public Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(
        FlightSearchRequest request, CancellationToken cancellationToken)
    {
        // 1. Call the external API (or return mock data).
        // 2. Map the external schema into FlightOffer(s).
        // 3. Apply this provider's pricing rule and compute totals.
        // 4. Return the normalized offers.
    }
}
```

2. Register it in `src/FlightAggregator.Api/Program.cs`:

```csharp
builder.Services.AddSingleton<IFlightProvider, SkyHighProvider>();
```

That's it — the aggregator automatically includes it in every search.

## API Endpoints

### `POST /api/flights/search`

Request:
```json
{
  "origin": "JFK",
  "destination": "LAX",
  "departureDate": "2026-06-15",
  "passengers": 2,
  "cabinClass": "Economy"
}
```

Supported `cabinClass` values: `Economy`, `Business`, `First`.

Response:
```json
[
  {
    "id": "globalair-ga123",
    "provider": "GlobalAir",
    "flightNumber": "GA123",
    "origin": "JFK",
    "destination": "LAX",
    "departureTime": "2026-06-15T08:00:00+00:00",
    "arrivalTime": "2026-06-15T11:30:00+00:00",
    "durationMinutes": 210,
    "cabinClass": "Economy",
    "pricePerPassenger": 115.00,
    "totalPrice": 230.00,
    "currency": "USD"
  }
]
```

The response contains everything the frontend needs to display offers and sort by **price**, **duration**, or **departure time**.

### `POST /api/bookings`

Request:
```json
{
  "flightId": "globalair-ga123",
  "passengers": [
    {
      "fullName": "John Doe",
      "email": "john@example.com",
      "documentNumber": "ABC123"
    }
  ]
}
```

Response:
```json
{
  "bookingReference": "BK-9F7A2D"
}
```

## Running

```bash
# Run the API
dotnet run --project src/FlightAggregator.Api

# Run the tests
dotnet test
```

### Example requests (curl)

```bash
curl -X POST http://localhost:5000/api/flights/search \
  -H "Content-Type: application/json" \
  -d '{"origin":"JFK","destination":"LAX","departureDate":"2026-06-15","passengers":2,"cabinClass":"Economy"}'
```

```bash
curl -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d '{"flightId":"globalair-ga123","passengers":[{"fullName":"John Doe","email":"john@example.com","documentNumber":"ABC123"}]}'
```

> The actual port is printed on startup (e.g. `http://localhost:5xxx`). Adjust the URL accordingly.
