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
        List<CompanyModel> retrieveClients();
        List<PendingOrders> retrievePendingOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null);
        List<RedeemeddOrders> retrieveRedeemdOrders(string client, DateTime startdate, DateTime enddate, string filteredNam = null);
        List<ProcessedOrders> retrieveProcessedOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null);
        List<OrderDetail> retrieveOrderDetail(string client, DateTime startdate, DateTime enddate, string filteredName = null);
        UploadedFile retrieveLogoFileInfoForClient(string client);
        UploadedFile retrieveBackgroundFileInfoForClient(string client);
        bool UpdateSalon(CompanyModel input);
        List<String> RetrieveCategoriesBySalon(String company_name);
        List<Product> RetrieveProductsByCompanyCategory(String company_name, string category_name);
    }
}
