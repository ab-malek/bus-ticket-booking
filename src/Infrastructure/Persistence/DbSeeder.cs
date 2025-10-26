using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(BusReservationDbContext context)
    {
        if (await context.Buses.AnyAsync())
            return; // Database already seeded

        // Create Routes
        var route1 = new Route("New York", "Boston", 215, TimeSpan.FromHours(4));
        var route2 = new Route("Los Angeles", "San Francisco", 382, TimeSpan.FromHours(6));
        var route3 = new Route("Chicago", "Detroit", 283, TimeSpan.FromHours(5));

        await context.Routes.AddRangeAsync(route1, route2, route3);
        await context.SaveChangesAsync();

        // Create Buses
        var bus1 = new Bus("Express Travels", "Express Deluxe", "ET-1234", "AC", 40);
        var bus2 = new Bus("Royal Coaches", "Royal Sleeper", "RC-5678", "AC Sleeper", 40);
        var bus3 = new Bus("City Liner", "City Express", "CL-9012", "Non-AC", 45);
        var bus4 = new Bus("Metro Express", "Metro Luxury", "ME-3456", "AC", 40);
        var bus5 = new Bus("Comfort Lines", "Comfort Deluxe", "CL-7890", "AC Sleeper", 45);

        await context.Buses.AddRangeAsync(bus1, bus2, bus3, bus4, bus5);
        await context.SaveChangesAsync();

        // Get tomorrow's date and specific dates
        var tomorrow = DateTime.Today.AddDays(1);
        var dayAfterTomorrow = DateTime.Today.AddDays(2);
        var oct29_2025 = new DateTime(2025, 10, 29);

        // Create Bus Schedules for New York to Boston
        var schedule1 = new BusSchedule(
            bus1.Id, route1.Id,
            tomorrow,
            new TimeSpan(8, 0, 0),
            new TimeSpan(12, 0, 0),
            45m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule1.InitializeSeats(40);

        var schedule2 = new BusSchedule(
            bus2.Id, route1.Id,
            tomorrow,
            new TimeSpan(10, 0, 0),
            new TimeSpan(14, 0, 0),
            50m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule2.InitializeSeats(40);

        var schedule3 = new BusSchedule(
            bus3.Id, route1.Id,
            tomorrow,
            new TimeSpan(14, 0, 0),
            new TimeSpan(18, 0, 0),
            40m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule3.InitializeSeats(45);

        // Additional schedules for day after tomorrow
        var schedule4 = new BusSchedule(
            bus1.Id, route1.Id,
            dayAfterTomorrow,
            new TimeSpan(9, 0, 0),
            new TimeSpan(13, 0, 0),
            45m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule4.InitializeSeats(40);

        // Create schedules for other routes
        var schedule5 = new BusSchedule(
            bus4.Id, route2.Id,
            tomorrow,
            new TimeSpan(9, 0, 0),
            new TimeSpan(15, 0, 0),
            65m,
            "Los Angeles Union Station",
            "San Francisco Ferry Building"
        );
        schedule5.InitializeSeats(40);

        var schedule6 = new BusSchedule(
            bus5.Id, route3.Id,
            tomorrow,
            new TimeSpan(11, 0, 0),
            new TimeSpan(16, 0, 0),
            55m,
            "Chicago Union Station",
            "Detroit Central Station"
        );
        schedule6.InitializeSeats(45);

        // ========== SCHEDULES FOR OCTOBER 29, 2025 ==========

        // New York to Boston - October 29, 2025
        var schedule7 = new BusSchedule(
            bus1.Id, route1.Id,
            oct29_2025,
            new TimeSpan(6, 0, 0),  // 6:00 AM
            new TimeSpan(10, 0, 0), // 10:00 AM
            45m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule7.InitializeSeats(40);

        var schedule8 = new BusSchedule(
            bus2.Id, route1.Id,
            oct29_2025,
            new TimeSpan(8, 30, 0),  // 8:30 AM
            new TimeSpan(12, 30, 0), // 12:30 PM
            50m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule8.InitializeSeats(40);

        var schedule9 = new BusSchedule(
            bus3.Id, route1.Id,
            oct29_2025,
            new TimeSpan(12, 0, 0),  // 12:00 PM
            new TimeSpan(16, 0, 0),  // 4:00 PM
            40m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule9.InitializeSeats(45);

        var schedule10 = new BusSchedule(
            bus1.Id, route1.Id,
            oct29_2025,
            new TimeSpan(15, 30, 0),  // 3:30 PM
            new TimeSpan(19, 30, 0),  // 7:30 PM
            48m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule10.InitializeSeats(40);

        var schedule11 = new BusSchedule(
            bus2.Id, route1.Id,
            oct29_2025,
            new TimeSpan(20, 0, 0),  // 8:00 PM
            new TimeSpan(0, 0, 0),   // 12:00 AM (next day)
            55m,
            "New York Port Authority",
            "Boston South Station"
        );
        schedule11.InitializeSeats(40);

        // Los Angeles to San Francisco - October 29, 2025
        var schedule12 = new BusSchedule(
            bus4.Id, route2.Id,
            oct29_2025,
            new TimeSpan(7, 0, 0),   // 7:00 AM
            new TimeSpan(13, 0, 0),  // 1:00 PM
            65m,
            "Los Angeles Union Station",
            "San Francisco Ferry Building"
        );
        schedule12.InitializeSeats(40);

        var schedule13 = new BusSchedule(
            bus5.Id, route2.Id,
            oct29_2025,
            new TimeSpan(14, 0, 0),  // 2:00 PM
            new TimeSpan(20, 0, 0),  // 8:00 PM
            70m,
            "Los Angeles Union Station",
            "San Francisco Ferry Building"
        );
        schedule13.InitializeSeats(45);

        var schedule14 = new BusSchedule(
            bus4.Id, route2.Id,
            oct29_2025,
            new TimeSpan(22, 0, 0),  // 10:00 PM
            new TimeSpan(4, 0, 0),   // 4:00 AM (next day)
            60m,
            "Los Angeles Union Station",
            "San Francisco Ferry Building"
        );
        schedule14.InitializeSeats(40);

        // Chicago to Detroit - October 29, 2025
        var schedule15 = new BusSchedule(
            bus5.Id, route3.Id,
            oct29_2025,
            new TimeSpan(8, 0, 0),   // 8:00 AM
            new TimeSpan(13, 0, 0),  // 1:00 PM
            55m,
            "Chicago Union Station",
            "Detroit Central Station"
        );
        schedule15.InitializeSeats(45);

        var schedule16 = new BusSchedule(
            bus3.Id, route3.Id,
            oct29_2025,
            new TimeSpan(13, 30, 0),  // 1:30 PM
            new TimeSpan(18, 30, 0),  // 6:30 PM
            52m,
            "Chicago Union Station",
            "Detroit Central Station"
        );
        schedule16.InitializeSeats(45);

        var schedule17 = new BusSchedule(
            bus4.Id, route3.Id,
            oct29_2025,
            new TimeSpan(18, 0, 0),  // 6:00 PM
            new TimeSpan(23, 0, 0),  // 11:00 PM
            58m,
            "Chicago Union Station",
            "Detroit Central Station"
        );
        schedule17.InitializeSeats(40);

        await context.BusSchedules.AddRangeAsync(
            schedule1, schedule2, schedule3, schedule4, schedule5, schedule6,
            schedule7, schedule8, schedule9, schedule10, schedule11,
            schedule12, schedule13, schedule14,
            schedule15, schedule16, schedule17);

        await context.SaveChangesAsync();
    }
}
