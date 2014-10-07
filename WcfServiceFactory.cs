using Microsoft.Practices.Unity;
using MyGIftCard;
using Unity.Wcf;

namespace MyGiftCard
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            container
  .RegisterType<IMyGiftCardService, MyGiftCardService>()
  .RegisterType<IEncryptionUtil, EncryptionUtil>()
  .RegisterType<IMyGiftCardController, MyGiftCardController>();
			// register all your components with the container here
            // container
            //    .RegisterType<IService1, Service1>()
            //    .RegisterType<DataContext>(new HierarchicalLifetimeManager());
        }
    }    
}