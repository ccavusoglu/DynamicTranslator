﻿#region using

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DynamicTranslator.Core.Config;
using DynamicTranslator.Core.Dependency.Manager;
using DynamicTranslator.Core.Orchestrators;
using DynamicTranslator.Core.ViewModel.Constants;
using DynamicTranslator.ViewModel.Model;

#endregion

namespace DynamicTranslator.ViewModel
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            IocManager.Instance.Register(typeof (MainWindow), this);
            translator = IocManager.Instance.Resolve<ITranslatorBootstrapper>();
            translator.SubscribeShutdownEvents();
            configuration = IocManager.Instance.Resolve<IStartupConfiguration>();
            foreach (var language in configuration.LanguageMap)
            {
                ComboBoxLanguages.Items.Add(new Language(language.Key, language.Value));
            }

            ComboBoxLanguages.SelectedValue = configuration.ToLanguageExtension;
        }

        private readonly IStartupConfiguration configuration;
        private readonly ITranslatorBootstrapper translator;
        private bool isRunning;

        protected override void OnClosing(CancelEventArgs e)
        {
            Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            base.OnClosing(e);
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                BtnSwitch.Content = "Start Translator";

                isRunning = false;

                UnlockUiElements();

                translator.Dispose();
            }
            else
            {
                BtnSwitch.Content = "Stop Translator";

                configuration.SetAndPersistConfigurationManager(
                    nameof(configuration.ToLanguage),
                    ((Language) ComboBoxLanguages.SelectedItem).Name);

                PrepareTranslators();
                LockUiElements();

                Task.Run(
                    async () =>
                    {
                        await Dispatcher.InvokeAsync(
                            async () =>
                            {
                                if (!translator.IsInitialized)
                                    await translator.InitializeAsync();
                            });
                    });

                isRunning = true;
            }
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6U2T5SPVDZ7TW");
        }

        private void GithubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/osoykan/DynamicTranslator");
        }

        private void LockUiElements()
        {
            ComboBoxLanguages.Focusable = false;
            ComboBoxLanguages.IsHitTestVisible = false;
            CheckBoxGoogleTranslate.IsHitTestVisible = false;
            CheckBoxTureng.IsHitTestVisible = false;
            CheckBoxYandexTranslate.IsHitTestVisible = false;
            CheckBoxSesliSozluk.IsHitTestVisible = false;
            CheckBoxZargan.IsHitTestVisible = false;
            CheckBoxBing.IsHitTestVisible = false;
        }

        private void PrepareTranslators()
        {
            configuration.ClearActiveTranslators();

            if (CheckBoxGoogleTranslate.IsChecked != null && CheckBoxGoogleTranslate.IsChecked.Value)
                configuration.AddTranslator(TranslatorType.Google);
            if (CheckBoxYandexTranslate.IsChecked != null && CheckBoxYandexTranslate.IsChecked.Value)
                configuration.AddTranslator(TranslatorType.Yandex);
            if (CheckBoxTureng.IsChecked != null && CheckBoxTureng.IsChecked.Value)
                configuration.AddTranslator(TranslatorType.Tureng);
            if (CheckBoxSesliSozluk.IsChecked != null && CheckBoxSesliSozluk.IsChecked.Value)
                configuration.AddTranslator(TranslatorType.Seslisozluk);
            if (CheckBoxBing.IsChecked != null && CheckBoxBing.IsChecked.Value)
                configuration.AddTranslator(TranslatorType.Bing);
            if (CheckBoxZargan.IsChecked != null && CheckBoxZargan.IsChecked.Value)
                configuration.AddTranslator(TranslatorType.Zargan);

            if (!configuration.ActiveTranslators.Any())
            {
                foreach (var value in Enum.GetValues(typeof (TranslatorType)))
                {
                    configuration.AddTranslator((TranslatorType) value);
                }
            }
        }

        private void UnlockUiElements()
        {
            ComboBoxLanguages.Focusable = true;
            ComboBoxLanguages.IsHitTestVisible = true;
            CheckBoxGoogleTranslate.IsHitTestVisible = true;
            CheckBoxTureng.IsHitTestVisible = true;
            CheckBoxYandexTranslate.IsHitTestVisible = true;
            CheckBoxSesliSozluk.IsHitTestVisible = true;
            CheckBoxZargan.IsHitTestVisible = true;
            CheckBoxBing.IsHitTestVisible = true;
        }
    }
}
