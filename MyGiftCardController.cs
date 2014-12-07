using Microsoft.Practices.Unity;
using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MyGiftCard
{
    public class MyGiftCardController : IMyGiftCardController
    {
        public readonly IEncryptionUtil encryptionUtil;
        public readonly IGiftCardDAO dao;
        private static byte[] key = null;
        private static byte[] iv;


        [InjectionConstructor]
        public MyGiftCardController(IEncryptionUtil encryptionUtil, IGiftCardDAO dao)
        {
            this.encryptionUtil = encryptionUtil;
            this.dao = dao;
        }

        public bool SaveAuthInfo(int salon_id, string username, string password)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            var enc = Encoding.Default;
            var hash = mySHA256.ComputeHash(enc.GetBytes(password));
            return dao.SaveAuthInfo(salon_id, username, hash);
        }

        public string authenticateLogin(AuthModel model, string msg)
        {
            var encryptionUtil = new EncryptionUtil();
            String token = "";
            SHA256 mySHA256 = SHA256Managed.Create();
            var enc = Encoding.Default;
            var hash = mySHA256.ComputeHash(enc.GetBytes(model.Password));
            try
            {
                var fp = dao.retrievePassword(model.CompanyID, model.Username, hash);
                if (!fp.HasValue)
                {
                    return "{\"error\":\"password not found, " + model.CompanyID + ", " + model.Username + "," + model.Password + "," + "SELECT password_len FROM AuthInfo WHERE salon_id=" + model.CompanyID + " AND username='" + model.Username + "' AND password='" + Convert.ToBase64String(hash) + "'" + "\"}";
                }
                if (fp.Value)
                {
                    token = "{\"token\":\"my token\"}";
                    using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                    {
                        // Encrypt the string to an array of bytes. 
                        if (key == null)
                        {
                            key = myAes.Key;
                            iv = myAes.IV;
                        }
                        byte[] encrypted = encryptionUtil.EncryptStringToBytes_Aes(msg, key, iv);
                        return (System.Convert.ToBase64String(encrypted));
                    }
                }
                else
                {
                    token = "{\"error\":\"Username or password does not match for salon, " + model.Username + "," + model.Password + "," + "SELECT password_len FROM AuthInfo WHERE salon_id=" + model.CompanyID + " AND username='" + model.Username + "' AND password='" + Convert.ToBase64String(hash) + "'" + "\"}";
                }
            }
            catch (Exception e)
            {
                token = e.StackTrace;
            }
            return token;
        }

        public List<CompanyModel> retrieveClients()
        {
            return dao.retrieveClients();
        }

        public List<T> retrieveOrdersByClient<T>(int ordertype, int client, DateTime startdate, DateTime enddate, string filtername)
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

        public Image retrieveUploadedFile(int client, string filetype, int width_percentage)
        {
            UploadedFile uploadedFile = null;
            if (filetype.Equals("logo"))
                uploadedFile = dao.retrieveLogoFileInfoForClient(client);
            else
                uploadedFile = dao.retrieveBackgroundFileInfoForClient(client);
            return ResizeImage(Image.FromFile(Path.Combine(uploadedFile.FileName, uploadedFile.FileName), true), width_percentage * uploadedFile.Width, width_percentage * uploadedFile.Height);
        }

        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public string verifyToken(string token)
        {
            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
            {
                // Encrypt the string to an array of bytes.
                if (key == null)
                {
                    key = myAes.Key;
                    iv = myAes.IV;
                }
                return encryptionUtil.DecryptToken(token, key, iv);
            }
        }
    }
}