using PushTest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PushTest.Services
{
    public interface IDeviceInstallationService
    {
        string Token { get; set; }
        bool NotificationsSupported { get; }
        string GetDeviceId();
        DeviceInstallation GetDeviceInstallation(params string[] tags);
    }
} 