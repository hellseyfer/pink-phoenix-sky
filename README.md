# Flight Aggregator Backend

A backend REST API for a flight price aggregation service, built with **.NET 10** and **Minimal APIs**. It aggregates flight offers from multiple airline providers and exposes a unified API for search and booking.

The codebase is organized as a **Modular Monolith with Vertical Slice Architecture** (inspired by [this article](https://antondevtips.com/blog/building-a-modular-monolith-with-vertical-slice-architecture-in-dotnet)). Each business capability is an independent module with clear boundaries; modules talk to each other only through a published **PublicApi** contract — never via internal types.

Provider integrations are currently **mocked**, but the architecture (Adapter Pattern + dependency injection) makes it trivial to swap mocks for real external HTTP APIs without changing the aggregation logic.

> A future Angular 21 frontend will consume this API.

## How It Works

**Search (Flights module):**
1. A client sends a search request (`origin`, `destination`, `departureDate`, `passengers`, `cabinClass`).
2. The `SearchFlightsHandler` aggregator fans the request out to **every** registered `IFlightProvider` concurrently.
3. Each provider acts as an **adapter**: it translates its own external schema into the internal `FlightOffer` domain model and applies its own pricing rules.
4. Results from all providers are flattened into a single normalized list and returned.

**Booking (Bookings module):**
1. A client books a specific flight via the booking endpoint.
2. The `CreateBookingHandler` calls the Flights module's `IFlightsModuleApi.FlightExistsAsync` to validate the `flightId` (an **inter-module call** through the PublicApi contract).
3. If valid, it returns a unique booking reference; otherwise a `400`.

Because the aggregator depends **only** on the `IFlightProvider` interface, there are no provider-specific `switch`/`if` chains or parsing logic outside the adapters.

## Architecture

A modular monolith: one deployable host (`Api`) composes self-contained modules. Each module owns its `Domain`, `Features` (vertical slices), and `Infrastructure`, and exposes a separate `PublicApi` project for inter-module communication.

```
+---------------------------------------------------------------+
|                     FlightAggregator.Api                       |
|   Host: JSON config + composes modules via IModule             |
|   (RegisterServices + MapEndpoints)                            |
+------------------------------+--------------------------------+
               |                                |
               v                                v
+-----------------------------+   +-----------------------------+
|   Modules.Flights            |   |   Modules.Bookings           |
|  Domain/                     |   |  Domain/                     |
|   - FlightOffer, CabinClass  |   |   - BookingPassenger         |
|   - IFlightProvider          |   |  Features/CreateBooking/     |
|  Features/SearchFlights/     |   |   - Handler + Endpoint       |
|   - Handler (aggregator)     |   |       |                      |
|   - Endpoint                 |   |       | inter-module call    |
|  Infrastructure/             |   |       v                      |
|   - GlobalAirProvider        |   +-------+----------------------+
|   - BudgetWingsProvider      |           |
|   - FlightsModuleApi (impl)  |           |
+--------------+--------------+            |
               | implements                | depends only on
               v                           v
+---------------------------------------------------------------+
|            Modules.Flights.PublicApi (contracts)               |
|   IFlightsModuleApi.FlightExistsAsync(flightId)                |
+---------------------------------------------------------------+

               Shared:  IModule (registration + endpoint mapping)
```

### Projects

- **`src/Shared`** — Cross-cutting `IModule` abstraction (`RegisterServices` + `MapEndpoints`) used by the host to compose modules.
- **`src/Modules/Flights/FlightAggregator.Modules.Flights`** — The Flights module:
  - `Domain/` — `FlightOffer`, `FlightSearchRequest`, `CabinClass`, `IFlightProvider`.
  - `Infrastructure/` — `GlobalAirProvider`, `BudgetWingsProvider` adapters + the **internal** `FlightsModuleApi` implementation.
  - `Features/SearchFlights/` — vertical slice: contracts, `SearchFlightsHandler` (aggregator), endpoint.
  - `FlightsModule.cs` — module registration.
- **`src/Modules/Flights/FlightAggregator.Modules.Flights.PublicApi`** — The Flights module's published contract (`IFlightsModuleApi`). This is the **only** thing other modules may reference.
- **`src/Modules/Bookings/FlightAggregator.Modules.Bookings`** — The Bookings module:
  - `Domain/` — `BookingPassenger`.
  - `Features/CreateBooking/` — vertical slice: contracts, `CreateBookingHandler` (calls `IFlightsModuleApi`), endpoint.
  - `BookingsModule.cs` — module registration.
- **`src/Api`** — Minimal API host. Configures JSON, instantiates the modules, and calls `RegisterServices` / `MapEndpoints` on each.
- **`tests/FlightAggregator.Tests`** — xUnit tests for pricing rules and aggregation behavior.

### Key design decisions

- **Modular monolith** — Each business capability is an isolated module with its own `Domain` / `Features` / `Infrastructure`. A module can later be extracted into a microservice with minimal churn.
- **Vertical slices** — Each use case (`SearchFlights`, `CreateBooking`) bundles its contracts, handler, and endpoint together, so feature changes stay localized.
- **Strict module boundaries via PublicApi** — Modules communicate only through published contracts (`IFlightsModuleApi`). The implementation (`FlightsModuleApi`) is `internal`, so internals never leak across modules.
- **Adapter Pattern** — Providers may expose completely different request/response formats. Each provider normalizes its data into the shared `FlightOffer` model.
- **Aggregator depends only on the contract** — `SearchFlightsHandler` receives `IEnumerable<IFlightProvider>` and never knows which concrete providers exist.
- **Pricing stays local to each provider** — There is no shared pricing engine, since the rules differ per provider. Each adapter exposes a static `CalculatePerPassengerPrice` method (unit-tested directly).
- **Lean stack (no heavy deps)** — Plain handler classes and minimal-API endpoint extensions instead of MediatR/Carter; mocked in-memory data instead of EF Core. These can be introduced later without breaking module boundaries.
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

Adding a provider requires **only** a new adapter inside the Flights module — no changes to the aggregator or endpoints.

1. Create a class in `src/Modules/Flights/FlightAggregator.Modules.Flights/Infrastructure/` implementing `IFlightProvider`:

```csharp
using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Infrastructure;

public sealed class SkyHighProvider : IFlightProvider
{
    public IReadOnlyList<string> KnownFlightIds => ["skyhigh-sh789"];

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

2. Register it in `FlightsModule.RegisterServices`:

```csharp
services.AddSingleton<IFlightProvider, SkyHighProvider>();
```

That's it — the aggregator automatically includes it in every search, and booking validation recognizes its `KnownFlightIds`.

## Adding a New Module

1. Create a module project with `Domain` / `Features` / `Infrastructure` folders (and a `PublicApi` project if other modules need to call it).
2. Add a class implementing `IModule` (`RegisterServices` + `MapEndpoints`).
3. Register the module in `src/Api/Program.cs`:

```csharp
IModule[] modules =
[
    new FlightsModule(),
    new BookingsModule(),
    new YourNewModule()
];
```

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

Success response:
```json
{
  "bookingReference": "BK-9F7A2D"
}
```

The handler validates `flightId` against the Flights module via `IFlightsModuleApi`. An unknown id returns `400`:
```json
"Flight 'nope-999' was not found"
```

## Running

```bash
# Run the API
dotnet run --project src/Api

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
