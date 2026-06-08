import { Injectable, computed, signal } from '@angular/core';
import { Airport, FlightOffer } from '../models/flight.models';

export interface BookingSelection {
  readonly offer: FlightOffer;
  readonly passengers: number;
  readonly origin: Airport;
  readonly destination: Airport;
}

@Injectable({ providedIn: 'root' })
export class BookingContext {
  private readonly _selection = signal<BookingSelection | null>(null);

  readonly selection = this._selection.asReadonly();

  readonly isInternational = computed(() => {
    const selection = this._selection();
    return selection ? selection.origin.countryCode !== selection.destination.countryCode : false;
  });

  select(selection: BookingSelection): void {
    this._selection.set(selection);
  }

  clear(): void {
    this._selection.set(null);
  }
}
