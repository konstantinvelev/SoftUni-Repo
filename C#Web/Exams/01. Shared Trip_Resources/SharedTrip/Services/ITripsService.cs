using SharedTrip.Models;
using SharedTrip.ViewModels;
using System.Collections.Generic;

namespace SharedTrip.Services
{
    public interface ITripsService
    {
        List<Trip> All();

        void Add(CreateTripInputModel create);

        Trip GetTripById(string id);

        void AddUserToTrip(string tripId, string userId);

        List<string> AllUsersId(string tripId);
    }
}
