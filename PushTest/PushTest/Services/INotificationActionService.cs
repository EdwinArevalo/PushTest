using System;
using System.Collections.Generic;
using System.Text;

namespace PushTest.Services
{
    public interface INotificationActionService
    {
        void TriggerAction(string action);
    }
}
