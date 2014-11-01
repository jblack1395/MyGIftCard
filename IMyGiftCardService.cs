using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MyGiftCard
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGiftCertProService" in both code and config file together.
    [ServiceContract]
//    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public interface IMyGiftCardService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
                                 ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "GetSalonList/")]
        Stream GetSalonList();

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "SalonLogin",
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        string SalonLogin(AuthModel model);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "GiftCardOrder",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GiftCardOrder(OrderDetail order);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
                                 ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "ListOrders/{op}/{token}/{startdate=empty}/{enddate=empty}/{customer_name=empty}")]
        Stream ListOrders(string op, string token, string startdate, string enddate, string customer_name);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
                                 ResponseFormat = WebMessageFormat.Xml,
                                UriTemplate = "SalonImage/{salon}/{width=100}/{height=100}")]
        System.IO.Stream SalonImage(string salon, string width, string height);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
                                 ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "ListCurrentSalonSettings/{token}")]
        Stream ListCurrentSalonSettings(string token);

        [OperationContract(Name = "Upload")]
        [DataContractFormat]
        [WebInvoke(Method = "POST",
                   UriTemplate = "Upload",
                   BodyStyle = WebMessageBodyStyle.Bare,
                   ResponseFormat = WebMessageFormat.Json)]
        string Upload(Stream Uploading);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class AuthModel
    {
        [DataMember]
        public string Salon { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    [DataContract]
    public class PendingOrders
    {
        [DataMember]
        public String OrderNumber { get; set; }
        [DataMember]
        public String OrderDate { get; set; }
        [DataMember]
        public String Purchaser { get; set; }
        [DataMember]
        public String Recipient { get; set; }
        [DataMember]
        public Double Total { get; set; }
        [DataMember]
        public String Status { get; set; }
        [DataMember]
        public String Actions { get; set; }
    }
    [DataContract]
    public class ProcessedOrders : PendingOrders
    {
        [DataMember]
        public String[] ItemsOrderedList { get; set; }
        [DataMember]
        public Double ShippingAndHandling { get; set; }
        [DataMember]
        public Double Tip { get; set; }
    }
    [DataContract]
    public class RedeemeddOrders : ProcessedOrders
    {
        [DataMember]
        public String DateRedeemed { get; set; }
    }
    [DataContract]
    public class CreditCardInfo
    {
        [DataMember]
        public String CardType { get; set; }
        [DataMember]
        public String CardNumber { get; set; }
        [DataMember]
        public String ExpirationDate { get; set; }
        [DataMember]
        public String CardCode { get; set; }
    }
    [DataContract]
    public class OrderDetail : RedeemeddOrders
    {
        [DataMember]
        public String Message { get; set; }
        [DataMember]
        public String DeliverBy { get; set; }
        [DataMember]
        public String BillingInfo { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public CreditCardInfo CardInfo { get; set; }
        [DataMember]
        public String ProcessedOn { get; set; }
        [DataMember]
        public String AuthenticationCode { get; set; }
    }
    [DataContract]
    public partial class SalonModel
    {
        [DataMember]
        public String SalonName { set; get; }
        [DataMember]
        public Address SalonAddress { set; get; }
        [DataMember]
        public ContactInfo SalonContactInfo { set; get; }
        [DataMember]
        public Boolean VisaAccepted { get; set; }
        [DataMember]
        public Boolean MasterCardAccepted { get; set; }
        [DataMember]
        public Boolean AmericanExpressAccepted { get; set; }
        [DataMember]
        public Boolean DiscoverAccepted { get; set; }
        [DataMember]
        public String PayPalID { get; set; }
        [DataMember]
        public String AuthorizeDotNetAPILoginID { get; set; }
        [DataMember]
        public String AuthorizeDotNetPassword { get; set; }
        [DataMember]
        public Boolean AuthorizeDotNetLive { get; set; }
        [DataMember]
        public Boolean? AllowMailOption { get; set; }
        [DataMember]
        public Boolean? AllowGratuity { get; set; }
        [DataMember]
        public Double? ShippingCost { get; set; }
        [DataMember]
        public Int16? ExpireAfterDays { get; set; }
        [DataMember]
        public String FinePrint { get; set; }

        [System.Runtime.Serialization.OnDeserialized]
        void OnDeserialized(System.Runtime.Serialization.StreamingContext c)
        {
            AllowGratuity = (AllowGratuity == null ? false : AllowGratuity);
            AllowMailOption = (AllowMailOption == null ? false : AllowMailOption);
            ExpireAfterDays = (ExpireAfterDays == null ? 0 : ExpireAfterDays);
        }
    }
    [DataContract]
    public partial class ContactInfo
    {
        [DataMember]
        public String FirstName { set; get; }
        [DataMember]
        public String LastName { set; get; }
        [DataMember]
        public String Phone { set; get; }
        [DataMember]
        public String Fax { set; get; }
        [DataMember]
        public String Email { set; get; }
        [DataMember]
        public String Wedsite { set; get; }
    }
    [DataContract]
    public partial class Address
    {
        [DataMember]
        public String AddressOne { set; get; }
        [DataMember]
        public String AddressTwo { set; get; }
        [DataMember]
        public String City { set; get; }
        [DataMember]
        public String State { set; get; }
        [DataMember]
        public String Zip { set; get; }
        [DataMember]
        public String County { set; get; }
    }
    [DataContract]
    public partial class CurrentSalonDisplaySettings
    {
        [DataMember]
        public String SalonName { get; set; }
        [DataMember]
        public String CurrentThemeName { set; get; }
        [DataMember]
        public Boolean? LogoUploaded { set; get; }
        [System.Runtime.Serialization.OnDeserialized]
        void OnDeserialized(System.Runtime.Serialization.StreamingContext c)
        {
            LogoUploaded = (LogoUploaded == null ? false : LogoUploaded);
        }
    }

    [DataContract]
  public class UploadedFile
  {
    [DataMember]
    public string FilePath { get; set; }

    [DataMember]
    public string FileLength { get; set; }

    [DataMember]
    public string FileName { get; set; }
  }  
}
