using SharedTrip.Services;
using SharedTrip.ViewModels;
using SharedTrip.ViewModels.Trips;
using SharedTrip.ViewModels.Users;
using SIS.HTTP;
using SIS.MvcFramework;
using System;
using System.Globalization;

namespace SharedTrip.Controllers
{
    public class TripsController : Controller
    {
        private readonly ITripsService tripsService;

        public TripsController(ITripsService tripsService)
        {
            this.tripsService = tripsService;
        }

        public HttpResponse Add()
        {
            if (!this.IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            return this.View();
        }

        [HttpPost]
        public HttpResponse Add(CreateTripInputModel input)
        {
            if (!this.IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            if (string.IsNullOrWhiteSpace(input.StartPoint))
            {
                return this.Redirect("/Trips/Add");
            }
            if (string.IsNullOrWhiteSpace(input.EndPoint))
            {
                return this.Redirect("/Trips/Add");
            }
            if (input.DepartureTime == null)
            {
                return this.Redirect("/Trips/Add");
            }
            if (input.Seats < 2 || input.Seats > 6)
            {
                return this.Redirect("/Trips/Add");
            }
            if (input.Description.Length > 80)
            {
                return this.Redirect("/Trips/Add");
            }

            this.tripsService.Add(input);
            return this.Redirect("/");
        }

        public HttpResponse All()
        {
            if (!this.IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            var viewModel = new HomeViewModel
            {
                Trips = this.tripsService.All()
            };

            return this.View(viewModel);
        }

        public HttpResponse Details(string tripId)
        {
            if (!this.IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            var trip = this.tripsService.GetTripById(tripId);
            if (trip == null)
            {
                this.Redirect("/Trips/All");
            }

            var viewModel = new DetailsTripViewModel
            {
                Id = trip.Id,
                StartPoint = trip.StartPoint,
                EndPoint = trip.EndPoint,
                DepartureTime = trip.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
                Description = trip.Description,
                ImagePath = trip.ImagePath,
                Seats = trip.Seats
            };
            return this.View(viewModel);
        }
        public HttpResponse AddUserToTrip(string tripId)
        {
            var trip = this.tripsService.GetTripById(tripId);
            var userToTripId = this.tripsService.AllUsersId(tripId);

            if (!this.IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            if (trip == null)
            {
                return this.Redirect("/Trips/Details?tripId=" + tripId);
            }
            if (userToTripId.Contains(this.User))
            {
                return this.Redirect("/Trips/Details?tripId=" + tripId);
            }

            this.tripsService.AddUserToTrip(tripId, this.User);
            return this.Redirect("/");
        }
    }
}
