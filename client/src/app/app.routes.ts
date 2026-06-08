import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'flights' },
  {
    path: 'flights',
    loadComponent: () =>
      import('./features/flights/flights-page').then((m) => m.FlightsPage),
  },
  {
    path: 'booking',
    loadComponent: () =>
      import('./features/booking/booking-page').then((m) => m.BookingPage),
  },
  { path: '**', redirectTo: 'flights' },
];
