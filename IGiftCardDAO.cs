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
        List<PendingOrders> retrievePendingOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null);
        List<RedeemeddOrders> retrieveRedeemdOrders(int client, DateTime startdate, DateTime enddate, string filteredNam = null);
        List<ProcessedOrders> retrieveProcessedOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null);
        List<OrderDetail> retrieveOrderDetail(int client, DateTime startdate, DateTime enddate, string filteredName = null);
        UploadedFile retrieveLogoFileInfoForClient(int client);
        UploadedFile retrieveBackgroundFileInfoForClient(int client);
        bool UpdateSalon(CompanyModel input);
        List<String> RetrieveCategoriesBySalon(String company_name);
        List<Product> RetrieveProductsByCompanyCategory(String company_name, string category_name);
        CurrentCompanyDisplaySettings RetrieveDisplaySettings(int companyId);
        int? InsertPendingOrder(PendingOrders orders);
        int? CreateCategoryForCompany(int companyId, String categoryName);
        List<String> RetrieveCategoriesByCompany(int companyId);
        int? CreateProductForCategoryByCompany(int categoryId, String productName, float price);
        List<Product> RetrieveProductsBySalonCategory(int categoryId);
        bool SaveAuthInfo(int companyId, string username, byte[] passwordHash);
        bool CheckPassword(int companyId, string username, byte[] passwordHash);
        int? SaveUploadedFile(UploadedFile input);
        UploadedFile RetrieveLogoFileInfoForClient(int companyId);
        bool UpdateCompanySettings(CurrentCompanyDisplaySettings info, int? logoId);
        bool? retrievePassword(int salonid, string username, byte[] password);
    }
}
