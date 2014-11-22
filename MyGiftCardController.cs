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
using System.Web;

namespace MyGiftCard
{
    public class MyGiftCardController : IMyGiftCardController
    {
        private static byte[] key;
        private static byte[] iv;

        public readonly IEncryptionUtil encryptionUtil;
        public readonly IGiftCardDAO dao;

        [InjectionConstructor]
        public MyGiftCardController(IEncryptionUtil encryptionUtil, IGiftCardDAO dao)
        {
            this.encryptionUtil = encryptionUtil;
            this.dao = dao;
        }

        public string authenticateLogin(AuthModel model, string msg)
        {
            String token = "";
            try
            {
                if (model.Salon == "testsalon")
                {
                    if (model.Password.Equals(new String(model.Username.ToCharArray().Reverse().ToArray())))
                    {
                        token = "{\"token\":\"my token\"}";
                        using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                        {
                            // Encrypt the string to an array of bytes. 
                            key = myAes.Key;
                            iv = myAes.IV;
                            byte[] encrypted = encryptionUtil.EncryptStringToBytes_Aes(msg, myAes.Key, myAes.IV);
                            token = "{\"token\":\"" + System.Convert.ToBase64String(encrypted) + "\", \"msg\":\"" + msg + "\"}";
                        }
                    }
                    else
                    {
                        token = "{\"error\":\"Username or password does not match for salon, " + model.Username + "," + model.Password + "," + new String(model.Username.ToCharArray().Reverse().ToArray()) + "\"}";
                    }
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

        public Image retrieveUploadedFile(string client, string filetype, int width_percentage)
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
            return encryptionUtil.DecryptToken(token, key, iv);
        }
    }
}