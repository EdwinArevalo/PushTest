using System;
using System.Collections.Generic;
using System.Text;
using PushTest.Models;

namespace PushTest.Services
{
    public interface IPushDemoNotificationActionService : INotificationActionService
    {
        event EventHandler<PushDemoAction> ActionTriggered;
    }
}
