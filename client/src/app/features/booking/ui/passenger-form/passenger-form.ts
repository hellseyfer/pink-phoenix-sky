import { ChangeDetectionStrategy, Component, OnInit, computed, inject, input, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PassengerDetails } from '../../../../shared/models/booking.models';

const PASSPORT_PATTERN = /^[A-Za-z0-9]{6,9}$/;
const NATIONAL_ID_PATTERN = /^[0-9]{7,10}$/;

@Component({
  selector: 'app-passenger-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  templateUrl: './passenger-form.html',
  styleUrl: './passenger-form.css',
})
export class PassengerForm implements OnInit {
  private readonly fb = inject(FormBuilder);

  readonly passengers = input.required<number>();
  readonly isInternational = input(false);
  readonly pending = input(false);
  readonly submitForm = output<PassengerDetails[]>();

  protected readonly form = this.fb.array<FormGroup>([]);

  protected readonly documentLabel = computed(() =>
    this.isInternational() ? 'Passport Number' : 'National ID',
  );
  protected readonly documentHint = computed(() =>
    this.isInternational()
      ? '6–9 letters or digits.'
      : '7–10 digits.',
  );

  ngOnInit(): void {
    const documentValidators = this.isInternational()
      ? [Validators.required, Validators.pattern(PASSPORT_PATTERN)]
      : [Validators.required, Validators.pattern(NATIONAL_ID_PATTERN)];

    for (let i = 0; i < this.passengers(); i++) {
      this.form.push(
        this.fb.group({
          fullName: ['', [Validators.required, Validators.minLength(2)]],
          email: ['', [Validators.required, Validators.email]],
          documentNumber: ['', documentValidators],
        }),
      );
    }
  }

  protected get groups(): FormGroup[] {
    return this.form.controls as FormGroup[];
  }

  protected showError(group: FormGroup, controlName: string): boolean {
    const control = group.get(controlName);
    return !!control && control.invalid && (control.touched || control.dirty);
  }

  protected documentError(group: FormGroup): string | null {
    const control = group.get('documentNumber');
    if (!control || !this.showError(group, 'documentNumber')) {
      return null;
    }
    if (control.hasError('required')) {
      return `${this.documentLabel()} is required.`;
    }
    if (control.hasError('pattern')) {
      return this.isInternational()
        ? 'Enter a valid passport number (6–9 letters or digits).'
        : 'Enter a valid national ID (7–10 digits).';
    }
    return null;
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitForm.emit(this.form.getRawValue() as PassengerDetails[]);
  }
}
