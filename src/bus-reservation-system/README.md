# Bus Ticket Reservation System

A simple Angular application that allows users to search for available buses between cities, view seat layouts, and book tickets with passenger details.

## Features

- **Bus Search**: Search for buses between two cities on a specific date
- **Seat Selection**: Interactive seat layout for selecting seats
- **Booking System**: Enter passenger details and confirm bookings
- **E-Ticket**: Generate and print e-tickets with booking details

## Technology Stack

- Angular 17+
- Bootstrap 5
- Standalone Components
- TypeScript
- SCSS

## Project Structure

- **Components**:

  - `HomeComponent`: Landing page with introduction and call-to-action
  - `SearchComponent`: Search form for finding buses between cities
  - `SeatLayoutComponent`: Interactive seat layout for selecting seats
  - `BookingComponent`: Form for entering passenger details
  - `ConfirmationComponent`: Booking confirmation and e-ticket generation
  - `HeaderComponent`: Application header with navigation
  - `FooterComponent`: Application footer

- **Services**:

  - `BusService`: Handles bus search and seat layout operations
  - `BookingService`: Manages booking operations

- **Models**:
  - `Bus`: Interface for bus details
  - `Seat` & `SeatLayout`: Interfaces for seat selection functionality
  - `Booking` & `Passenger`: Interfaces for booking data

## Installation & Setup

1. **Clone the repository**:

   ```bash
   git clone <repository-url>
   cd bus-reservation-system
   ```

2. **Install dependencies**:

   ```bash
   npm install
   ```

3. **Run the development server**:

   ```bash
   ng serve
   ```

4. **Open your browser** and navigate to `http://localhost:4200/`.

## Build

To build the project for production:

```bash
ng build
```

The build artifacts will be stored in the `dist/` directory.

## Note

This is a frontend-only application with mocked data services. In a real-world scenario, these services would connect to a backend API to fetch and persist data.

## Future Enhancements

- User authentication and registration
- Payment gateway integration
- Bus operator login for managing buses and bookings
- Booking history and cancellation features
- Email notifications for bookings
