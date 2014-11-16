using MyGIftCard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGiftCard
{
    public interface IMyGiftCardController
    {
        IEncryptionUtil EncUtil { get; }
        List<CompanyModel> retrieveClients();
        List<T> retrieveOrdersByClient<T>(int ordertype, string client, DateTime startdate, DateTime enddate, string filtername = null);
        Image retrieveUploadedFile(string client, string filetype, int width_percentage);
    }
}
