import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { BusService } from '../../services/bus';
import { Bus } from '../../models/bus';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './search.html',
  styleUrl: './search.scss'
})
export class SearchComponent implements OnInit {
  departureCity: string = '';
  arrivalCity: string = '';
  journeyDate: string = '';
  availableCities: string[] = [];
  buses: Bus[] = [];
  searchPerformed = false;
  loading = false;
  today = new Date().toISOString().split('T')[0]; // Today's date in YYYY-MM-DD format

  constructor(
    private busService: BusService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.busService.getAvailableCities().subscribe({
      next: (cities) => {
        this.availableCities = cities;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading cities:', err);
      }
    });
    
    // Set default date to tomorrow
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.journeyDate = tomorrow.toISOString().split('T')[0];
  }

  searchBuses(): void {
    if (!this.departureCity || !this.arrivalCity || !this.journeyDate) {
      alert('Please fill all fields');
      return;
    }

    this.loading = true;
    this.searchPerformed = true;

    console.log('Searching buses:', {
      from: this.departureCity,
      to: this.arrivalCity,
      date: this.journeyDate
    });

    this.busService.searchBuses(this.departureCity, this.arrivalCity, this.journeyDate).subscribe({
      next: (result) => {
        console.log('Buses loaded successfully:', result);
        this.buses = result;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error searching for buses:', err);
        console.error('Full error:', err);
        alert('Failed to search buses: ' + (err.error?.message || err.message || 'Unknown error'));
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
