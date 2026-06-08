export interface PassengerDetails {
  readonly fullName: string;
  readonly email: string;
  readonly documentNumber: string;
}

export interface CreateBookingRequest {
  readonly flightId: string;
  readonly passengers: ReadonlyArray<PassengerDetails>;
}

export interface BookingResponse {
  readonly bookingReference: string;
}
