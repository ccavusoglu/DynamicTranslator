﻿#region using

using System.Threading.Tasks;
using System.Xml;
using DynamicTranslator.Core.Orchestrators.Model;
using DynamicTranslator.Core.ViewModel.Constants;

#endregion

namespace DynamicTranslator.Orchestrators.Organizers
{
    public class YandexMeanOrganizer : AbstractMeanOrganizer
    {
        public override TranslatorType TranslatorType => TranslatorType.Yandex;

        public override async Task<Maybe<string>> OrganizeMean(string text, string fromLanguageExtension)
        {
            return await Task.Run(() =>
            {
                if (text == null) return new Maybe<string>();

                var doc = new XmlDocument();
                doc.LoadXml(text);
                var node = doc.SelectSingleNode("//Translation/text");
                var output = node?.InnerText ?? "!!! An error occured";

                return new Maybe<string>(output.ToLower().Trim());
            });
        }
    }
}
