import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home';
import { SearchComponent } from './components/search/search';
import { SeatLayoutComponent } from './components/seat-layout/seat-layout';
import { BookingComponent } from './components/booking/booking';
import { ConfirmationComponent } from './components/confirmation/confirmation';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'search', component: SearchComponent },
  { path: 'seat-layout/:id', component: SeatLayoutComponent },
  { path: 'booking/:id', component: BookingComponent },
  { path: 'confirmation/:id', component: ConfirmationComponent },
  { path: '**', redirectTo: '' } // Redirect to home for any unknown routes
];
