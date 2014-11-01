using Microsoft.Practices.Unity;
using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyGiftCard
{
    public class MyGiftCardController : IMyGiftCardController
    {
        public readonly IEncryptionUtil encryptionUtil;
        public readonly IGiftCardDAO dao;

        [InjectionConstructor]
        public MyGiftCardController(IEncryptionUtil encryptionUtil, IGiftCardDAO dao)
        {
            this.encryptionUtil = encryptionUtil;
            this.dao = dao;
        }
        public IEncryptionUtil EncUtil
        {
            get {
                return encryptionUtil;
            }
        }

        public List<SalonModel> retrieveClients()
        {
            throw new NotImplementedException();
        }

        public List<T> retrieveOrdersByClient<T>(int ordertype, string client, DateTime startdate, DateTime enddate, string filtername)
        {
            switch (ordertype)
            {
                case Constants.PENDING_ORDER:
                    var list = dao.retrievePendingOrders(client, startdate, enddate);
                    return list as List<T>;
                case Constants.REDEEMED_ORDER:
                    return dao.retrieveRedeemdOrders(client, startdate, enddate) as List<T>;
                case Constants.PROCESSED_ORDER:
                    return dao.retrieveProcessedOrders(client, startdate, enddate) as List<T>;
                default:
                    return dao.retrieveOrderDetail(client, startdate, enddate) as List<T>;
            }
        }

        public UploadedFile retrieveUploadedFileInfo(string client, string filetype)
        {
            throw new NotImplementedException();
        }
    }
}