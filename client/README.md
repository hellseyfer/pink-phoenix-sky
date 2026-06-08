# Flight Aggregator — Frontend

An **Angular 21** single-page app for searching aggregated flight offers and completing a booking. It consumes the [.NET backend](../README.md) and is built with a modern, signal-first architecture.

## Tech & Conventions

- **Angular 21** standalone components (no NgModules).
- **Signals everywhere** — `signal()`, `computed()`, `input()` / `output()`, and `httpResource` for reactive data fetching.
- **OnPush** change detection on every component.
- **Reactive forms** with cross-field and conditional validation.
- **Native control flow** (`@if`, `@for`) and `class`/`style` bindings (no `ngClass`/`ngStyle`).
- **Lazy-loaded** feature routes.
- Accessibility-minded: labels, `aria-live` regions, focus-visible styles, skip link, responsive table.

## Architecture

The app is organized **by feature (vertical slices)**. Each feature separates a **fetch layer** (`data/`) from a **presentational layer** (`ui/`), composed by a **smart container page**.

```
src/app/
  shared/                       Cross-cutting code
    models/                     flight.models.ts, booking.models.ts
    data/                       airports.ts (6 airports across US + Argentina)
    pipes/                      money-pipe.ts (2-decimal currency formatting)
    state/                      booking-context.ts (signal state bridging features)
  features/
    flights/
      data/                     flight-api.ts          — fetch layer (httpResource)
      ui/
        flight-search-form/     presentational form (origin/dest/date/pax/cabin)
        flight-results-table/   presentational results + front-end sorting
      flights-page.ts           smart container (search → results → select)
    booking/
      data/                     booking-api.ts         — fetch layer (POST /api/bookings)
      ui/
        booking-summary/        presentational flight summary
        price-breakdown/        presentational per-passenger / total
        passenger-form/         presentational form (Passport vs National ID)
      booking-page.ts           smart container (confirm → reference)
```

### Layer responsibilities

- **Fetch layer (`data/`)** — Owns all HTTP. `FlightApi.createSearchResource()` returns a reactive `httpResource` driven by a params signal; `BookingApi.createBooking()` posts a booking.
- **Presentational layer (`ui/`)** — Dumb components. They receive data via `input()` and emit events via `output()`; they contain no HTTP and no routing.
- **Smart containers (`*-page.ts`)** — Wire the fetch layer to presentational components, own page state, and handle navigation.
- **Shared state (`BookingContext`)** — A `providedIn: 'root'` signal store that carries the selected flight (and route) from the flights page to the booking page, and derives `isInternational`.

## Features

### Flight search
- Origin/destination dropdowns (6 airports across 2 countries), departure date, passenger count, and cabin (Economy / Business / First Class).
- Validates required fields and prevents selecting the same origin and destination.

### Flight results
- Table columns: airline provider, flight number, departure, arrival, duration, cabin, and price.
- **Price** shows the **total for all passengers** as primary, with the **per-person** price as secondary text (e.g. `USD 320.00 total / USD 160.00 per person`) via the `money` pipe (two decimals).
- **Front-end sorting**: price (low→high, high→low), duration (shortest first), departure time (earliest first).
- Loading spinner while a search is in progress; a **Select** button per row starts the booking flow.

### Booking flow
- Summary of the selected flight (route, provider, times, cabin).
- Price breakdown: per-passenger price × number of passengers = total.
- Passenger details form (full name, email, document number) — one set of fields per passenger.
- **Domestic vs international** is derived from the airports' country codes:
  - **International** → field labelled **Passport Number** (6–9 letters/digits).
  - **Domestic** → field labelled **National ID** (7–10 digits).
  - Both the label and the validation rule change with the route.
- **Confirm booking** posts to the backend and displays the returned **booking reference**.

## Running locally

The frontend talks to the backend through a dev proxy, so the backend must be running first.

```bash
# 1. Start the backend (from the repo root) — listens on http://localhost:5000
dotnet run --project src/Api

# 2. Start the frontend (from this client/ folder) — http://localhost:4200
npm install
npm start
```

Requests to `/api/*` are proxied to `http://localhost:5000` via `proxy.conf.json`, so no backend CORS configuration is required.

## Build

```bash
npm run build
```

Build artifacts are emitted to `dist/`. Each feature route is emitted as a separate lazy chunk.

## Tests

Unit tests run with the [Vitest](https://vitest.dev/) runner:

```bash
ng test
```
