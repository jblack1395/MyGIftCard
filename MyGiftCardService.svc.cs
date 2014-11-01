using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

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

        public Stream GetSalonList()
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
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<SalonModel>));
            ser.WriteObject(stream1, v);
            return stream1;
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

        public Stream ListOrders(string op, string token, string startdate, string enddate, string customer_name)
        {
            string returnType = "html";
            int indx = op.IndexOf(".");
            if (indx > -1)
            {
                returnType = op.Substring(indx + 1);
                op = op.Substring(0, indx);
            }
            var s = giftCardController.EncUtil.DecryptToken(token, key, iv);
            if (s == "my message")
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Accepted;
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            }
            MemoryStream stream1 = new MemoryStream();
            if (op == "pending")
            {
                var list = new List<PendingOrders>();
                if (returnType == "json")
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<PendingOrders>));
                    ser.WriteObject(stream1, list);
                }
                else if (returnType == "xml")
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                    XmlSerializer ser = new XmlSerializer(typeof(List<PendingOrders>));
                    Console.WriteLine("Testing for type: {0}", typeof(List<PendingOrders>));
                    ser.Serialize(XmlWriter.Create(stream1), list);
                    stream1.Flush();
                }
                else
                {
                    StringBuilder buf = new StringBuilder("<html><head><title></title></head><body><table><thead>");
                    buf.Append("</thead><tbody>");
                    buf.Append("</tbody></table></body></html>");
                    byte[] resultBytes = Encoding.UTF8.GetBytes(buf.ToString());
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                    stream1 = new MemoryStream(resultBytes);
                }
            }
            else if (op == "processed")
            {
                var list = new List<ProcessedOrders>();
                if (returnType == "json")
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<ProcessedOrders>));
                    ser.WriteObject(stream1, list);
                }
                else if (returnType == "xml")
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                    XmlSerializer ser = new XmlSerializer(typeof(List<ProcessedOrders>));
                    Console.WriteLine("Testing for type: {0}", typeof(List<ProcessedOrders>));
                    ser.Serialize(XmlWriter.Create(stream1), list);
                    stream1.Flush();
                }
                else
                {
                    StringBuilder buf = new StringBuilder("<html><head><title></title></head><body><table><thead>");
                    buf.Append("</thead><tbody>");
                    buf.Append("</tbody></table></body></html>");
                    byte[] resultBytes = Encoding.UTF8.GetBytes(buf.ToString());
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                    stream1 = new MemoryStream(resultBytes);
                }
            }
            else if (op == "redeemed")
            {
                var list = new List<RedeemeddOrders>();
                if (returnType == "json")
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<RedeemeddOrders>));
                    ser.WriteObject(stream1, list);
                }
                else if (returnType == "xml")
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                    XmlSerializer ser = new XmlSerializer(typeof(List<RedeemeddOrders>));
                    Console.WriteLine("Testing for type: {0}", typeof(List<RedeemeddOrders>));
                    ser.Serialize(XmlWriter.Create(stream1), list);
                    stream1.Flush();
                }
                else
                {
                    StringBuilder buf = new StringBuilder("<html><head><title></title></head><body><table><thead>");
                    buf.Append("</thead><tbody>");
                    buf.Append("</tbody></table></body></html>");
                    byte[] resultBytes = Encoding.UTF8.GetBytes(buf.ToString());
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                    stream1 = new MemoryStream(resultBytes);
                }
            }
            return stream1;
        }

        public System.IO.Stream SalonImage(string salon, string width, string height)
        {
            int w;
            int.TryParse(width, out w);
            int h;
            int.TryParse(height, out h);
            Bitmap bitmap = new Bitmap(w, h);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, (Math.Abs(i - j) < 2) ? Color.Blue : Color.Yellow);
                }
            }
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Position = 0;
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
            return ms;
        }

        public Stream ListCurrentSalonSettings(string token)
        {
            throw new NotImplementedException();
        }


        public string Upload(System.IO.Stream Uploading)
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
