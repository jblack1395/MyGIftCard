using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGiftCard
{
    public interface IMyGiftCardController
    {
        IEncryptionUtil EncUtil { get; }
    }
}
