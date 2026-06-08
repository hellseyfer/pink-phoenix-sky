import { Airport } from '../models/flight.models';

export const AIRPORTS: ReadonlyArray<Airport> = [
  { code: 'JFK', name: 'John F. Kennedy International', city: 'New York', country: 'United States', countryCode: 'US' },
  { code: 'LAX', name: 'Los Angeles International', city: 'Los Angeles', country: 'United States', countryCode: 'US' },
  { code: 'ORD', name: "O'Hare International", city: 'Chicago', country: 'United States', countryCode: 'US' },
  { code: 'EZE', name: 'Ministro Pistarini International', city: 'Buenos Aires', country: 'Argentina', countryCode: 'AR' },
  { code: 'COR', name: 'Ingeniero Taravella International', city: 'Córdoba', country: 'Argentina', countryCode: 'AR' },
  { code: 'MDZ', name: 'El Plumerillo International', city: 'Mendoza', country: 'Argentina', countryCode: 'AR' },
];

export function findAirport(code: string): Airport | undefined {
  return AIRPORTS.find((airport) => airport.code === code);
}
