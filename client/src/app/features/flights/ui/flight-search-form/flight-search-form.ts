import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import {
  Airport,
  CABIN_OPTIONS,
  CabinClass,
  FlightSearchParams,
} from '../../../../shared/models/flight.models';

function differentAirports(group: AbstractControl): ValidationErrors | null {
  const origin = group.get('origin')?.value;
  const destination = group.get('destination')?.value;
  return origin && destination && origin === destination ? { sameAirport: true } : null;
}

@Component({
  selector: 'app-flight-search-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  templateUrl: './flight-search-form.html',
  styleUrl: './flight-search-form.css',
})
export class FlightSearchForm {
  private readonly fb = inject(FormBuilder);

  readonly airports = input.required<ReadonlyArray<Airport>>();
  readonly pending = input(false);
  readonly search = output<FlightSearchParams>();

  readonly cabinOptions = CABIN_OPTIONS;
  readonly minDate = new Date().toISOString().slice(0, 10);

  readonly form = this.fb.nonNullable.group(
    {
      origin: ['', Validators.required],
      destination: ['', Validators.required],
      departureDate: [this.minDate, Validators.required],
      passengers: [1, [Validators.required, Validators.min(1), Validators.max(9)]],
      cabinClass: ['Economy' as CabinClass, Validators.required],
    },
    { validators: differentAirports },
  );

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.search.emit({
      origin: value.origin,
      destination: value.destination,
      departureDate: value.departureDate,
      passengers: Number(value.passengers),
      cabinClass: value.cabinClass,
    });
  }

  protected showError(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!control && control.invalid && (control.touched || control.dirty);
  }
}
