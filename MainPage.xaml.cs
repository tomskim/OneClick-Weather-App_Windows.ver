using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows;
using Windows.UI.Popups;
using Windows.UI.Core;

namespace CityWeather
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        int casetype1 = 0;

        int errorfree = 1;

        private async void celsius_Checked(object sender, RoutedEventArgs e)
        {
            if (casetype1 == 0)
            {
                casetype1 = 1;
            }
            else if (casetype1 == 1)
            {
                casetype1 = 0;
            }
            else
            {
                var dialog = new MessageDialog("You cannot assign more than one unit!");
                await dialog.ShowAsync();
                celsius.IsChecked = false;
            }
        }

        private void celsius_Unchecked(object sender, RoutedEventArgs e)
        {
            if (casetype1 == 1)
            {
                casetype1 = 0;
            }
        }
            

        private async void fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            if (casetype1 == 0)
            {
                casetype1 = 2;
            }
            else if (casetype1 == 2)
            {
                casetype1 = 0;
            }
            else
            {
                var dialog = new MessageDialog("You cannot assign more than one unit!");
                await dialog.ShowAsync();
                fahrenheit.IsChecked = false;
            }
        }

        private void fahrenheit_Unchecked(object sender, RoutedEventArgs e)
        {
            if (casetype1 == 2)
            {
                casetype1 = 0;
            }
        }

        private async void kelvin_Checked(object sender, RoutedEventArgs e)
        {
            if (casetype1 == 0)
            {
                casetype1 = 3;
            }
            else if (casetype1 == 3)
            {
                casetype1 = 0;
            }
            else
            {
                var dialog = new MessageDialog("You cannot assign more than one unit!");
                await dialog.ShowAsync();
                kelvin.IsChecked = false;
            }
        }

        private void kelvin_Unchecked(object sender, RoutedEventArgs e)
        {
            if (casetype1 == 3)
            {
                casetype1 = 0;
            }
        }

        private void textbox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tbSender = (TextBox)sender;

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                button_Click(sender, new RoutedEventArgs());
            }
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
      
            if ((string.IsNullOrWhiteSpace(city.Text)) || (string.IsNullOrWhiteSpace(country.Text)))
            {
                var dialog = new MessageDialog("One or more text fields are empty!");
                await dialog.ShowAsync();
            }
            else
            {
                try
                {
                    string url_test = "http://api.openweathermap.org/data/2.5/weather?APPID=3e9539d1f7a9e3882e6998d0222fed52&q=" + city.Text + "," + country.Text;

                    HttpClient client_test = new HttpClient();

                    string stringdate_test = await client_test.GetStringAsync(new Uri(url_test));

                    RootObject data_test = JsonConvert.DeserializeObject<RootObject>(stringdate_test);
                }

                catch (JsonReaderException)
                {
                    errorfree = 0;
                    var dialog = new MessageDialog("Please enter valid city/country names!");
                    await dialog.ShowAsync();
                }
                
                catch (HttpRequestException)
                {
                    errorfree = 0;
                    var dialog = new MessageDialog("Please make sure that you are connected to Internet!");
                    await dialog.ShowAsync();
                }

                
                if(errorfree == 1)
                {
                    string url = "http://api.openweathermap.org/data/2.5/weather?APPID=3e9539d1f7a9e3882e6998d0222fed52&q=" + city.Text + "," + country.Text;

                    HttpClient client = new HttpClient();

                    string stringdate = await client.GetStringAsync(new Uri(url));

                    RootObject data = JsonConvert.DeserializeObject<RootObject>(stringdate);

                    try
                    {
                        string url_time = "http://api.timezonedb.com/?format=json&key=04WHGJMVG1LY&lat=" + data.coord.lat + "&lng=" + data.coord.lon;

                        HttpClient client_time = new HttpClient();

                        string stringdate_time = await client_time.GetStringAsync(new Uri(url_time));

                        RootObject_time data_time = JsonConvert.DeserializeObject<RootObject_time>(stringdate_time);
                    }


                    catch (NullReferenceException)
                    {
                        errorfree = 0;
                        var dialog = new MessageDialog("Please enter valid city/country names!");
                        await dialog.ShowAsync();
                    }

                    catch (HttpRequestException)
                    {
                        errorfree = 0;
                        var dialog = new MessageDialog("Search limit reached; please wait a few seconds!");
                        await dialog.ShowAsync();
                    }

                    if (casetype1 == 0)
                    {
                        var dialog = new MessageDialog("Please check a specific unit!");
                        await dialog.ShowAsync();
                    }
                    else if (errorfree == 1)
                    {
                        string url_time = "http://api.timezonedb.com/?format=json&key=04WHGJMVG1LY&lat=" + data.coord.lat + "&lng=" + data.coord.lon;

                        HttpClient client_time = new HttpClient();

                        string stringdate_time = await client_time.GetStringAsync(new Uri(url_time));

                        RootObject_time data_time = JsonConvert.DeserializeObject<RootObject_time>(stringdate_time);

                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                        dateTime = dateTime.AddSeconds(data_time.timestamp);

                        result1.Text = string.Concat(dateTime.Hour.ToString("00"), ":", dateTime.Minute.ToString("00"));

                        if (casetype1 == 1)
                        {
                            result2.Text = Math.Round((data.main.temp - 273.15), 1).ToString() + "°C";
                            result4.Text = Math.Round((data.main.temp_max - 273.15), 1).ToString() + "°C";
                            result5.Text = Math.Round((data.main.temp_min - 273.15), 1).ToString() + "°C";
                        }
                        else if (casetype1 == 2)
                        {
                            result2.Text = Math.Round(((data.main.temp * 9) / 5 - 459.67), 1).ToString() + "°F";
                            result4.Text = Math.Round(((data.main.temp_max * 9) / 5 - 459.67), 1).ToString() + "°F";
                            result5.Text = Math.Round(((data.main.temp_min * 9) / 5 - 459.67), 1).ToString() + "°F";
                        }
                        else if (casetype1 == 3)
                        {
                            result2.Text = Math.Round(data.main.temp, 1).ToString() + "K";
                            result4.Text = Math.Round(data.main.temp_max, 1).ToString() + "K";
                            result5.Text = Math.Round(data.main.temp_min, 1).ToString() + "K";
                        }

                        image.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("http://openweathermap.org/img/w/" + data.weather[0].icon + ".png", UriKind.Absolute));

                        result3.Text = data.weather[0].main;

                        result6.Text = data.main.humidity.ToString() + "%";

                        result7.Text = data.weather[0].description;
              
                    }
                    
                }
                
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBlock4.Visibility = Visibility.Visible;
            textBlock5.Visibility = Visibility.Visible;
            textBlock6.Visibility = Visibility.Visible;
            textBlock7.Visibility = Visibility.Visible;
            result4.Visibility = Visibility.Visible;
            result5.Visibility = Visibility.Visible;
            result6.Visibility = Visibility.Visible;
            result7.Visibility = Visibility.Visible;
            textBlock1.Visibility = Visibility.Collapsed;
            textBlock2.Visibility = Visibility.Collapsed;
            textBlock3.Visibility = Visibility.Collapsed;
            result1.Visibility = Visibility.Collapsed;
            result2.Visibility = Visibility.Collapsed;
            result3.Visibility = Visibility.Collapsed;
            button1.Visibility = Visibility.Collapsed;
            button2.Visibility = Visibility.Visible;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Visibility = Visibility.Visible;
            textBlock2.Visibility = Visibility.Visible;
            textBlock3.Visibility = Visibility.Visible;
            result1.Visibility = Visibility.Visible;
            result2.Visibility = Visibility.Visible;
            result3.Visibility = Visibility.Visible;
            textBlock4.Visibility = Visibility.Collapsed;
            textBlock5.Visibility = Visibility.Collapsed;
            textBlock6.Visibility = Visibility.Collapsed;
            textBlock7.Visibility = Visibility.Collapsed;
            result4.Visibility = Visibility.Collapsed;
            result5.Visibility = Visibility.Collapsed;
            result6.Visibility = Visibility.Collapsed;
            result7.Visibility = Visibility.Collapsed;
            button1.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Collapsed;
        }
    }

    // json structure for weather api
    // note that the json structure changes oftentime by the provider, so it might need to be updated periodically.

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public double deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public double message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class RootObject
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    // json structure for time zone api

    public class RootObject_time
    {
        public string status { get; set; }
        public string message { get; set; }
        public string countryCode { get; set; }
        public string zoneName { get; set; }
        public string abbreviation { get; set; }
        public string gmtOffset { get; set; }
        public string dst { get; set; }
        public int timestamp { get; set; }
    }
}
