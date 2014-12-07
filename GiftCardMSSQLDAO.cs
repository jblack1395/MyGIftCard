using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Data;
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

        public bool? retrievePassword(int salonid, string username, byte[] password)
        {
            var myconn = new SqlConnection(conn);
            var list = new List<String>();
            using (var command = new SqlCommand("SELECT password_len FROM AuthInfo WHERE salon_id=" + salonid + " AND username=@username AND password='" + Convert.ToBase64String(password) + "'", myconn))
            {
                var p = new SqlParameter("@username", SqlDbType.VarChar, 50);
                p.Value = username;
                command.Parameters.Add(p);
                myconn.Open();
                try
                {
                    command.Prepare();
                    using (var result = command.ExecuteReader())
                    {
                        var b = result.Read();
                        return b;
                    }
                }
                finally
                {
                    myconn.Close();
                }
            }
        }

        public bool SaveAuthInfo(int salonid, string username, byte[] password)
        {
            var myconn = new SqlConnection(conn);
            var list = new List<String>();
            using (var command = new SqlCommand("INSERT INTO AuthInfo(salon_id, username, password, password_len) VALUES(" + salonid + ",@username, @password, " + password.Length + ")", myconn))
            {
                var p = new SqlParameter("@username", SqlDbType.VarChar, 50);
                p.Value = username;
                command.Parameters.Add(p);
                p = new SqlParameter("@password", SqlDbType.VarChar, 255);
                p.Value = Convert.ToBase64String(password);
                command.Parameters.Add(p);
                myconn.Open();
                try
                {
                    command.Prepare();
                    if (command.ExecuteNonQuery() == 1)
                        return true;
                }
                finally
                {
                    myconn.Close();
                }
            }
            using (var command = new SqlCommand("UPDATE AuthInfo SET password=@password WHERE salon_id=" + salonid + " AND username=@username", myconn))
            {
                var p = new SqlParameter("@username", SqlDbType.VarChar, 50);
                p.Value = username;
                command.Parameters.Add(p);
                p = new SqlParameter("@password", SqlDbType.VarChar, 255);
                p.Value = Convert.ToBase64String(password);
                command.Parameters.Add(p);
                myconn.Open();
                try
                {
                    command.Prepare();
                    if (command.ExecuteNonQuery() == 1)
                        return true;
                }
                finally
                {
                    myconn.Close();
                }
            }
            return false;
        }

        public int? InsertPendingOrder(PendingOrders input)
        {
            var myconn = new SqlConnection(conn);
            int customerId = 0;
            int recipientId = 0;
            var sql = "INSERT INTO Customers(name) VALUES(@customer_name);select SCOPE_IDENTITY();";
            using (var command = new SqlCommand(sql))
            {
                command.Parameters.Add(new SqlParameter("@customer_name", input.Purchaser));
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    customerId = reader.GetInt32(0);
                }
            }
            int? recipient_address_id = null;
            if (input.DeliveryInfo != null && String.IsNullOrWhiteSpace(input.DeliveryInfo.Email) && input.DeliveryInfo != null)
            {
                sql = "INSERT INTO [dbo].[CustomerAddress] ([address_one],[address_two],[city],[state],[zip]) " +
        "VALUES(@address_one, @address_two, @city, @state, @zip);select SCOPE_IDENTITY();";
                using (var command = new SqlCommand(sql))
                {
                    command.Parameters.Add(new SqlParameter("@address_one", input.DeliveryInfo.DeliveryAddress.AddressOne));
                    command.Parameters.Add(new SqlParameter("@address_two", input.DeliveryInfo.DeliveryAddress.AddressTwo));
                    command.Parameters.Add(new SqlParameter("@city", input.DeliveryInfo.DeliveryAddress.City));
                    command.Parameters.Add(new SqlParameter("@state", input.DeliveryInfo.DeliveryAddress.State));
                    command.Parameters.Add(new SqlParameter("@zip", input.DeliveryInfo.DeliveryAddress.Zip));
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        recipient_address_id = reader.GetInt32(0);
                    }
                }
            }
            sql = "INSERT INTO Recipient(name) VALUES(@customer_name);select SCOPE_IDENTITY();";
            using (var command = new SqlCommand(sql))
            {
                command.Parameters.Add(new SqlParameter("@customer_name", input.Recipient));
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    recipientId = reader.GetInt32(0);
                }
            }
            int creditcardid = 0;
            sql = "INSERT INTO [dbo].[CreditCardInfo] ([customer_id],[card_type],[card_number],[expiration_date],[card_code]) " +
     "VALUES(@customer_id, @card_type, @card_number, @expiration_date, @card_code);select SCOPE_IDENTITY();";
            using (var command = new SqlCommand(sql))
            {
                command.Parameters.Add(new SqlParameter("@customer_id", customerId));
                command.Parameters.Add(new SqlParameter("@card_type", input.CardInfo.CardType));
                command.Parameters.Add(new SqlParameter("@card_number", input.CardInfo.CardNumber));
                command.Parameters.Add(new SqlParameter("@expiration_date", input.CardInfo.ExpirationDate));
                command.Parameters.Add(new SqlParameter("@card_code", input.CardInfo.CardCode));
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    creditcardid = reader.GetInt32(0);
                }
            }
            sql = "INSERT INTO [dbo].[CustomerAddress] ([address_one],[address_two],[city],[state],[zip]) " +
     "VALUES(@address_one, @address_two, @city, @state, @zip)";
            using (var command = new SqlCommand(sql))
            {
                command.Parameters.Add(new SqlParameter("@address_one", input.CardInfo.BillingInfo.AddressOne));
                command.Parameters.Add(new SqlParameter("@address_two", input.CardInfo.BillingInfo.AddressTwo));
                command.Parameters.Add(new SqlParameter("@city", input.CardInfo.BillingInfo.City));
                command.Parameters.Add(new SqlParameter("@state", input.CardInfo.BillingInfo.State));
                command.Parameters.Add(new SqlParameter("@zip", input.CardInfo.BillingInfo.Zip));
                command.Prepare();
                command.ExecuteNonQuery();
            }

            sql = "INSERT INTO [dbo].[Orders] " +
           "([customer_id]" +
           ",[recipient_id]" +
           ",[card_info_id]" +
           ",[total]" +
           ",[status]" +
           ",[actions]" +
           ",[tip]" +
           ",[date_redeemed]" +
           ",[message]" +
           ",[delivered_by]" +
           ",[email]" +
           ",[processed_on]" +
           ",[authentication_code]" +
           ",[shipping_handling]" +
           ",[salon_id])" +
     " VALUES(" +
           "@customer_id" +
           ",@recipient_id" +
           ",@card_id" +
           ",@total" +
           ",@status" +
           ",@actions" +
           ",@tip" +
           ",NULL" +
           ",@message" +
           ",NULL" +
           ",@email" +
           ",NULL" +
           ",NULL" +
           ",@shipping" +
           ",@salon_id);select SCOPE_IDENTITY();";
            Int32? id = 0;
            using (var command = new SqlCommand(sql))
            {
                command.Parameters.Add(new SqlParameter("@card_id", creditcardid));
                command.Parameters.Add(new SqlParameter("@customer_id", customerId));
                command.Parameters.Add(new SqlParameter("@recipient_id", recipientId));
                command.Parameters.Add(new SqlParameter("@salon_id", input.CompanyId));
                command.Parameters.Add(new SqlParameter("@total", input.Total));
                command.Parameters.Add(new SqlParameter("@status", input.Status));
                command.Parameters.Add(new SqlParameter("@actions", input.Actions));
                command.Parameters.Add(new SqlParameter("@tip", input.Tip));
                command.Parameters.Add(new SqlParameter("@message", input.Message));
                command.Parameters.Add(new SqlParameter("@shipping", input.ShippingAndHandling));
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    id = reader.GetInt32(0);
                }
            }
            if (id.HasValue)
            {
                // Save items
                sql = "INSERT INTO ItemOrders(product_id, order_id) VALUES(@product_id, " + id.Value + ")";
                using (var command = new SqlCommand(sql))
                {
                    input.ItemsOrderedList.ForEach(i =>
                    {
                        command.Parameters.Add(new SqlParameter("@product_id", i.ProductId));
                        command.Prepare();
                        command.ExecuteNonQuery();
                    });
                }
            }
            return id;
        }

        public List<String> RetrieveCategoriesBySalon(String company_name)
        {
            var myconn = new SqlConnection(conn);
            var list = new List<String>();
            using (var command = new SqlCommand("SELECT c.name FROM categories c INNER JOIN Companies s ON(s.id=c.salon_id) WHERE s.company_name=@company_name", myconn))
            {
                command.Parameters.Add(new SqlParameter("@company_name", company_name));
                command.Prepare();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        list.Add(result.GetString(0));
                    }
                }
            }
            return list;
        }

        public List<Product> RetrieveProductsByCompanyCategory(String company_name, string category_name)
        {
            var myconn = new SqlConnection(conn);
            var list = new List<Product>();
            using (var command = new SqlCommand("SELECT p.name, p.price FROM products p INNER JOIN categories c ON(p.category_id=c.id) INNER JOIN Companies s ON(s.id=c.company_id) WHERE s.company_name=@salon_name AND c.name=@category_name", myconn))
            {
                command.Parameters.Add(new SqlParameter("@company_name", company_name));
                command.Parameters.Add(new SqlParameter("@category_name", category_name));
                command.Prepare();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        list.Add(new Product()
                        {
                            Name = result.GetString(0),
                            Price = result.GetFloat(1)
                        });
                    }
                }
            }
            return list;
        }

        public bool UpdateSalon(CompanyModel input)
        {
            var sql = input.Id.HasValue ?
            "UPDATE [dbo].[Salons]" +
             "  SET [salon_name] = @salon_name" +
                  ",[salon_contact_id] = @salon_contact_id" +
                  ",[salon_address_id] = @salon_address_id" +
                  ",[visa_accepted] = @visa_accepted" +
                  ",[amex_accepted] = @amex_accepted" +
                  ",[discover_accepted] = @discover_accepted" +
                  ",[pay_pal_id] = @pay_pal_id" +
                  ",[authorize_dotnet_api_login_id] = @authorize_dotnet_api_login_id" +
                  ",[authorize_dotnet_password] = @authorize_dotnet_password" +
                  ",[authorize_dotnet_live] = @authorize_dotnet_live" +
                  ",[allow_mail_option] = @allow_mail_option" +
                  ",[allow_gratuity] = @allow_gratuity" +
                  ",[shipping_cost] = @shipping_cost" +
                  ",[expire_after_days] = @expire_after_days" +
            " WHERE [id] = @id"
                : "INSERT INTO [dbo].[Salons]" +
                       ",[salon_name]" +
                       ",[salon_contact_id]" +
                       ",[salon_address_id]" +
                       ",[visa_accepted]" +
                       ",[amex_accepted]" +
                       ",[discover_accepted]" +
                       ",[pay_pal_id]" +
                       ",[authorize_dotnet_api_login_id]" +
                       ",[authorize_dotnet_password]" +
                       ",[authorize_dotnet_live]" +
                       ",[allow_mail_option]" +
                       ",[allow_gratuity]" +
                       ",[shipping_cost]" +
                       ",[expire_after_days]) " +
                 "VALUES(@salon_name, NULL, NULL, @visa_accepted, @amex_accepted, @discover_accepted, @pay_pal_id," +
                 "@authorize_dotnet_api_login_id, @authorize_dotnet_password, @authorize_dotnet_live, @allow_mail_option, @allow_gratuity," +
                 "@shipping_cost, @expire_after_days)";

            var myconn = new SqlConnection(conn);
            using (var command = new SqlCommand(sql))
            {
                if (input.Id.HasValue)
                    command.Parameters.Add(new SqlParameter("@id", input.Id.Value));
                command.Parameters.Add(new SqlParameter("salon_name", input.CompanyName));
                command.Parameters.Add(new SqlParameter("@visa_accepted", input.VisaAccepted));
                command.Parameters.Add(new SqlParameter("@amex_accepted", input.AmericanExpressAccepted));
                command.Parameters.Add(new SqlParameter("@discover_accepted", input.DiscoverAccepted));
                command.Parameters.Add(new SqlParameter("@pay_pal_id", input.PayPalID));
                command.Parameters.Add(new SqlParameter("@allow_mail_option", input.AllowMailOption));
                command.Parameters.Add(new SqlParameter("@allow_gratuity", input.AllowGratuity));
                command.Parameters.Add(new SqlParameter("@shipping_cost", input.ShippingCost));
                command.Parameters.Add(new SqlParameter("@expire_after_days", input.ExpireAfterDays));
                command.Prepare();
                var num = command.ExecuteNonQuery();
                return num == 1;
            }
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

        public List<MyGiftCard.PendingOrders> retrievePendingOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null)
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

        public List<MyGiftCard.RedeemeddOrders> retrieveRedeemdOrders(int client, DateTime startdate, DateTime enddate, string filteredNam = null)
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

        public List<MyGiftCard.ProcessedOrders> retrieveProcessedOrders(int client, DateTime startdate, DateTime enddate, string filteredName = null)
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

        public List<MyGiftCard.OrderDetail> retrieveOrderDetail(int client, DateTime startdate, DateTime enddate, string filteredName = null)
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

        public MyGiftCard.UploadedFile retrieveLogoFileInfoForClient(int client)
        {
            throw new NotImplementedException();
        }

        public MyGiftCard.UploadedFile retrieveBackgroundFileInfoForClient(int client)
        {
            throw new NotImplementedException();
        }


        public CurrentCompanyDisplaySettings RetrieveDisplaySettings(int companyId)
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
