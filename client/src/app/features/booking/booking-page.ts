import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { PassengerDetails } from '../../shared/models/booking.models';
import { BookingContext } from '../../shared/state/booking-context';
import { BookingApi } from './data/booking-api';
import { BookingSummary } from './ui/booking-summary/booking-summary';
import { PriceBreakdown } from './ui/price-breakdown/price-breakdown';
import { PassengerForm } from './ui/passenger-form/passenger-form';

@Component({
  selector: 'app-booking-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [BookingSummary, PriceBreakdown, PassengerForm],
  templateUrl: './booking-page.html',
})
export class BookingPage {
  private readonly bookingContext = inject(BookingContext);
  private readonly bookingApi = inject(BookingApi);
  private readonly router = inject(Router);

  protected readonly selection = this.bookingContext.selection;
  protected readonly isInternational = this.bookingContext.isInternational;

  protected readonly submitting = signal(false);
  protected readonly bookingReference = signal<string | null>(null);
  protected readonly error = signal<string | null>(null);

  constructor() {
    if (!this.selection()) {
      void this.router.navigate(['/flights']);
    }
  }

  protected onConfirm(passengers: PassengerDetails[]): void {
    const selection = this.selection();
    if (!selection) {
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    this.bookingApi.createBooking({ flightId: selection.offer.id, passengers }).subscribe({
      next: (response) => {
        this.bookingReference.set(response.bookingReference);
        this.submitting.set(false);
      },
      error: () => {
        this.error.set('We could not complete your booking. Please try again.');
        this.submitting.set(false);
      },
    });
  }

  protected startNewSearch(): void {
    this.bookingContext.clear();
    void this.router.navigate(['/flights']);
  }
}
