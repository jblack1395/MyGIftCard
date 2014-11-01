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
        List<SalonModel> retrieveClients();
        List<T> retrieveOrdersByClient<T>(int ordertype, string client, DateTime startdate, DateTime enddate, string filtername = null);
        UploadedFile retrieveUploadedFileInfo(string client, string filetype);
    }
}
