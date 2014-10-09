using MyGiftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyGIftCard
{
    public class MyGiftCardEFDAO : IGiftCardDAO
    {
        public List<SalonModel> retrieveCustomers()
        {
            var list = new List<SalonModel>();
            return list;
        }
    }
}