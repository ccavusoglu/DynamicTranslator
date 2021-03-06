﻿#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTranslator.Core.Orchestrators.Model;
using DynamicTranslator.Core.Orchestrators.Organizer;
using DynamicTranslator.Core.Service.Result;

#endregion

namespace DynamicTranslator.Orchestrators.Organizers
{
    public class ResultOrganizer : IResultOrganizer
    {
        private readonly IResultService resultService;

        public ResultOrganizer(IResultService resultService)
        {
            if (resultService == null)
                throw new ArgumentNullException(nameof(resultService));

            this.resultService = resultService;
        }

        public async Task<Maybe<string>> OrganizeResult(ICollection<TranslateResult> findedMeans, string currentString)
        {
            var mean = new StringBuilder();
            foreach (var result in findedMeans.Where(result => result.IsSucess))
            {
                mean.AppendLine(result.ResultMessage.DefaultIfEmpty(string.Empty).First());
            }

            if (!string.IsNullOrEmpty(mean.ToString()))
            {
                var means = mean.ToString().Split('\r')
                    .Select(x => x.Trim().ToLower())
                    .Where(s => s != string.Empty && s != currentString.Trim() && s != "Translation")
                    .Distinct()
                    .ToList();

                mean.Clear();
                means.ForEach(m => mean.AppendLine("* " + m.ToLower()));
                await resultService.SaveAndUpdateFrequencyAsync(currentString, new CompositeTranslateResult(currentString, 1, findedMeans, DateTime.Now));
                return new Maybe<string>(mean.ToString());
            }

            return new Maybe<string>();
        }
    }
}
