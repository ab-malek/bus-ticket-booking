import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Booking } from '../../models/booking';

@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './confirmation.html',
  styleUrl: './confirmation.scss'
})
export class ConfirmationComponent implements OnInit {
  bookingId: number = 0;
  booking: Booking | undefined;
  loading = false;
  private isBrowser: boolean;

  constructor(
    private route: ActivatedRoute,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.bookingId = +params['id']; // Convert to number
      
      // Try to get booking from sessionStorage (set by booking component)
      if (this.isBrowser) {
        const storedBooking = sessionStorage.getItem('lastBooking');
        if (storedBooking) {
          this.booking = JSON.parse(storedBooking);
        }
      }
    });
  }

  printTicket(): void {
    window.print();
  }
}
