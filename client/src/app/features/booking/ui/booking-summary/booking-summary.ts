import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { cabinLabel } from '../../../../shared/models/flight.models';
import { BookingSelection } from '../../../../shared/state/booking-context';

const DATE_FORMAT: Intl.DateTimeFormatOptions = {
  weekday: 'short',
  day: '2-digit',
  month: 'short',
  hour: '2-digit',
  minute: '2-digit',
  hour12: false,
  timeZone: 'UTC',
};

@Component({
  selector: 'app-booking-summary',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="card" aria-label="Selected flight summary">
      <h2 class="card__title">Your flight</h2>
      <div class="route">
        <span class="route__code">{{ selection().origin.code }}</span>
        <span class="route__arrow" aria-hidden="true">→</span>
        <span class="route__code">{{ selection().destination.code }}</span>
        @if (isInternational()) {
          <span class="badge badge--intl">International</span>
        } @else {
          <span class="badge badge--domestic">Domestic</span>
        }
      </div>
      <p class="route__cities text-muted">
        {{ selection().origin.city }} to {{ selection().destination.city }}
      </p>

      <dl class="summary-grid">
        <div>
          <dt>Airline</dt>
          <dd>{{ selection().offer.provider }} · {{ selection().offer.flightNumber }}</dd>
        </div>
        <div>
          <dt>Departs</dt>
          <dd>{{ departs() }}</dd>
        </div>
        <div>
          <dt>Arrives</dt>
          <dd>{{ arrives() }}</dd>
        </div>
        <div>
          <dt>Cabin</dt>
          <dd>{{ cabin() }}</dd>
        </div>
      </dl>
    </section>
  `,
  styleUrl: './booking-summary.css',
})
export class BookingSummary {
  readonly selection = input.required<BookingSelection>();
  readonly isInternational = input(false);

  protected readonly cabin = computed(() => cabinLabel(this.selection().offer.cabinClass));
  protected readonly departs = computed(() => this.formatDate(this.selection().offer.departureTime));
  protected readonly arrives = computed(() => this.formatDate(this.selection().offer.arrivalTime));

  private formatDate(iso: string): string {
    return new Date(iso).toLocaleString('en-GB', DATE_FORMAT);
  }
}
