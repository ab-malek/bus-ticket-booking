import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BusService } from '../../services/bus';
import { BookingService } from '../../services/booking';
import { Bus, Seat } from '../../models/bus';
import { Booking, Passenger } from '../../models/booking';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './booking.html',
  styleUrl: './booking.scss'
})
export class BookingComponent implements OnInit {
  busId: string = ''; // Changed from number to string
  bus: Bus | undefined;
  selectedSeats: Seat[] = [];
  passengers: Passenger[] = [];
  contactEmail: string = '';
  contactPhone: string = '';
  loading = true;
  private isBrowser: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private busService: BusService,
    private bookingService: BookingService,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.busId = params['id']; // Get as string (GUID)
      this.loadBusDetails();
      this.loadSelectedSeats();
    });
  }

  loadBusDetails(): void {
    this.busService.getBusById(this.busId).subscribe(bus => {
      this.bus = bus;
      this.loading = false;
    });
  }

  loadSelectedSeats(): void {
    // Only access sessionStorage in browser environment
    if (!this.isBrowser) {
      return;
    }

    const storedSeats = sessionStorage.getItem('selectedSeats');
    if (storedSeats) {
      this.selectedSeats = JSON.parse(storedSeats);
      // Initialize passenger info for each selected seat
      this.selectedSeats.forEach(seat => {
        this.passengers.push({
          name: '',
          age: 0,
          gender: '',
          seatNumber: seat.seatNumber
        });
      });
    } else {
      // If no seats were selected, go back to seat selection
      this.router.navigate(['/seat-layout', this.busId]);
    }
  }

  confirmBooking(): void {
    // Validate passenger details
    const invalidPassengers = this.passengers.filter(p =>
      !p.name || p.age <= 0 || !p.gender
    );

    if (invalidPassengers.length > 0) {
      alert('Please fill all passenger details');
      return;
    }

    if (!this.contactEmail || !this.contactPhone) {
      alert('Please provide contact details');
      return;
    }

    // Create booking
    if (this.bus) {
      const totalFare = this.bus.fare * this.selectedSeats.length;

      const booking: Booking = {
        busId: this.busId,
        busName: this.bus.busName,
        journeyDate: this.bus.journeyDate,
        departureTime: this.bus.departureTime,
        arrivalTime: this.bus.arrivalTime,
        departureCity: this.bus.departureCity,
        arrivalCity: this.bus.arrivalCity,
        passengers: this.passengers,
        totalFare,
        contactEmail: this.contactEmail,
        contactPhone: this.contactPhone
      };

      this.bookingService.createBooking(booking).subscribe({
        next: (newBooking) => {
          // Save booking to sessionStorage for confirmation page (only in browser)
          if (this.isBrowser) {
            sessionStorage.setItem('lastBooking', JSON.stringify(newBooking));
            sessionStorage.removeItem('selectedSeats');
          }

          // Navigate to confirmation page
          this.router.navigate(['/confirmation', newBooking.id]);
        },
        error: (err) => {
          console.error('Booking error:', err);
          alert('Failed to book seat. Please try again. Error: ' + (err.error?.message || err.message));
        }
      });
    }
  }
}
