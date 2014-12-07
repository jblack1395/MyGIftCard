using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    static class SafeBase64UrlEncoder
    {
        private const string Plus = "+";
        private const string Minus = "-";
        private const string Slash = "/";
        private const string Underscore = "_";
        private const string EqualSign = "=";
        private const string Pipe = "|";
        private static readonly IDictionary<string, string> _mapper;

        static SafeBase64UrlEncoder()
        {
            _mapper = new Dictionary<string, string> { { Plus, Minus }, { Slash, Underscore }, { EqualSign, Pipe } };
        }

        /// <summary>
        /// Encode the base64 to safeUrl64
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public static string EncodeBase64Url(string base64Str)
        {
            if (string.IsNullOrEmpty(base64Str))
                return base64Str;

            foreach (var pair in _mapper)
            {
                base64Str = base64Str.Replace(pair.Key, pair.Value);
            }

            return base64Str;
        }


        /// <summary>
        /// Decode the Url back to original base64
        /// </summary>
        /// <param name="safe64Url"></param>
        /// <returns></returns>
        public static string DecodeBase64Url(string safe64Url)
        {

            if (string.IsNullOrEmpty(safe64Url))
                return safe64Url;

            foreach (var pair in _mapper)
            {
                safe64Url = safe64Url.Replace(pair.Value, pair.Key);
            }

            return safe64Url;
        }
    }
    public class MyGiftCardService : IMyGiftCardService
    {
        private readonly IMyGiftCardController giftCardController;

        public MyGiftCardService(IMyGiftCardController controller)
        {
            this.giftCardController = controller;
        }

        public Stream GetSalonList()
        {

            var v = new List<CompanyModel>() 
         { 
                 new CompanyModel() { CompanyName = "Name 1", CompanyAddress = new Address { AddressOne="address one", City="Knoxville", State="TN"}},
                 new CompanyModel() { CompanyName = "Name 2", CompanyAddress = new Address { AddressOne="some other address", City="Knoxville", State="TN"}},
                 new CompanyModel() { CompanyName = "Name 3", CompanyAddress = new Address { AddressOne="home address", City="Knoxville", State="TN"}},
                 new CompanyModel() { CompanyName = "Name 4", CompanyAddress = new Address { AddressOne="work address", City="Knoxville", State="TN"}}
         };
            v.AddRange(giftCardController.retrieveClients());
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<CompanyModel>));
            ser.WriteObject(stream1, v);
            return stream1;
        }

        public string SalonLogin(string company_id, AuthModel model)
        {
            int id;
            WebOperationContext ctx = WebOperationContext.Current;
            if (!Int32.TryParse(company_id, out id))
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return "Salon ID is not valid";
            }
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint =
                prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = "1";
            var result = giftCardController.authenticateLogin(new AuthModel()
            {
                CompanyID = id,
                Password = model.Password,
                Username = model.Username
            }, (id + ":" + ip + ":" + DateTime.Now.ToString("yyyMMdd_HHmmss")));

            String s = result.Replace("\\", "");
            return SafeBase64UrlEncoder.EncodeBase64Url(s);
        }

        public Stream ListOrders(string op, string urltoken, string startdate, string enddate, string customer_name)
        {
            if (op.ToLower().StartsWith("help"))
            {
                StringBuilder buf = new StringBuilder("<html><head><title></title></head><body><table><thead>");
                buf.Append("<tr><th>Order Name</th><th>Description</th></tr>");
                buf.Append("</thead><tbody>");
                buf.Append("<tr><td>ReportsByMonth</td><td>Look at the breakdown by month, default values is the past two months to today</td></tr>");
                buf.Append("<tr><td>ReportsByMonthByCustomer</td><td>Look at the breakdown by month, looking at each customer individually, default values is the past two months to today</td></tr>");
                buf.Append("</tbody></table></body></html>");
                byte[] resultBytes = Encoding.UTF8.GetBytes(buf.ToString());
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(resultBytes);
            }
            string returnType = "html";
            int indx = op.IndexOf(".");
            if (indx > -1)
            {
                returnType = op.Substring(indx + 1);
                op = op.Substring(0, indx);
            }
            string token = urltoken.Replace("-", "+").Replace("_", "/").Replace("|", "=");
            WebOperationContext ctx = WebOperationContext.Current;
            MemoryStream stream1 = new MemoryStream();
            var s = giftCardController.verifyToken(token);
            int client = 0;
            var ss = s.Split(';');
            if (ss.Length == 3)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Accepted;
                int.TryParse(ss[0], out client);
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                StringBuilder buf = new StringBuilder("<html><head><title></title></head><body>");
                    buf.Append("The token used was invalid, please login again.</body></html>");
                    byte[] resultBytes = Encoding.UTF8.GetBytes(buf.ToString());
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                    return new MemoryStream(resultBytes);
            }
            var ds = DateTime.ParseExact(startdate, "yyyyMMdd_hhmm", CultureInfo.InvariantCulture);
            var de = DateTime.ParseExact(enddate, "yyyyMMdd_hhmm", CultureInfo.InvariantCulture);
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            if (startdate.Equals("empty"))
            {
                start = DateTime.Now.AddDays(-60);
            }
            if (enddate.Equals("empty"))
            {
                end = DateTime.Now;
            }
            if (op == "pending")
            {
                var list = customer_name.Equals("empty") ? giftCardController.retrieveOrdersByClient<PendingOrders>(Constants.PENDING_ORDER, client, start, end)
                    : giftCardController.retrieveOrdersByClient<PendingOrders>(Constants.PENDING_ORDER, client, start, end, customer_name);
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
                var list = customer_name.Equals("empty") ? giftCardController.retrieveOrdersByClient<ProcessedOrders>(Constants.PENDING_ORDER, client, start, end)
                    : giftCardController.retrieveOrdersByClient<ProcessedOrders>(Constants.PENDING_ORDER, client, start, end, customer_name);
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
                var list = customer_name.Equals("empty") ? giftCardController.retrieveOrdersByClient<RedeemeddOrders>(Constants.PENDING_ORDER, client, start, end)
                    : giftCardController.retrieveOrdersByClient<RedeemeddOrders>(Constants.PENDING_ORDER, client, start, end, customer_name);
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

        public System.IO.Stream SalonImage(int salon, string image_type, string width_percentage)
        {
            int w;
            int.TryParse(width_percentage, out w);
            var bitmap = giftCardController.retrieveUploadedFile(salon, image_type, w);
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


        Stream IMyGiftCardService.GetSalonList()
        {
            throw new NotImplementedException();
        }

        string IMyGiftCardService.SalonLogin(string salon_id, AuthModel model)
        {
            throw new NotImplementedException();
        }

        string IMyGiftCardService.GiftCardOrder(OrderDetail order)
        {
            throw new NotImplementedException();
        }

        Stream IMyGiftCardService.ListOrders(string op, string token, string startdate, string enddate, string customer_name)
        {
            throw new NotImplementedException();
        }

        Stream IMyGiftCardService.OrderReports(string op, string token, string enddate, string startdate, string custom1, string custom2)
        {
            if (op.ToLower().StartsWith("help"))
            {
                StringBuilder buf = new StringBuilder("<html><head><title></title></head><body><table><thead>");
                buf.Append("<tr><th>Order Name</th><th>Description</th></tr>");
                buf.Append("</thead><tbody>");
                buf.Append("<tr><td>ReportsByMonth</td><td>Look at the breakdown by month, default values is the past two months to today</td></tr>");
                buf.Append("<tr><td>ReportsByMonthByCustomer</td><td>Look at the breakdown by month, looking at each customer individually, default values is the past two months to today</td></tr>");
                buf.Append("</tbody></table></body></html>");
                byte[] resultBytes = Encoding.UTF8.GetBytes(buf.ToString());
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(resultBytes);
            }
            throw new NotImplementedException();
        }

        Stream IMyGiftCardService.SalonImage(string salon, string image_type, string width_percentage)
        {
            throw new NotImplementedException();
        }

        Stream IMyGiftCardService.ListCurrentSalonSettings(string token)
        {
            throw new NotImplementedException();
        }

        string IMyGiftCardService.Upload(Stream Uploading)
        {
            throw new NotImplementedException();
        }
    }
}
