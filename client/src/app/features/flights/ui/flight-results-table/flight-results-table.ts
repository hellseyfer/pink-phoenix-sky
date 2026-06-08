import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { FlightOffer, cabinLabel } from '../../../../shared/models/flight.models';
import { MoneyPipe } from '../../../../shared/pipes/money-pipe';

export type SortKey = 'price-asc' | 'price-desc' | 'duration-asc' | 'departure-asc';

interface OfferRow {
  readonly offer: FlightOffer;
  readonly cabin: string;
  readonly departure: string;
  readonly arrival: string;
  readonly duration: string;
}

const TIME_FORMAT: Intl.DateTimeFormatOptions = {
  hour: '2-digit',
  minute: '2-digit',
  hour12: false,
  timeZone: 'UTC',
};

function formatTime(iso: string): string {
  return new Date(iso).toLocaleTimeString('en-GB', TIME_FORMAT);
}

function formatDuration(minutes: number): string {
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return `${hours}h ${mins.toString().padStart(2, '0')}m`;
}

const COMPARATORS: Record<SortKey, (a: FlightOffer, b: FlightOffer) => number> = {
  'price-asc': (a, b) => a.totalPrice - b.totalPrice,
  'price-desc': (a, b) => b.totalPrice - a.totalPrice,
  'duration-asc': (a, b) => a.durationMinutes - b.durationMinutes,
  'departure-asc': (a, b) =>
    new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime(),
};

@Component({
  selector: 'app-flight-results-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MoneyPipe],
  templateUrl: './flight-results-table.html',
  styleUrl: './flight-results-table.css',
})
export class FlightResultsTable {
  readonly offers = input.required<ReadonlyArray<FlightOffer>>();
  readonly loading = input(false);
  readonly searched = input(false);
  readonly passengers = input(1);
  readonly select = output<FlightOffer>();

  readonly sortKey = signal<SortKey>('price-asc');

  readonly sortOptions: ReadonlyArray<{ value: SortKey; label: string }> = [
    { value: 'price-asc', label: 'Price: low to high' },
    { value: 'price-desc', label: 'Price: high to low' },
    { value: 'duration-asc', label: 'Duration: shortest first' },
    { value: 'departure-asc', label: 'Departure: earliest first' },
  ];

  readonly rows = computed<OfferRow[]>(() => {
    const sorted = [...this.offers()].sort(COMPARATORS[this.sortKey()]);
    return sorted.map((offer) => ({
      offer,
      cabin: cabinLabel(offer.cabinClass),
      departure: formatTime(offer.departureTime),
      arrival: formatTime(offer.arrivalTime),
      duration: formatDuration(offer.durationMinutes),
    }));
  });

  protected onSortChange(value: string): void {
    this.sortKey.set(value as SortKey);
  }
}
