import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MoneyPipe } from '../../../../shared/pipes/money-pipe';

@Component({
  selector: 'app-price-breakdown',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MoneyPipe],
  template: `
    <section class="card" aria-label="Price breakdown">
      <h2 class="card__title">Price breakdown</h2>
      <dl class="breakdown">
        <div class="breakdown__row">
          <dt>Price per passenger</dt>
          <dd>{{ pricePerPassenger() | money: currency() }}</dd>
        </div>
        <div class="breakdown__row">
          <dt>Passengers</dt>
          <dd>&times; {{ passengers() }}</dd>
        </div>
        <div class="breakdown__row breakdown__row--total">
          <dt>Total</dt>
          <dd>{{ total() | money: currency() }}</dd>
        </div>
      </dl>
    </section>
  `,
  styleUrl: './price-breakdown.css',
})
export class PriceBreakdown {
  readonly pricePerPassenger = input.required<number>();
  readonly passengers = input.required<number>();
  readonly total = input.required<number>();
  readonly currency = input('USD');
}
