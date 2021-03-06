﻿namespace DynamicTranslator.Orchestrators.Detector
{
    #region using

    using System;
    using System.Net.Cache;
    using System.Text;
    using System.Threading.Tasks;
    using DynamicTranslator.Core.Config;
    using DynamicTranslator.Core.Orchestrators.Detector;
    using DynamicTranslator.Core.Orchestrators.Model.Yandex;
    using Newtonsoft.Json;
    using RestSharp;

    #endregion

    public class YandexLanguageDetector : ILanguageDetector
    {
        private readonly IStartupConfiguration configuration;

        public YandexLanguageDetector(IStartupConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> DetectLanguage(string text)
        {
            var uri = string.Format(configuration.YandexDetectTextUrl, text);

            var response = await new RestClient(uri)
            {
                Encoding = Encoding.UTF8,
                CachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.FromHours(1))
            }.ExecuteGetTaskAsync(new RestRequest(Method.GET)
                .AddHeader("cache-control", "no-cache")
                .AddHeader("accept-language", "en-US,en;q=0.8,tr;q=0.6")
                .AddHeader("accept-encoding", "gzip, deflate, sdch")
                .AddHeader("accept", "*/*")
                .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36"));

            var result = JsonConvert.DeserializeObject<YandexDetectResponse>(response.Content);
            if (result != null && string.IsNullOrEmpty(result.Lang))
                return result.Lang;

            return configuration.ToLanguageExtension;
        }
    }
}
