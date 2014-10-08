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

    }
}