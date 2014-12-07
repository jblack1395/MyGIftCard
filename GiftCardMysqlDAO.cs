using MyGiftCard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MyGIftCard
{
    public class GiftCardMysqlDAO : IGiftCardDAO
    {
        public List<CompanyModel> retrieveClients()
        {
            return new List<CompanyModel>()
            {
                new CompanyModel() {
                    CompanyName = "Test name 1"
                },
                new CompanyModel() {
                    CompanyName = "Test name 2"
                },
                new CompanyModel() {
                    CompanyName = "Test name 3"
                }
            };
        }

        public List<PendingOrders> retrievePendingOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            return new List<PendingOrders>()
            {
                new PendingOrders() {
                    OrderDate = startdate.ToString("YYYYMMDD"),
                    CompanyName = "Test name 1"
                },
                new PendingOrders() {
                    OrderDate =  enddate.ToString("YYYYMMDD"),
                    CompanyName = "Test name 3"
                }
            };
        }

        public List<RedeemeddOrders> retrieveRedeemdOrders(int client, DateTime startdate, DateTime enddate, string filteredNam = null)
        {
            return new List<RedeemeddOrders>()
            {
                new RedeemeddOrders() {
                    OrderDate = startdate.ToString("YYYYMMDD"),
                    CompanyName = "Test name 1"
                },
                new RedeemeddOrders() {
                    OrderDate =  enddate.ToString("YYYYMMDD"),
                    CompanyName = "Test name 3"
                }
            };
        }

        public List<ProcessedOrders> retrieveProcessedOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            return new List<ProcessedOrders>()
            {
                new ProcessedOrders() {
                    OrderDate = startdate.ToString("YYYYMMDD"),
                    CompanyName = "Test name 1"
                },
                new ProcessedOrders() {
                    OrderDate =  enddate.ToString("YYYYMMDD"),
                    CompanyName = "Test name 3"
                }
            };
        }

        public List<OrderDetail> retrieveOrderDetail(int client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            return new List<OrderDetail>()
            {
                new OrderDetail() {
                    CompanyName = "Test name 2"
                }
            };
        }


        public UploadedFile retrieveLogoFileInfoForClient(int client)
        {
            return new UploadedFile() {
                FileName = "logo.png",
                FilePath = "C:\\Temp\\",
                Height = 100,
                Width = 100
            };
        }

        public UploadedFile retrieveBackgroundFileInfoForClient(int client)
        {
            return new UploadedFile()
            {
                FileName = "background.png",
                FilePath = "C:\\Temp\\",
                Height = 100,
                Width = 100
            };
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


        public bool? retrievePassword(int salonid, string username, byte[] password)
        {
            throw new NotImplementedException();
        }
    }
}