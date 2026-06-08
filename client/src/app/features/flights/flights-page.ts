import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AIRPORTS, findAirport } from '../../shared/data/airports';
import { FlightOffer, FlightSearchParams } from '../../shared/models/flight.models';
import { BookingContext } from '../../shared/state/booking-context';
import { FlightApi } from './data/flight-api';
import { FlightSearchForm } from './ui/flight-search-form/flight-search-form';
import { FlightResultsTable } from './ui/flight-results-table/flight-results-table';

@Component({
  selector: 'app-flights-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FlightSearchForm, FlightResultsTable],
  templateUrl: './flights-page.html',
})
export class FlightsPage {
  private readonly flightApi = inject(FlightApi);
  private readonly bookingContext = inject(BookingContext);
  private readonly router = inject(Router);

  readonly airports = AIRPORTS;

  private readonly params = signal<FlightSearchParams | null>(null);
  private readonly searchResource = this.flightApi.createSearchResource(this.params);

  protected readonly offers = this.searchResource.value;
  protected readonly loading = this.searchResource.isLoading;
  protected readonly hasError = computed(() => this.searchResource.error() !== undefined);
  protected readonly searched = computed(() => this.params() !== null);
  protected readonly passengers = computed(() => this.params()?.passengers ?? 1);

  protected onSearch(params: FlightSearchParams): void {
    this.params.set(params);
  }

  protected onSelect(offer: FlightOffer): void {
    const params = this.params();
    const origin = findAirport(offer.origin) ?? (params ? findAirport(params.origin) : undefined);
    const destination =
      findAirport(offer.destination) ?? (params ? findAirport(params.destination) : undefined);

    if (!origin || !destination) {
      return;
    }

    this.bookingContext.select({
      offer,
      passengers: params?.passengers ?? 1,
      origin,
      destination,
    });
    void this.router.navigate(['/booking']);
  }
}
