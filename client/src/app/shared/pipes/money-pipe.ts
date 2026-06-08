import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'money' })
export class MoneyPipe implements PipeTransform {
  transform(value: number | null | undefined, currency = 'USD'): string {
    const amount = typeof value === 'number' ? value : 0;
    return `${currency} ${amount.toFixed(2)}`;
  }
}
