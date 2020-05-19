using System.Collections.Generic;
using System.Linq;
using SharedTrip.Models;
using SharedTrip.ViewModels;

namespace SharedTrip.Services
{
    public class TripsService : ITripsService
    {
        private readonly ApplicationDbContext db;

        public TripsService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void Add(CreateTripInputModel create)
        {
            var trip = new Trip
            {
                StartPoint = create.StartPoint,
                EndPoint = create.EndPoint,
                DepartureTime = create.DepartureTime,
                ImagePath = create.ImagePath,
                Description = create.Description,
                Seats = create.Seats,
            };

            this.db.Trips.Add(trip);
            this.db.SaveChanges();
        }

        public void AddUserToTrip(string tripId, string userId)
        {
            var trip = this.db.Trips.Find(tripId);
            var user = this.db.Users.Find(userId);

            var userTrip = new UserTrip
            {
                Trip = trip,
                TripId = trip.Id,
                User = user,
                UserId = user.Id
            };

            trip.Seats = trip.Seats - 1;
            trip.UserTrips.Add(userTrip);
            this.db.SaveChanges();
        }

        public List<Trip> All()
        {
            var trips = this.db.Trips.ToList();
            return trips;
        }

        public List<string> AllUsersId(string tripId)
        {
           var usersId =  this.db.UsersTrips.Where(s => s.TripId == tripId).Select(s => s.UserId).ToList();
            return usersId;
        }

        public Trip GetTripById(string id)
        {
            var trip = this.db.Trips.Find(id);
            return trip;
        }
    }
}
