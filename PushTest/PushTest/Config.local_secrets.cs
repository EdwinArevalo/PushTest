﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PushTest
{
    public static partial class Config
    {
        static Config()
        {
            //ApiKey = "<your_api_key>";
            //BackendServiceEndpoint = "https://192.168.0.104:44378/";
            //BackendServiceEndpoint = "http://192.168.0.104:52353/";
            BackendServiceEndpoint = "http://10.0.2.2:52353/";
        }
    }
}
