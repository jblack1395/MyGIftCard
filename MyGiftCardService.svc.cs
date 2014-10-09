using MyGIftCard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace MyGiftCard
{
    public class MyGiftCardService : IMyGiftCardService
    {
        private static byte[] key;
        private static byte[] iv;
        private readonly IMyGiftCardController giftCardController;

        public MyGiftCardService(IMyGiftCardController controller)
        {
            this.giftCardController = controller;
        }

        public List<SalonModel> GetSalonList()
        {

            var v = new List<SalonModel>() 
         { 
                 new SalonModel() { SalonName = "Name 1", SalonAddress = new Address { AddressOne="address one", City="Knoxville", State="TN"}},
                 new SalonModel() { SalonName = "Name 2", SalonAddress = new Address { AddressOne="some other address", City="Knoxville", State="TN"}},
                 new SalonModel() { SalonName = "Name 3", SalonAddress = new Address { AddressOne="home address", City="Knoxville", State="TN"}},
                 new SalonModel() { SalonName = "Name 4", SalonAddress = new Address { AddressOne="work address", City="Knoxville", State="TN"}}
         };
            v.AddRange(giftCardController.retrieveCustomers());
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            return v;
        }

        public string SalonLogin(AuthModel model)
        {
            String token = "{\"error\":\"Not a valid Salon name\"}"; ;
            var statuscode = System.Net.HttpStatusCode.Unauthorized;
            var username = model.Username;
            var password = model.Password;

            try
            {
                if (model.Salon == "testsalon")
                {
                    if (password.Equals(new String(username.ToCharArray().Reverse().ToArray())))
                    {
                        token = "{\"token\":\"my token\"}";
                        statuscode = System.Net.HttpStatusCode.Accepted;
                        var msg = "";
                        using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                        {
                            var properties = OperationContext.Current.IncomingMessageProperties;
                            var endpointProperty = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                            msg = "my message";
                            if (endpointProperty != null)
                            {
                                var ip = endpointProperty.Address;
                                msg = ip.ToString() + ":" + model.Salon + ":" + (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                            }
                            // Encrypt the string to an array of bytes. 
                            key = myAes.Key;
                            iv = myAes.IV;
                            byte[] encrypted = giftCardController.EncUtil.EncryptStringToBytes_Aes(msg, myAes.Key, myAes.IV);
                            token = "{\"token\":\"" + System.Convert.ToBase64String(encrypted) + "\", \"msg\":\"" + msg + "\"}";
                        }
                    }
                    else
                    {
                        statuscode = System.Net.HttpStatusCode.Unauthorized;
                        token = "{\"error\":\"Username or password does not match for salon, " + model.Username + "," + model.Password + "," + new String(username.ToCharArray().Reverse().ToArray()) + "\"}";
                    }
                }
            }
            catch (Exception e)
            {
                token = e.StackTrace;
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            WebOperationContext.Current.OutgoingResponse.StatusCode = statuscode;
            return token;
        }

        public List<RedeemeddOrders> SearchOrdersByName(string token, string search_name, string startdate, string enddate)
        {
            throw new NotImplementedException();
        }

        public List<PendingOrders> ListPendingOrders(string token, string startdate, string enddate)
        {
            var s = giftCardController.EncUtil.DecryptToken(token, key, iv);
            var list = new List<PendingOrders>();
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            if (s == "my message")
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Accepted;
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            }
            return list;
        }

        public List<ProcessedOrders> ListProcessedOrders(string token, string startdate, string enddate)
        {
            throw new NotImplementedException();
        }

        public List<RedeemeddOrders> ListRedeemedOrders(string token, string startdate, string enddate)
        {
            throw new NotImplementedException();
        }


        public System.IO.Stream SalonImage(string salon)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(@"D:\a.jpg");
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
            return fs;
        }

        public CurrentSalonDisplaySettings ListCurrentSalonSettings(string token)
        {
            throw new NotImplementedException();
        }


        public UploadedFile Upload(System.IO.Stream Uploading)
        {
            throw new NotImplementedException();
        }


        public string GiftCardOrder(OrderDetail order)
        {
            var orderNumber = "fake order number with message: " + order.Message;

            return orderNumber;
        }

    }
}
