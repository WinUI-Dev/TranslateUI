using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TranslateUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("translations")]
        public Translation[] Translations { get; set; }
    }
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Enable custom title bar
            this.ExtendsContentIntoTitleBar = true;

        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = TB_Input.Text;
            string selectedInputLang = CB_InputLang.SelectedItem.ToString();
            string selectedOutputLang = CB_OutputLang.SelectedItem.ToString();
            string RealInputLang = "";
            string RealOutputLang = "";
            if (selectedInputLang == "English")
            {
                RealInputLang = "en-US";
            }
            else if (selectedInputLang == "Chinsese (Simplified)")
            {
                RealOutputLang = "zh-Hans";
            }
            else
            {
                Debug.WriteLine("Error");
                return;
            }

        }

        private void CB_OutputLang_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CB_InputLang_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        public string GetTranslatedText(string json)
        {
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var root = System.Text.Json.JsonSerializer.Deserialize<Root[]>(json, options);
                if (root != null && root.Length > 0 && root[0].Translations.Length > 0)
                {
                    return root[0].Translations[0].Text;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Debug.WriteLine($"Error parsing JSON: {ex.Message}");
                return string.Empty;
            }

            // No translation found
            Debug.WriteLine("Error: Translation empty.");
            return string.Empty;
        }

        private async void SendPostRequest(string TranslateToLang, string Content)
        {
            using (HttpClient client = new HttpClient())
            {
                // 设置请求头
                // 根据 @lingbopro 的研究，没什么用
                // 但是还是设置一下
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("sec-mesh-client-edge-version", "131.0.2903.70");
                client.DefaultRequestHeaders.Add("sec-mesh-client-edge-channel", "stable");
                client.DefaultRequestHeaders.Add("sec-mesh-client-os", "Windows");
                client.DefaultRequestHeaders.Add("sec-mesh-client-os-version", "10.0.26100");
                client.DefaultRequestHeaders.Add("sec-mesh-client-arch", "x86_64");
                client.DefaultRequestHeaders.Add("sec-mesh-client-webview", "0");

                // 瞎写的网站
                client.DefaultRequestHeaders.Add("origin", "https://github.com");
                client.DefaultRequestHeaders.Add("x-edge-shopping-flag", "1");
                client.DefaultRequestHeaders.Add("sec-fetch-site", "cross-site");
                client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");

                // 瞎写的地址
                client.DefaultRequestHeaders.Add("referer", "https://github.com/zsr-lukezhang/TranslateUI/");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br, zstd");

                // 这一行就不要了
                // 省得影响输出内容
                // client.DefaultRequestHeaders.Add("accept-language", "en,zh-CN;q=0.9,zh;q=0.8,en-GB;q=0.7,en-US;q=0.6");
                client.DefaultRequestHeaders.Add("priority", "u=1");

                // 请求Body
                var content = new StringContent($"{{\"message\":\"{Content}\"}}", Encoding.UTF8, "application/json");
                // 发送 POST 请求
                HttpResponseMessage response = await client.PostAsync("https://https://edge.microsoft.com/translate/translatetext?from=&to=", content);

                // 处理响应
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
        }
    }
}
