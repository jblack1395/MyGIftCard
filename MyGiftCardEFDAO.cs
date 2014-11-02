using MyGiftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyGIftCard
{
    public class MyGiftCardEFDAO : IGiftCardDAO
    {
        public List<SalonModel> retrieveClients()
        {
            throw new NotImplementedException();
        }

        public List<PendingOrders> retrievePendingOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            throw new NotImplementedException();
        }

        public List<RedeemeddOrders> retrieveRedeemdOrders(string client, DateTime startdate, DateTime enddate, string filteredNam = null)
        {
            throw new NotImplementedException();
        }

        public List<ProcessedOrders> retrieveProcessedOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetail> retrieveOrderDetail(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            throw new NotImplementedException();
        }


        public UploadedFile retrieveLogoFileInfoForClient(string client)
        {
            throw new NotImplementedException();
        }

        public UploadedFile retrieveBackgroundFileInfoForClient(string client)
        {
            throw new NotImplementedException();
        }
    }
}