import { Component, OnInit, PLATFORM_ID, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BusService } from '../../services/bus';
import { Bus, Seat, SeatLayout } from '../../models/bus';

@Component({
  selector: 'app-seat-layout',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './seat-layout.html',
  styleUrl: './seat-layout.scss'
})
export class SeatLayoutComponent implements OnInit {
  busId: string = ''; // Changed from number to string
  bus: Bus | undefined;
  seatLayout: SeatLayout | undefined;
  selectedSeats: Seat[] = [];
  loading = true;
  private isBrowser: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private busService: BusService,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.busId = params['id']; // Get as string (GUID)
      console.log('Seat Layout - Bus ID:', this.busId);
      this.loadBusDetails();
      this.loadSeatLayout();
    });
  }

  loadBusDetails(): void {
    console.log('Loading bus details...');
    this.busService.getBusById(this.busId).subscribe({
      next: (bus) => {
        console.log('Bus details received:', bus);
        this.bus = bus;
        this.cdr.detectChanges(); // Manually trigger change detection
      },
      error: (err) => {
        console.error('Error loading bus details:', err);
        console.error('Full error:', err);
        alert('Error: ' + (err.error?.message || err.message || 'Failed to load bus details'));
      }
    });
  }

  loadSeatLayout(): void {
    console.log('Loading seat layout...');
    this.busService.getSeatLayout(this.busId).subscribe({
      next: (layout) => {
        console.log('Seat layout received:', layout);
        this.seatLayout = layout;
        this.loading = false;
        this.cdr.detectChanges(); // Manually trigger change detection
      },
      error: (err) => {
        console.error('Error loading seat layout:', err);
        console.error('Full error:', err);
        this.loading = false;
        alert('Error: ' + (err.error?.message || err.message || 'Failed to load seat layout'));
      }
    });
  }

  toggleSeatSelection(seat: Seat): void {
    if (seat.isBooked) {
      return; // Can't select booked seats
    }

    seat.isSelected = !seat.isSelected;

    if (seat.isSelected) {
      this.selectedSeats.push(seat);
    } else {
      this.selectedSeats = this.selectedSeats.filter(s => s.id !== seat.id);
    }
  }

  isSeatSelected(seat: Seat): boolean {
    return seat.isSelected;
  }

  /**
   * Arrange seats in rows of 4 (2-2 configuration)
   * This creates a bus-like layout with an aisle in the middle
   */
  getSeatsInRows(): Seat[][] {
    if (!this.seatLayout || !this.seatLayout.seats) {
      return [];
    }

    const rows: Seat[][] = [];
    const seatsPerRow = 4;
    
    for (let i = 0; i < this.seatLayout.seats.length; i += seatsPerRow) {
      rows.push(this.seatLayout.seats.slice(i, i + seatsPerRow));
    }

    return rows;
  }

  proceedToBooking(): void {
    if (this.selectedSeats.length === 0) {
      alert('Please select at least one seat to continue.');
      return;
    }

    // Store selected seats in session storage (only in browser)
    if (this.isBrowser) {
      sessionStorage.setItem('selectedSeats', JSON.stringify(this.selectedSeats));
    }
    this.router.navigate(['/booking', this.busId]);
  }
}
