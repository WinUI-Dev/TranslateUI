using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TranslateUI
{
    public sealed partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
        }

        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            PB_Wait.Visibility = Visibility.Visible;

            string inputText = TB_Input.Text;
            string selectedOutputLang = CB_OutputLang.SelectedItem?.ToString();
            string realOutputLang = GetLanguageCode(selectedOutputLang);

            if (!string.IsNullOrEmpty(realOutputLang))
            {
                try
                {
                    string responseBody = await SendPostRequest(realOutputLang, inputText).ConfigureAwait(false);
                    string translatedText = GetTranslatedText(responseBody);

                    // 使用 DispatcherQueue 在 UI 线程上更新控件
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        TB_Output.Text = translatedText;
                        PB_Wait.Visibility = Visibility.Collapsed;
                    });
                }
                catch (COMException comEx)
                {
                    Debug.WriteLine($"COMException: {comEx.Message}");
                    Debug.WriteLine($"COMException StackTrace: {comEx.StackTrace}");
                    // 使用 DispatcherQueue 在 UI 线程上更新控件
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        PB_Wait.ShowError = true;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception: {ex.Message}");
                    Debug.WriteLine($"Exception StackTrace: {ex.StackTrace}");
                    // 使用 DispatcherQueue 在 UI 线程上更新控件
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        PB_Wait.ShowError = true;
                    });
                }
            }
            else
            {
                Debug.WriteLine("Error: Invalid language selection.");
                // 使用 DispatcherQueue 在 UI 线程上更新控件
                _ = DispatcherQueue.TryEnqueue(() =>
                {
                    PB_Wait.ShowError = true;
                });
            }
        }

        private void CB_OutputLang_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TranslateButton.IsEnabled = CB_OutputLang.SelectedItem != null;
        }

        private string GetLanguageCode(string language)
        {
            switch (language)
            {
                case "English":
                    return "en";
                case "Chinese (Simplified)":
                    return "zh";
                default:
                    return null;
            }
        }

        public string GetTranslatedText(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        JsonElement firstElement = root[0];
                        if (firstElement.TryGetProperty("translations", out JsonElement translations) && translations.ValueKind == JsonValueKind.Array && translations.GetArrayLength() > 0)
                        {
                            JsonElement firstTranslation = translations[0];
                            if (firstTranslation.TryGetProperty("text", out JsonElement text))
                            {
                                return text.GetString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing JSON: {ex.Message}");
                return string.Empty;
            }

            Debug.WriteLine("Error: Translation empty.");
            return string.Empty;
        }

        private async Task<string> SendPostRequest(string translateToLang, string content)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("accept", "*/*");
            client.DefaultRequestHeaders.Add("origin", "https://github.com");
            client.DefaultRequestHeaders.Add("referer", "https://github.com/zsr-lukezhang/TranslateUI/");
            client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br, zstd");
            client.DefaultRequestHeaders.Add("priority", "u=1");

            var requestContent = new StringContent($"[\"{content}\"]", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"https://edge.microsoft.com/translate/translatetext?from=&to={translateToLang}", requestContent).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}