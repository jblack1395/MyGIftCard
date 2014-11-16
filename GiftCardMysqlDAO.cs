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

        public List<PendingOrders> retrievePendingOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null)
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

        public List<RedeemeddOrders> retrieveRedeemdOrders(string client, DateTime startdate, DateTime enddate, string filteredNam = null)
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

        public List<ProcessedOrders> retrieveProcessedOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null)
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

        public List<OrderDetail> retrieveOrderDetail(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            return new List<OrderDetail>()
            {
                new OrderDetail() {
                    CompanyName = "Test name 2"
                }
            };
        }


        public UploadedFile retrieveLogoFileInfoForClient(string client)
        {
            return new UploadedFile() {
                FileName = "logo.png",
                FilePath = "C:\\Temp\\",
                Height = 100,
                Width = 100
            };
        }

        public UploadedFile retrieveBackgroundFileInfoForClient(string client)
        {
            return new UploadedFile()
            {
                FileName = "background.png",
                FilePath = "C:\\Temp\\",
                Height = 100,
                Width = 100
            };
        }
    }
}