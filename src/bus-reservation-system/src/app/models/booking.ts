export interface Passenger {
  id?: number;
  name: string;
  age: number;
  gender: string;
  seatNumber: string;
}

export interface Booking {
  id?: number;
  busId: string; // Changed from number to string to support GUID
  busName?: string;
  journeyDate: string;
  departureTime?: string;
  arrivalTime?: string;
  departureCity: string;
  arrivalCity: string;
  passengers: Passenger[];
  totalFare: number;
  bookingDate?: Date;
  bookingStatus?: string;
  bookingReference?: string;
  totalAmount?: number;
  contactEmail: string;
  contactPhone: string;
}
