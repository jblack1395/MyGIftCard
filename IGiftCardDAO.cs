using MyGiftCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGIftCard
{
    public interface IGiftCardDAO
    {
        List<SalonModel> retrieveCustomers();
    }
}
