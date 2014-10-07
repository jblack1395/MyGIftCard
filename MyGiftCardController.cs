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

        public MyGiftCardController(IEncryptionUtil encryptionUtil)
        {
            this.encryptionUtil = encryptionUtil;
        }
        public IEncryptionUtil EncUtil {
            get {
                return encryptionUtil;
            }
        }

    }
}