
//
//
//
//
//                                                 TranslateUI
//                                      by Luke Zhang (GitHub@zsr-lukezhang)
//                                   original idea by Lingbo (GitHub@lingbopro)
//                                      Application Version: 1.1.3 ( Canary )
//
//
//

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.ApplicationModel;
using Windows.Storage;
using System.IO;
using Microsoft.UI.Windowing;

namespace TranslateUI
{
    public sealed partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetWindowIcon("Assets/TranslateUI.ico");
        }

        private void SetWindowIcon(string iconPath)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            var iconFullPath = Path.Combine(Package.Current.InstalledLocation.Path, iconPath);
            appWindow.SetIcon(iconFullPath);
        }

        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            PB_Wait.ShowError = false;
            PB_Wait.Visibility = Visibility.Visible;
            TranslateButton.IsEnabled = false;
            TB_Input.IsEnabled = false;
            TB_Output.Text = string.Empty;

            string inputText = TB_Input.Text;
            string selectedOutputLang = CB_OutputLang.SelectedItem?.ToString();
            string realOutputLang = GetLanguageCode(selectedOutputLang);

            if (inputText == "_Dev_Mode_")
            {
                await Task.Delay(1000);
                ShowError("Hello, world!", "Emm...\nIt seems like you are a software developer...\nPlease join us to fix and add more features to this app!");
                // 使用 DispatcherQueue 在 UI 线程上更新控件
                _ = DispatcherQueue.TryEnqueue(() =>
                {
                    TB_Output.Text = "Hello, world!";
                    TranslateButton.IsEnabled = true;
                    TB_Input.IsEnabled = true;
                    PB_Wait.ShowError = true;
                });

                return;
            }

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
                        TranslateButton.IsEnabled = true;
                        PB_Wait.ShowError = true;
                        TB_Input.IsEnabled = true;
                    });
                }
                catch (COMException comEx)
                {
                    // 使用 DispatcherQueue 在 UI 线程上更新控件
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        PB_Wait.ShowError = true;
                        TranslateButton.IsEnabled = true;
                        TB_Input.IsEnabled = true;
                    });
                    Debug.WriteLine($"COMException: {comEx.Message}");
                    Debug.WriteLine($"COMException StackTrace: {comEx.StackTrace}");
                    ShowError("COM Exception", $"Exception:\n{comEx.Message}\nException StackTrace:\n{comEx.StackTrace}");
                }
                catch (Exception ex)
                {
                    // 使用 DispatcherQueue 在 UI 线程上更新控件
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        PB_Wait.ShowError = true;
                        TranslateButton.IsEnabled = true;
                        TB_Input.IsEnabled = true;
                    });
                    Debug.WriteLine($"Exception: {ex.Message}");
                    Debug.WriteLine($"Exception StackTrace: {ex.StackTrace}");
                    ShowError("Exception", $"Exception:\n{ex.Message}\nException StackTrace:\n{ex.StackTrace}");
                }
            }
            else
            {
                Debug.WriteLine("Error: Invalid language selection.");
                ShowError("Error", "Invalid language selection.");

                // 使用 DispatcherQueue 在 UI 线程上更新控件
                _ = DispatcherQueue.TryEnqueue(() =>
                {
                    PB_Wait.ShowError = true;
                    TranslateButton.IsEnabled = true;
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
                ShowError("Error Parsing JSON", ex.Message);
                return string.Empty;
            }

            Debug.WriteLine("Error: Translation empty.");
            ShowError("Error", "Translation Empty");
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

        private void ShowError(string title, string content)
        {
            var dispatcherQueue = this.DispatcherQueue;
            if (dispatcherQueue != null)
            {
                _ = dispatcherQueue.TryEnqueue(async () =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = title,
                        Content = $":(\nYou reached an error.\nYou can try to solve this problem yourself,\nor post a new issue with these logs on GitHub in the issues page.\n===================================\n{content}",
                        XamlRoot = this.Content.XamlRoot, // 设置 XamlRoot
                        // 这个长宽我不知道怎么调 需要帮助
                        // Need help with the width and height of the Content Dialog
                        Width = 400, // 设置宽度
                        Height = 300, // 设置高度
                        PrimaryButtonText = "Not your problem",
                        SecondaryButtonText = "Copy to clipboard",
                        CloseButtonText = "Copy & open issues",
                        DefaultButton = ContentDialogButton.Primary,
                        PrimaryButtonStyle = new Style(typeof(Button))
                        {
                            Setters =
                        {
                            new Setter(Button.BackgroundProperty, new SolidColorBrush(Microsoft.UI.Colors.Blue)),
                            new Setter(Button.ForegroundProperty, new SolidColorBrush(Microsoft.UI.Colors.White)),
                            new Setter(Button.ForegroundProperty, new SolidColorBrush(Microsoft.UI.Colors.White))
                        }
                        }
                    };

                    dialog.SecondaryButtonClick += (sender, args) =>
                    {
                        var dataPackage = new DataPackage();
                        dataPackage.SetText($"Error title:\n{title}\nError Message:\n{content}");
                        Clipboard.SetContent(dataPackage);
                    };

                    dialog.CloseButtonClick += (sender, args) =>
                    {
                        var dataPackage = new DataPackage();
                        dataPackage.SetText($"Error title:\n{title}\nError Message:\n{content}");
                        Clipboard.SetContent(dataPackage);
                        OpenBrowser("https://github.com/zsr-lukezhang/TranslateUI/issues");
                    };

                    await dialog.ShowAsync();
                });
            }
            else
            {
                // 处理 DispatcherQueue 为 null 的情况
                // 我不知道怎么处理（
                // Luke Zhang
                // 2024/12/09
            }
        }

        private void OpenBrowser(string URL)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = URL,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            { 
                Debug.WriteLine("Error starting browser:" + ex.Message);
            }
        }

        private void BT_About_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}