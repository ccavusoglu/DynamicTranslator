﻿namespace DynamicTranslator.Orchestrators.Finders
{
    #region using

    using System;
    using System.Linq;
    using System.Net.Cache;
    using System.Text;
    using System.Threading.Tasks;
    using DynamicTranslator.Core.Config;
    using DynamicTranslator.Core.Orchestrators.Finder;
    using DynamicTranslator.Core.Orchestrators.Model;
    using DynamicTranslator.Core.Orchestrators.Organizer;
    using DynamicTranslator.Core.ViewModel.Constants;
    using Newtonsoft.Json;
    using RestSharp;

    #endregion

    public class BingTranslatorFinder : IMeanFinder
    {
        private readonly IStartupConfiguration configuration;
        private readonly IMeanOrganizerFactory meanOrganizerFactory;

        public BingTranslatorFinder(IStartupConfiguration configuration, IMeanOrganizerFactory meanOrganizerFactory)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (meanOrganizerFactory == null)
                throw new ArgumentNullException(nameof(meanOrganizerFactory));

            this.configuration = configuration;
            this.meanOrganizerFactory = meanOrganizerFactory;
        }

        public TranslatorType TranslatorType => TranslatorType.Bing;

        public async Task<TranslateResult> Find(TranslateRequest translateRequest)
        {
            if (!configuration.IsAppropriateForTranslation(TranslatorType, translateRequest.FromLanguageExtension))
                return new TranslateResult(false, new Maybe<string>());

            var requestObject = new
            {
                from = configuration.FromLanguage.ToLower(),
                to = configuration.ToLanguage.ToLower(),
                text = translateRequest.CurrentText
            };

            var response = await new RestClient(configuration.BingTranslatorUrl)
            {
                Encoding = Encoding.UTF8,
                CachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.FromHours(1))
            }.ExecutePostTaskAsync(new RestRequest(Method.POST)
                .AddHeader("content-type", "application/json;Charset=UTF-8")
                .AddParameter("application/json;Charset=UTF-8",
                    JsonConvert.SerializeObject(requestObject),
                    ParameterType.RequestBody));

            var meanOrganizer = meanOrganizerFactory.GetMeanOrganizers().First(x => x.TranslatorType == TranslatorType);
            var mean = await meanOrganizer.OrganizeMean(response.Content);

            return new TranslateResult(true, mean);
        }
    }
}
