export interface Bus {
  id: string; // Changed from number to string to support GUID from backend
  busName: string;
  busNumber: string;
  departureCity: string;
  arrivalCity: string;
  departureTime: string;
  arrivalTime: string;
  journeyDate: string;
  availableSeats: number;
  totalSeats: number;
  fare: number;
  busType: string; // AC, Non-AC, etc.
}

export interface Seat {
  id: string; // Changed to string
  seatNumber: string;
  isBooked: boolean;
  isSelected: boolean;
  isSold?: boolean; // Optional property for sold seats
}

export interface SeatLayout {
  busId: string; // Changed from number to string
  seats: Seat[];
  totalRows: number;
  totalColumns: number;
}
