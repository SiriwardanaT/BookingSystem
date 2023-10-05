using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManagmentSystemAPI.DBconfig;
using TransportManagmentSystemAPI.Models;

namespace TransportManagmentSystemAPI.Services
{
    interface ITravallerService
    {
        TravallerProfile CreateTravellerProfile(TravallerProfile travallerProfile);
        List<TravallerProfile> DisplayAllActiveProfile(bool isActive);
        TravallerProfile UpdateTravellerProfile(string id , TravallerProfile travallerProfile);
        String DeletedTravellerProfile(String _Nic);
        TravallerProfile ManageActivationTravellerProfile(TravallerProfile travallerProfile);
    }
}
