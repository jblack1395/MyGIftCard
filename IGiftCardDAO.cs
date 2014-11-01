using MyGiftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGIftCard
{
    public interface IGiftCardDAO
    {
        List<SalonModel> retrieveClients();
        List<PendingOrders> retrievePendingOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null);
        List<RedeemeddOrders> retrieveRedeemdOrders(string client, DateTime startdate, DateTime enddate, string filteredNam = null);
        List<ProcessedOrders> retrieveProcessedOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null);
        List<OrderDetail> retrieveOrderDetail(string client, DateTime startdate, DateTime enddate, string filteredName = null);
        UploadedFile retrienveLogoForClient(string client);
        UploadedFile retrienveBackgroundForClient(string client);
    }
}
