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
            TranslateButton.Content = "Clicked";
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
            }

            return string.Empty;
        }
    }
}
