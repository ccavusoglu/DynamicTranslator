﻿#region using

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTranslator.Core.Extensions;
using DynamicTranslator.Core.Orchestrators.Model;
using DynamicTranslator.Core.ViewModel.Constants;
using HtmlAgilityPack;

#endregion

namespace DynamicTranslator.Orchestrators.Organizers
{

    #region using

    #endregion

    public class SesliSozlukMeanOrganizer : AbstractMeanOrganizer
    {
        public override TranslatorType TranslatorType => TranslatorType.Seslisozluk;

        public override async Task<Maybe<string>> OrganizeMean(string text, string fromLanguageExtension)
        {
            return await Task.Run(() =>
            {
                var output = new StringBuilder();

                var document = new HtmlDocument();
                document.LoadHtml(text);

                (from x in document.DocumentNode.Descendants()
                 where x.Name == "pre"
                 from y in x.Descendants()
                 where y.Name == "ol"
                 from z in y.Descendants()
                 where z.Name == "li"
                 select z.InnerHtml)
                    .AsParallel()
                    .ToList()
                    .ForEach(mean => output.AppendLine(mean));

                if (string.IsNullOrEmpty(output.ToString()))
                {
                    (from x in document.DocumentNode.Descendants()
                     where x.Name == "pre"
                     from y in x.Descendants()
                     where y.Name == "span"
                     select y.InnerHtml)
                        .AsParallel()
                        .ToList()
                        .ForEach(mean => output.AppendLine(mean.StripTagsCharArray()));
                }

                return new Maybe<string>(output.ToString());
            });
        }
    }
}
