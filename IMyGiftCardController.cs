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
        List<CompanyModel> retrieveClients();
        List<T> retrieveOrdersByClient<T>(int ordertype, int client, DateTime startdate, DateTime enddate, string filtername = null);
        Image retrieveUploadedFile(int client, string filetype, int width_percentage);
        string authenticateLogin(AuthModel model, string msg);
        string verifyToken(String token);
    }
}
