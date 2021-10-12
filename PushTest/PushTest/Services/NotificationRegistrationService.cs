using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PushTest.Models;
using Xamarin.Essentials;
using System.IO;
using System.Net;

namespace PushTest.Services
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        const string RequestUrl = "api/notifications/installations";
        const string CachedDeviceTokenKey = "cached_device_token";
        const string CachedTagsKey = "cached_tags";

        string _baseApiUrl;
        HttpClient _client;
        IDeviceInstallationService _deviceInstallationService;

        public NotificationRegistrationService(string baseApiUri)// string apiKey
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            //_client.DefaultRequestHeaders.Add("apikey", apiKey);

            _baseApiUrl = baseApiUri;
        }

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService = ServiceContainer.Resolve<IDeviceInstallationService>());

        public async Task DeregisterDeviceAsync()
        {
            var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
                .ConfigureAwait(false);

            if (cachedToken == null)
                return;

            var deviceId = DeviceInstallationService?.GetDeviceId();

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new Exception("Unable to resolve an ID for the device.");

            await SendAsync(HttpMethod.Delete, $"{RequestUrl}/{deviceId}")
                .ConfigureAwait(false);

            SecureStorage.Remove(CachedDeviceTokenKey);
            SecureStorage.Remove(CachedTagsKey);
        }

        public async Task RegisterDeviceAsync(params string[] tags)
        {
            var deviceInstallation = DeviceInstallationService?.GetDeviceInstallation(tags);

            await SendAsync<DeviceInstallation>(HttpMethod.Put, RequestUrl, deviceInstallation)
                .ConfigureAwait(false);

            await SecureStorage.SetAsync(CachedDeviceTokenKey, deviceInstallation.PushChannel)
                .ConfigureAwait(false);

            await SecureStorage.SetAsync(CachedTagsKey, JsonConvert.SerializeObject(tags));
        }

        public async Task RefreshRegistrationAsync()
        {
            var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
                .ConfigureAwait(false);

            var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(cachedToken) ||
                string.IsNullOrWhiteSpace(serializedTags) ||
                string.IsNullOrWhiteSpace(DeviceInstallationService.Token) ||
                cachedToken == DeviceInstallationService.Token)
                return;

            var tags = JsonConvert.DeserializeObject<string[]>(serializedTags);

            await RegisterDeviceAsync(tags);
        }

        async Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
        {
            string serializedContent = null;

            await Task.Run(() => serializedContent = JsonConvert.SerializeObject(obj))
                .ConfigureAwait(false);

            await SendAsync(requestType, requestUri, serializedContent);
        }

        async Task SendAsync(
            HttpMethod requestType,
            string requestUri,
            string jsonRequest = null)
        {
            var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));
            if (jsonRequest != null)
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            //_client.BaseAddress = new Uri("http://192.168.0.104:52353");
            //_client.BaseAddress = new Uri("http://10.0.2.2:52353");
            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            //string rUri = $"/{requestUri}";
            //HttpResponseMessage response = await _client.PutAsync(rUri, content);
            //var result = await response.Content.ReadAsStringAsync();

            var response = await _client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        //private void RegisterOrUpdateDevice<T>(string endpoint, T obj)
        //{
        //    try
        //    {
        //        var jsonResult = "";
        //        WebRequest oRequest = WebRequest.Create(endpoint);
        //        oRequest.Method = "PUT";
        //        oRequest.ContentType = "application/json";
        //        using (var oSW = new StreamWriter(oRequest.GetRequestStream()))
        //        {
        //            string jsonRequest = JsonConvert.SerializeObject(obj);
        //            oSW.Write(jsonRequest);
        //            oSW.Flush();
        //            oSW.Close();
        //        }
        //        //oRequest.Headers.Add("Authorization", "Bearer " + token);
        //        WebResponse oResponse = oRequest.GetResponse();
        //        using (var oSR = new StreamReader(oResponse.GetResponseStream()))
        //        {
        //            jsonResult = oSR.ReadToEnd().Trim();
        //            var res = JsonConvert.DeserializeObject(jsonResult);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }

        ////}
    }
}

 