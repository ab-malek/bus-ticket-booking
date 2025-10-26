import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Booking } from '../models/booking';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = 'http://localhost:5122/api/booking';
  private isBrowser: boolean;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  /**
   * Create a new booking
   */
  createBooking(booking: Booking): Observable<Booking> {
    // The backend API expects to book one seat at a time with BookSeatInputDto
    // We'll book the first passenger and first seat
    // Note: For multiple seats, you'd need to loop and call the API multiple times
    const firstPassenger = booking.passengers[0];
    
    // Get the first selected seat from sessionStorage (only in browser)
    let selectedSeats: any[] = [];
    if (this.isBrowser) {
      selectedSeats = JSON.parse(sessionStorage.getItem('selectedSeats') || '[]');
    }
    
    if (selectedSeats.length === 0) {
      throw new Error('No seats selected');
    }
    
    const firstSeat = selectedSeats[0];
    
    // Map to the exact DTO structure expected by backend
    const bookingRequest = {
      busScheduleId: booking.busId,
      seatId: firstSeat.id, // The actual seat GUID from the backend
      passengerName: firstPassenger.name,
      mobileNumber: booking.contactPhone,
      email: booking.contactEmail,
      boardingPoint: booking.departureCity,
      droppingPoint: booking.arrivalCity
    };

    console.log('Booking request:', bookingRequest);

    return this.http.post<any>(`${this.apiUrl}/book-seat`, bookingRequest).pipe(
      map(response => {
        console.log('Booking response:', response);
        // Backend now returns simple { message: "Booking confirmed" }
        return {
          ...booking,
          id: Date.now(), // Frontend generated ID
          bookingDate: new Date(),
          bookingStatus: 'CONFIRMED',
          bookingReference: 'BKG-' + Date.now(), // Generate reference in frontend
          totalFare: booking.totalFare
        };
      })
    );
  }

  /**
   * Get booking by ID
   * Note: This endpoint doesn't exist in the current backend
   * You would need to add it if you want to retrieve bookings
   */
  getBookingById(id: number): Observable<Booking | undefined> {
    // This would need a backend endpoint like GET /api/booking/{id}
    // For now, return undefined
    return new Observable(observer => {
      observer.next(undefined);
      observer.complete();
    });
  }

  /**
   * Get all bookings (for demo purposes)
   * Note: This endpoint doesn't exist in the current backend
   */
  getAllBookings(): Observable<Booking[]> {
    // This would need a backend endpoint like GET /api/booking
    // For now, return empty array
    return new Observable(observer => {
      observer.next([]);
      observer.complete();
    });
  }
}
