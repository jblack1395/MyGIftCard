using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGIftCard
{
    public interface IEncryptionUtil
    {
        byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV);
        string DecryptToken(string token, byte[] key, byte[] iv);
    }
}
