using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManagmentSystemAPI.DBconfig;
using TransportManagmentSystemAPI.Interfaces;
using TransportManagmentSystemAPI.Models;

namespace TransportManagmentSystemAPI.Services
{
    public class ReservationService : IReservation
    {
        private readonly IMongoCollection<Reservation> _reservationList;
        private readonly IMongoCollection<Train> _trainList;
        public ReservationService(IDatabaseSettings _databaseSettings, IScheam _scheam)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _reservationList = database.GetCollection<Reservation>(_scheam.ReservationScheam);
            _trainList = database.GetCollection<Train>(_scheam.TrainScheam);

        }
        public Dictionary<int,string> CreateReservation(Reservation reservation)
        {
            Dictionary<int, string> returnCode = new Dictionary<int, string>();
            var bookingDate = DateTime.Now;
            var reservationDate = reservation.ReservationDate;

            TimeSpan diffecrence = reservationDate - bookingDate;

            if (diffecrence.TotalDays >= 30 || diffecrence.TotalDays < 0)
            {
                returnCode.Add(100, "Reservation must be withing 30 days from Booking date");
                return returnCode;
            }
            else 
            {
                var ReservationCount = _reservationList.Find(res => res.ReferenceId == reservation.ReferenceId && !res.IsCancelled).ToList().Count;
                if (ReservationCount > 4)
                {
                    returnCode.Add(200, "Maximum 4 reservation per reference ID");
                    return returnCode;
                }
                else 
                {
                    if (reservation.Id != null)
                    {
                        TimeSpan UpdateDaydiff = reservationDate - DateTime.Now;
                        if (UpdateDaydiff.TotalDays > 5)
                        {
                            reservation.BookingCreatedDate = bookingDate;
                            _reservationList.ReplaceOne(res => res.Id == reservation.Id, reservation);
                            returnCode.Add(400, "Reservation Updated  " + reservation.Id);
                            return returnCode;
                        }
                        else if (UpdateDaydiff.TotalDays < 0) 
                        {
                            returnCode.Add(100, "Expired");
                            return returnCode;
                        }
                        else
                        {
                            returnCode.Add(100, "Your Reservation confirmed and this cannot be updated/Cancelled");
                            return returnCode;
                        }
                    }
                    else
                    {
                        reservation.BookingCreatedDate = bookingDate;
                        _reservationList.InsertOne(reservation);
                        returnCode.Add(400, "Reservation Created " + reservation.Id);
                        return returnCode;
                    }  
                }
            }
        }

        public List<Reservation> DisplayAllReservation(string travallerId)
        {
            var listofBookings = travallerId != null ? _reservationList.Find(res => res.TravallerProfile == travallerId).ToList() : _reservationList.Find(res => true).ToList();
            int index = 0;
            foreach (Reservation res in listofBookings)
            {
                index = index + 1;
                Train gottrain = _trainList.Find(tra => tra.Id == res.Train).ToList().FirstOrDefault();
                listofBookings[index - 1].BookedTrain = gottrain;
            }
            if (listofBookings.Count > 0)
            {
                return listofBookings;
            }
            else
            {
                return null;
            }
        }
    }
}
