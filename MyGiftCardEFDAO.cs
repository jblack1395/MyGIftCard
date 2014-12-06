using MyGiftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyGIftCard
{
    public class MyGiftCardEFDAO : IGiftCardDAO
    {
        public List<CompanyModel> retrieveClients()
        {
            throw new NotImplementedException();
        }

        public List<PendingOrders> retrievePendingOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            throw new NotImplementedException();
        }

        public List<RedeemeddOrders> retrieveRedeemdOrders(int client, DateTime startdate, DateTime enddate, string filteredNam = null)
        {
            throw new NotImplementedException();
        }

        public List<ProcessedOrders> retrieveProcessedOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetail> retrieveOrderDetail(int client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            throw new NotImplementedException();
        }


        public UploadedFile retrieveLogoFileInfoForClient(int client)
        {
            throw new NotImplementedException();
        }

        public UploadedFile retrieveBackgroundFileInfoForClient(int client)
        {
            throw new NotImplementedException();
        }


        public bool UpdateSalon(CompanyModel input)
        {
            throw new NotImplementedException();
        }


        public List<string> RetrieveCategoriesBySalon(string company_name)
        {
            throw new NotImplementedException();
        }

        public List<Product> RetrieveProductsByCompanyCategory(string company_name, string category_name)
        {
            throw new NotImplementedException();
        }


        public CurrentCompanyDisplaySettings RetrieveDisplaySettings(int companyId)
        {
            throw new NotImplementedException();
        }

        public int? InsertPendingOrder(PendingOrders orders)
        {
            throw new NotImplementedException();
        }


        public int? CreateCategoryForCompany(int companyId, string categoryName)
        {
            throw new NotImplementedException();
        }

        public List<string> RetrieveCategoriesByCompany(int companyId)
        {
            throw new NotImplementedException();
        }


        public int? CreateProductForCategoryByCompany(int categoryId, string productName, float price)
        {
            throw new NotImplementedException();
        }

        public List<Product> RetrieveProductsBySalonCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public bool SaveAuthInfo(int companyId, string username, byte[] passwordHash)
        {
            throw new NotImplementedException();
        }

        public bool CheckPassword(int companyId, string username, byte[] passwordHash)
        {
            throw new NotImplementedException();
        }


        public int? SaveUploadedFile(UploadedFile input)
        {
            throw new NotImplementedException();
        }

        public UploadedFile RetrieveLogoFileInfoForClient(int companyId)
        {
            throw new NotImplementedException();
        }


        public bool UpdateCompanySettings(CurrentCompanyDisplaySettings info, int? logoId)
        {
            throw new NotImplementedException();
        }
    }
}