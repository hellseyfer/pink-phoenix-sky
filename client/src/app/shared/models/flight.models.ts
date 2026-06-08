export type CabinClass = 'Economy' | 'Business' | 'First';

export interface Airport {
  readonly code: string;
  readonly name: string;
  readonly city: string;
  readonly country: string;
  readonly countryCode: string;
}

export interface FlightSearchParams {
  readonly origin: string;
  readonly destination: string;
  readonly departureDate: string;
  readonly passengers: number;
  readonly cabinClass: CabinClass;
}

export interface FlightOffer {
  readonly id: string;
  readonly provider: string;
  readonly flightNumber: string;
  readonly origin: string;
  readonly destination: string;
  readonly departureTime: string;
  readonly arrivalTime: string;
  readonly durationMinutes: number;
  readonly cabinClass: CabinClass;
  readonly pricePerPassenger: number;
  readonly totalPrice: number;
  readonly currency: string;
}

export const CABIN_OPTIONS: ReadonlyArray<{ value: CabinClass; label: string }> = [
  { value: 'Economy', label: 'Economy' },
  { value: 'Business', label: 'Business' },
  { value: 'First', label: 'First Class' },
];

export function cabinLabel(cabin: CabinClass): string {
  return CABIN_OPTIONS.find((option) => option.value === cabin)?.label ?? cabin;
}
