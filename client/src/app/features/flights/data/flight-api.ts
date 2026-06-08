import { Injectable, Signal } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { FlightOffer, FlightSearchParams } from '../../../shared/models/flight.models';

@Injectable({ providedIn: 'root' })
export class FlightApi {
  /**
   * Creates a reactive resource that re-fetches whenever the params signal changes.
   * When params is null no request is sent (idle state).
   */
  createSearchResource(params: Signal<FlightSearchParams | null>) {
    return httpResource<FlightOffer[]>(
      () => {
        const value = params();
        if (!value) {
          return undefined;
        }
        return {
          url: '/api/flights/search',
          method: 'POST',
          body: value,
        };
      },
      { defaultValue: [] },
    );
  }
}
