using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManagmentSystemAPI.DBconfig;
using TransportManagmentSystemAPI.Models;

namespace TransportManagmentSystemAPI.Services
{
    public class TravallerProfileService : ITravallerService
    {
        private readonly IMongoCollection<TravallerProfile> _travalProfileList;
        private readonly IMongoDatabase _database;
        public TravallerProfileService(IDatabaseSettings _databaseSettings,IScheam _scheam)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _travalProfileList = database.GetCollection<TravallerProfile>(_scheam.TravellerScheama);
        }
        public TravallerProfile CreateTravellerProfile(TravallerProfile travallerProfile)
        {
            try
            {
                var uniqueCounts =  _travalProfileList.Find(trv => trv.Nic == travallerProfile.Nic).ToList().Count;
                if (uniqueCounts == 0)
                {
                    travallerProfile.CreatedDate = DateTime.Now;
                    _travalProfileList.InsertOne(travallerProfile);
                    return travallerProfile;
                }
                else 
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong ERRLOGGED ! " + e.ToString());
            }
            
        }

        public String DeletedTravellerProfile(String _Nic)
        {
            try
            {
                 _travalProfileList.DeleteOne(trv => trv.Nic == _Nic);
                return _Nic;

            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong ERRLOGGED ! " + e.ToString());
            }
        }

        public List<TravallerProfile> DisplayAllActiveProfile(bool isActive)
        {
            try
            {
                var profileList = _travalProfileList.Find(trav => trav.AccStatus == isActive).ToList();
                return profileList.Count > 0 ? profileList : null; 
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong ERRLOGGED ! " + e.ToString());
            }
        }

        public TravallerProfile ManageActivationTravellerProfile(TravallerProfile travallerProfile)
        {
            try
            {
                if (travallerProfile.Nic != null)
                {
                    _travalProfileList.ReplaceOne(trav => trav.Nic == travallerProfile.Nic, travallerProfile);
                    return travallerProfile;
                }
                else 
                {
                    return null;
                }
               
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong ERRLOGGED ! " + e.ToString());
            }
        }

        public TravallerProfile UpdateTravellerProfile(string id , TravallerProfile travallerProfile)
        {
            try
            {
                if (travallerProfile != null)
                {
                    _travalProfileList.ReplaceOne(trav => trav.Id == id , travallerProfile);
                    return travallerProfile;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong ERRLOGGED ! " + e.ToString());
            }
           
        }
    }
}
