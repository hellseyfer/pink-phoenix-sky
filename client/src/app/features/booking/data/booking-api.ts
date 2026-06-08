import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookingResponse, CreateBookingRequest } from '../../../shared/models/booking.models';

@Injectable({ providedIn: 'root' })
export class BookingApi {
  private readonly http = inject(HttpClient);

  createBooking(request: CreateBookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>('/api/bookings', request);
  }
}
