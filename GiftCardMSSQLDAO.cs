using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGiftCard
{
    public class GiftCardMSSQLDAO : IGiftCardDAO
    {
        String conn;
        public GiftCardMSSQLDAO Init()
        {
            System.Configuration.Configuration rootwebconfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/Mohtisham");
            System.Configuration.ConnectionStringSettings constring;
            constring = rootwebconfig.ConnectionStrings.ConnectionStrings["conn"];
            conn = constring.ConnectionString;
            return this;
        }

        public List<MyGiftCard.CompanyModel> retrieveClients()
        {
            var sql = "SELECT c.id,c.company_name,c.visa_accepted,c.mc_accepted,c.amex_accepted,c.discover_accepted,c.paypal_id,c.allow_mail," +
                "c.allow_gratuity,c.shipping_cost,c.expire_after_days,c.fine_print, a.address_one, a.address_two, a.city," +
                "a.zipcode, a.country, c.contact_info_id, i.first_name, i.last_name, i.email, i.phone, i.fax, i.website " +
                "FROM [dbo].[Companies] c INNER JOIN CompanyAddresses a ON(c.address_id=a.id) INNER JOIN ContactInfo i ON(ci.id=c.contact_info_id) ";
            var myconn = new SqlConnection(conn);
            var list = new List<CompanyModel>();
            using (var command = new SqlCommand(sql, myconn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CompanyModel()
                        {
                            CompanyName = reader.GetString(1),
                            VisaAccepted = reader.GetBoolean(2),
                            MasterCardAccepted = reader.GetBoolean(3),
                            AmericanExpressAccepted = reader.GetBoolean(4),
                            DiscoverAccepted = reader.GetBoolean(5),
                            PayPalID = reader.GetString(6),
                            AllowMailOption = reader.GetBoolean(7),
                            AllowGratuity = reader.GetBoolean(8),
                            ShippingCost = reader.GetFloat(9),
                            ExpireAfterDays = reader.GetInt16(10),
                            FinePrint = reader.GetString(11),
                            CompanyAddress = new Address()
                            {
                                AddressOne = reader.GetString(12),
                                AddressTwo = reader.GetString(13),
                                City = reader.GetString(14),
                                Zip = reader.GetString(15),
                                County = reader.GetString(16)
                            },
                            CompanyContactInfo = new ContactInfo()
                            {
                                FirstName = reader.GetString(17),
                                LastName = reader.GetString(18),
                                Email = reader.GetString(19),
                                Phone = reader.GetString(20),
                                Fax = reader.GetString(21),
                                Wedsite = reader.GetString(22)
                            }
                        });
                    }
                }
            }

            return list;
        }

        public List<MyGiftCard.PendingOrders> retrievePendingOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            var sql = "SELECT o.id,o.order_number,o.order_date,o.total,o.status,o.actions,o.shipping,o.tip,o.message, c.company_name, " +
                "p.name, r.name," +
                "o.credit_card_id FROM [dbo].[Orders] o INNER JOIN Companies c ON(c.id=o.company_id) " +
                "INNER JOIN Purchasers p ON(p.id=o.purchaser_id) INNER JOIN Recipients r ON(r.id=o.recipient_id)" +
                " WHERE o.date_redeemed IS NULL AND o.order_date >= '" + startdate.ToString("yyyyMMdd HH:mm:ss") + "' AND o.order_date <= '" +
                enddate.ToString("yyyyMMdd HH:mm:ss") + "'" + (filteredName == null ? "" : " AND r.name='" + filteredName + "'") +
                " AND c.company_name='" + client + "'";
            var myconn = new SqlConnection(conn);
            var list = new List<PendingOrders>();
            using (var command = new SqlCommand(sql, myconn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PendingOrders()
                        {
                            OrderNumber = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2).ToString("MM/dd/yyyy HH:mm"),
                            Total = reader.GetFloat(3),
                            Status = reader.GetString(4),
                            Actions = reader.GetString(5),
                            ShippingAndHandling = reader.GetFloat(6),
                            Tip = reader.GetFloat(7),
                            Message = reader.GetString(8),
                            CompanyName = reader.GetString(9),
                            Purchaser = new Purchaser()
                            {
                                Name = reader.GetString(10)
                            },
                            Recipient = new Recipient()
                            {
                                Name = reader.GetString(11)
                            },
                            CardInfo = new CreditCardInfo()
                            {
                            }
                        });
                    }
                }
            }
            return list;
        }

        public List<MyGiftCard.RedeemeddOrders> retrieveRedeemdOrders(string client, DateTime startdate, DateTime enddate, string filteredNam = null)
        {
            var sql = "SELECT o.id,o.order_number,o.order_date,o.total,o.status,o.actions,o.shipping,o.tip,o.message, c.company_name, " +
                "p.name, r.name," +
                "o.credit_card_id FROM [dbo].[Orders] o INNER JOIN Companies c ON(c.id=o.company_id) " +
                "INNER JOIN Purchasers p ON(p.id=o.purchaser_id) INNER JOIN Recipients r ON(r.id=o.recipient_id)" +
                " WHERE o.date_redeemed IS NOT NULL AND o.order_date >= '" + startdate.ToString("yyyyMMdd HH:mm:ss") + "' AND o.order_date <= '" +
                enddate.ToString("yyyyMMdd HH:mm:ss") + "'" + (filteredNam == null ? "" : " AND r.name='" + filteredNam + "'") +
                " AND c.company_name='" + client + "'";
            var myconn = new SqlConnection(conn);
            var list = new List<RedeemeddOrders>();
            using (var command = new SqlCommand(sql, myconn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RedeemeddOrders()
                        {
                            OrderNumber = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2).ToString("MM/dd/yyyy HH:mm"),
                            Total = reader.GetFloat(3),
                            Status = reader.GetString(4),
                            Actions = reader.GetString(5),
                            ShippingAndHandling = reader.GetFloat(6),
                            Tip = reader.GetFloat(7),
                            Message = reader.GetString(8),
                            CompanyName = reader.GetString(9),
                            Purchaser = new Purchaser()
                            {
                                Name = reader.GetString(10)
                            },
                            Recipient = new Recipient()
                            {
                                Name = reader.GetString(11)
                            },
                            CardInfo = new CreditCardInfo()
                            {
                            }
                        });
                    }
                }
            }
            return list;
        }

        public List<MyGiftCard.ProcessedOrders> retrieveProcessedOrders(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            var sql = "SELECT o.id,o.order_number,o.order_date,o.total,o.status,o.actions,o.shipping,o.tip,o.message, c.company_name, " +
                "p.name, r.name," +
                "o.credit_card_id FROM [dbo].[Orders] o INNER JOIN Companies c ON(c.id=o.company_id) " +
                "INNER JOIN Purchasers p ON(p.id=o.purchaser_id) INNER JOIN Recipients r ON(r.id=o.recipient_id)" +
                " WHERE o.date_redeemed IS NULL AND o.order_date >= '" + startdate.ToString("yyyyMMdd HH:mm:ss") + "' AND o.order_date <= '" +
                enddate.ToString("yyyyMMdd HH:mm:ss") + "'" + (filteredName == null ? "" : " AND r.name='" + filteredName + "'") +
                " AND c.company_name='" + client + "'";
            var myconn = new SqlConnection(conn);
            var list = new List<ProcessedOrders>();
            using (var command = new SqlCommand(sql, myconn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProcessedOrders()
                        {
                            OrderNumber = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2).ToString("MM/dd/yyyy HH:mm"),
                            Total = reader.GetFloat(3),
                            Status = reader.GetString(4),
                            Actions = reader.GetString(5),
                            ShippingAndHandling = reader.GetFloat(6),
                            Tip = reader.GetFloat(7),
                            Message = reader.GetString(8),
                            CompanyName = reader.GetString(9),
                            Purchaser = new Purchaser()
                            {
                                Name = reader.GetString(10)
                            },
                            Recipient = new Recipient()
                            {
                                Name = reader.GetString(11)
                            },
                            CardInfo = new CreditCardInfo()
                            {
                            }
                        });
                    }
                }
            }
            return list;
        }

        public List<MyGiftCard.OrderDetail> retrieveOrderDetail(string client, DateTime startdate, DateTime enddate, string filteredName = null)
        {
            var sql = "SELECT o.id,o.order_number,o.order_date,o.total,o.status,o.actions,o.shipping,o.tip,o.message, c.company_name, " +
                "p.name, r.name," +
                "o.credit_card_id FROM [dbo].[Orders] o INNER JOIN Companies c ON(c.id=o.company_id) " +
                "INNER JOIN Purchasers p ON(p.id=o.purchaser_id) INNER JOIN Recipients r ON(r.id=o.recipient_id)" +
                " WHERE o.order_date >= '" + startdate.ToString("yyyyMMdd HH:mm:ss") + "' AND o.order_date <= '" +
                enddate.ToString("yyyyMMdd HH:mm:ss") + "'" + (filteredName == null ? "" : " AND r.name='" + filteredName + "'") +
                " AND c.company_name='" + client + "'";
            var myconn = new SqlConnection(conn);
            var list = new List<OrderDetail>();
            using (var command = new SqlCommand(sql, myconn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new OrderDetail()
                        {
                            OrderNumber = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2).ToString("MM/dd/yyyy HH:mm"),
                            Total = reader.GetFloat(3),
                            Status = reader.GetString(4),
                            Actions = reader.GetString(5),
                            ShippingAndHandling = reader.GetFloat(6),
                            Tip = reader.GetFloat(7),
                            Message = reader.GetString(8),
                            CompanyName = reader.GetString(9),
                            Purchaser = new Purchaser()
                            {
                                Name = reader.GetString(10)
                            },
                            Recipient = new Recipient()
                            {
                                Name = reader.GetString(11)
                            },
                            CardInfo = new CreditCardInfo()
                            {
                            }
                        });
                    }
                }
            }
            return list;
        }

        public MyGiftCard.UploadedFile retrieveLogoFileInfoForClient(string client)
        {
            throw new NotImplementedException();
        }

        public MyGiftCard.UploadedFile retrieveBackgroundFileInfoForClient(string client)
        {
            throw new NotImplementedException();
        }
    }
}
