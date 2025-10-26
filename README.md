# Bus Ticket Reservation System

A full-stack web application for bus ticket booking with real-time seat selection, built with Angular and .NET Core.

## 🚀 Features

- **Real-time Bus Search** - Search buses by route and date
- **Interactive Seat Selection** - Visual 2-2 bus layout with seat availability
- **Passenger Management** - Multi-passenger booking support
- **Booking Confirmation** - E-ticket generation
- **Responsive Design** - Works on desktop and mobile devices
- **RESTful API** - Clean architecture with Entity Framework Core
- **PostgreSQL Database** - Reliable data persistence

## 🛠️ Tech Stack

### Frontend

- **Angular 20.3.6** (Standalone Components)
- **TypeScript**
- **Bootstrap 5**
- **RxJS**
- **Server-Side Rendering (SSR)**

### Backend

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **Clean Architecture** (Domain, Application, Infrastructure, WebAPI)

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

1. **Node.js** (v18 or higher) - [Download](https://nodejs.org/)
2. **.NET SDK 9.0** - [Download](https://dotnet.microsoft.com/download)
3. **PostgreSQL** (v12 or higher) - [Download](https://www.postgresql.org/download/)
4. **Git** - [Download](https://git-scm.com/)

### Verify Installations

```bash
# Check Node.js version
node --version

# Check npm version
npm --version

# Check .NET version
dotnet --version

# Check PostgreSQL
psql --version
```

## 🔧 Installation & Setup

### 1. Clone the Repository

```bash
cd C:\Users\YourUsername\Desktop\dev
git clone <repository-url>
cd wafi_solution
```


### 3. Backend Setup

#### Update Connection String (if needed)

Open `src/WebApi/appsettings.json` and verify:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=BusReservationDB;Username=postgres;Password=yourpassword"
  }
}
```

#### Restore Dependencies & Run Migrations

```bash
# Navigate to WebApi project
cd src/WebApi

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Apply database migrations (creates tables and seeds data)
dotnet ef database update

# Or run the application (migrations run automatically)
dotnet run
```

The backend API will start at: **http://localhost:5122**

**Swagger UI** available at: **http://localhost:5122/swagger**

### 4. Frontend Setup

#### Install Dependencies

```bash
# Navigate to frontend project
cd ../../frontend/bus-reservation-system

# Install npm packages
npm install
```

#### Start Development Server

```bash
# Start Angular dev server
npm start
```

The frontend will start at: **http://localhost:4200**

## 🎯 Running the Application

### Development Mode

1. **Terminal 1 - Backend:**

   ```bash
   cd src/WebApi
   dotnet run
   ```

   Backend runs on: http://localhost:5122

2. **Terminal 2 - Frontend:**

   ```bash
   cd frontend/bus-reservation-system
   npm start
   ```

   Frontend runs on: http://localhost:4200

3. **Open Browser:**
   Navigate to **http://localhost:4200**
```


## 📁 Project Structure

```
wafi_solution/
├── frontend/
│   └── bus-reservation-system/        # Angular Frontend
│       ├── src/
│       │   ├── app/
│       │   │   ├── components/        # UI Components
│       │   │   ├── services/          # HTTP Services
│       │   │   └── models/            # TypeScript Interfaces
│       │   └── styles.scss
│       ├── angular.json
│       └── package.json
├── src/
│   ├── Domain/                        # Domain Entities & Business Logic
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Services/
│   ├── Application/                   # Application Services
│   │   └── Services/
│   ├── Application.Contracts/         # DTOs & Interfaces
│   │   ├── DTOs/
│   │   └── Repositories/
│   ├── Infrastructure/                # Data Access & External Services
│   │   ├── Persistence/
│   │   ├── Repositories/
│   │   └── Migrations/
│   └── WebApi/                        # REST API Controllers
│       ├── Controllers/
│       ├── Program.cs
│       └── appsettings.json
└── README.md
```

## 🔌 API Endpoints

### Search

- `GET /api/search/buses` - Search buses by route and date
- `GET /api/search/buses/{id}` - Get bus details by schedule ID

### Booking

- `GET /api/booking/seat-plan/{busScheduleId}` - Get seat layout
- `POST /api/booking/book-seat` - Book a seat

### Request Examples

**Search Buses:**

```http
GET http://localhost:5122/api/search/buses?from=New York&to=Boston&journeyDate=2025-10-29
```

**Get Seat Plan:**

```http
GET http://localhost:5122/api/booking/seat-plan/{busScheduleId}
```

**Book Seat:**

```http
POST http://localhost:5122/api/booking/book-seat
Content-Type: application/json

{
  "busScheduleId": "guid-here",
  "seatId": "guid-here",
  "passengerName": "John Doe",
  "mobileNumber": "1234567890",
  "email": "john@example.com",
  "boardingPoint": "New York Port Authority",
  "droppingPoint": "Boston South Station"
}
```

## 🗃️ Database Schema

### Main Tables

- **Buses** - Bus information (name, type, capacity)
- **Routes** - Travel routes between cities
- **BusSchedules** - Scheduled bus trips
- **Seats** - Seat details and status
- **Passengers** - Passenger information
- **Tickets** - Booking records

## 🧪 Testing

### Backend Tests

```bash
cd src/Tests
dotnet test
```

### Frontend Tests

```bash
cd frontend/bus-reservation-system
npm test
```



## 📄 License

This project is licensed under the MIT License.

## 👥 Authors

- **Your Name** - Abdul Malek
---