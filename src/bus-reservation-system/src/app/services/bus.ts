import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Bus, Seat, SeatLayout } from '../models/bus';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BusService {
  private apiUrl = 'http://localhost:5122/api';

  constructor(private http: HttpClient) { }

  /**
   * Search for buses between two cities on a specific date
   */
  searchBuses(departureCity: string, arrivalCity: string, journeyDate: string): Observable<Bus[]> {
    const params = new HttpParams()
      .set('from', departureCity)
      .set('to', arrivalCity)
      .set('journeyDate', journeyDate);

    return this.http.get<any[]>(`${this.apiUrl}/search/buses`, { params }).pipe(
      map(buses => buses.map(bus => ({
        id: bus.busScheduleId,
        busName: bus.busName,
        busNumber: bus.companyName,
        departureCity: bus.boardingPoint,
        arrivalCity: bus.droppingPoint,
        departureTime: this.formatTime(bus.departureTime),
        arrivalTime: this.formatTime(bus.arrivalTime),
        journeyDate: bus.journeyDate.split('T')[0],
        availableSeats: bus.seatsLeft,
        totalSeats: bus.totalSeats,
        fare: bus.price,
        busType: bus.busType
      })))
    );
  }

  /**
   * Get bus details by ID (BusScheduleId)
   */
  getBusById(id: string): Observable<Bus> {
    console.log('BusService.getBusById - Request ID:', id);
    return this.http.get<any>(`${this.apiUrl}/search/buses/${id}`).pipe(
      map(dto => {
        console.log('BusService.getBusById - Raw DTO:', dto);
        const bus: Bus = {
          id: dto.busScheduleId,
          busName: dto.busName || dto.companyName || 'Bus',
          busNumber: dto.busNumber || dto.busName || 'N/A',
          departureCity: dto.boardingPoint,
          arrivalCity: dto.droppingPoint,
          departureTime: this.formatTimeSpan(dto.departureTime),
          arrivalTime: this.formatTimeSpan(dto.arrivalTime),
          journeyDate: this.formatDate(dto.journeyDate),
          availableSeats: dto.availableSeats || dto.seatsLeft || 0,
          totalSeats: dto.totalSeats,
          fare: dto.fare || dto.price || 0,
          busType: dto.busType || 'Standard'
        };
        console.log('BusService.getBusById - Mapped Bus:', bus);
        return bus;
      })
    );
  }

  /**
   * Get seat layout for a specific bus
   */
  getSeatLayout(busScheduleId: string): Observable<SeatLayout> {
    console.log('BusService.getSeatLayout - Request ID:', busScheduleId);
    return this.http.get<any>(`${this.apiUrl}/booking/seat-plan/${busScheduleId}`).pipe(
      map(dto => {
        console.log('BusService.getSeatLayout - Raw DTO:', dto);
        const seats = dto.seats
          .map((seat: any) => ({
            id: seat.seatId, // Use the actual GUID from backend
            seatNumber: seat.seatNumber,
            isBooked: seat.status !== 0,
            isSelected: false,
            isSold: seat.status === 2
          }))
          // Sort seats by seat number (convert to number for proper sorting)
          .sort((a: any, b: any) => {
            const numA = parseInt(a.seatNumber, 10);
            const numB = parseInt(b.seatNumber, 10);
            return numA - numB;
          });
        
        const layout: SeatLayout = {
          busId: dto.busScheduleId,
          seats: seats,
          totalRows: Math.ceil(dto.seats.length / 4), // Assuming 4 seats per row (2-2 configuration)
          totalColumns: 4
        };
        console.log('BusService.getSeatLayout - Mapped Layout:', layout);
        return layout;
      })
    );
  }

  /**
   * Get available cities (for dropdown options)
   */
  getAvailableCities(): Observable<string[]> {
    // This would need a specific endpoint on the backend
    // For now, return hardcoded cities
    return new Observable(observer => {
      observer.next(['New York', 'Boston', 'Washington DC', 'Philadelphia', 'Chicago']);
      observer.complete();
    });
  }

  /**
   * Helper method to format time from "HH:mm:ss" to "HH:mm AM/PM"
   */
  private formatTime(time: string): string {
    const [hours, minutes] = time.split(':');
    const hour = parseInt(hours);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const displayHour = hour % 12 || 12;
    return `${displayHour}:${minutes} ${ampm}`;
  }

  /**
   * Helper method to format TimeSpan from backend (e.g., "08:00:00")
   */
  private formatTimeSpan(timeSpan: any): string {
    if (typeof timeSpan === 'string') {
      return this.formatTime(timeSpan);
    }
    // If it's an object, it might be in format { hours: 8, minutes: 0, seconds: 0 }
    return '00:00 AM';
  }

  /**
   * Helper method to format DateTime from backend
   */
  private formatDate(date: any): string {
    if (typeof date === 'string') {
      return date.split('T')[0];
    }
    return new Date(date).toISOString().split('T')[0];
  }
}
