using HCSMeta.Activity;

namespace HCSMeta.Function.Injection
{
    public interface ISelectedNotificationInjectable
    {
        void Inject(ISelectedNotification selectedNotification);
    }
}
