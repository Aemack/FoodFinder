using System;
using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Yelp.Api.Models;
using Windows.Media.Devices;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Yelper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        List<BussinessInfo> buss = new List<BussinessInfo>();
        int busId;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BigButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        public async void Search()
        {
            buss.Clear();
            string location = LocationSearchBox.Text;
            Location coords = GetCoords(location);

            string cuisine = CuisineSearchBox.Text;
            var client = new Yelp.Api.Client("[KEY HERE]");
            var results = await client.SearchBusinessesAllAsync(cuisine, coords.Lat, coords.Long);

            foreach (object business in results.Businesses)
            {
                BussinessInfo bus = new BussinessInfo();
                bus.Name = ((Yelp.Api.Models.BusinessResponse)business).Name;
                bus.Rating = ((Yelp.Api.Models.BusinessResponse)business).Rating.ToString();
                bus.Phone = ((Yelp.Api.Models.BusinessResponse)business).Phone;
                bus.Url = ((Yelp.Api.Models.BusinessResponse)business).Url;
                bus.Price = ((Yelp.Api.Models.BusinessResponse)business).Price;
                if (bus.Price == null)
                {
                    bus.Price = " ";
                }
                bus.ImageUrl = ((Yelp.Api.Models.BusinessResponse)business).ImageUrl;
                buss.Add(bus);
            }
            busId = 1;
            DisplayPanel.Background = new SolidColorBrush(Windows.UI.Colors.White);
            DisplayName.Text = buss[busId].Name;
            DisplayRating.Text = buss[busId].Rating + " Stars";
            DisplayPhone.Text = buss[busId].Phone;
            DisplayImage.Source = new BitmapImage(new Uri(buss[busId].ImageUrl));
            Uri myUri = new Uri(buss[busId].Url, UriKind.Absolute);
            SiteLink.NavigateUri = myUri;
            SiteLink.Content = "Check It Out!";
            DisplayPrice.Text = buss[busId].Price;
        }

        public Location GetCoords(string place)
        {
            Location searchLocation = new Location();
            string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + place + "&key=AIzaSyCRZqmvOXjKLKBCY-nAfSGc30w0sbnhsUg";

            var client = new RestClient("https://maps.googleapis.com/maps/api/");

            var request = new RestRequest("geocode/json?address=" + place + "&key=AIzaSyCRZqmvOXjKLKBCY-nAfSGc30w0sbnhsUg", DataFormat.Json);

            var response = client.Get(request);

            string str = response.Content;

            var details = JObject.Parse(str);

            object lat = details["results"][0]["geometry"]["location"]["lat"];
            object lng = details["results"][0]["geometry"]["location"]["lng"];

            float latitude = Convert.ToSingle(lat);
            float longitude = Convert.ToSingle(lng);

            searchLocation.Lat = latitude;
            searchLocation.Long = longitude;
            return searchLocation;
        }

        public class Location
        {
            public double Lat;
            public double Long;
        }

        public class BussinessInfo
        {
            public string Name;
            public string Rating;
            public string ImageUrl;
            public string Phone;
            public string Price;
            public string Url;

        }

        private void LocationSearchBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            BigButton.Content = "Search";
        }

        private void CuisineSearchBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            BigButton.Content = "Search";
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (busId >= buss.Count-1)
            {
                busId = 1;
            }
            busId = busId + 1;
            DisplayName.Text = buss[busId].Name;
            DisplayRating.Text = buss[busId].Rating + " Stars";
            DisplayPhone.Text = buss[busId].Phone;
            Uri myUri = new Uri(buss[busId].Url, UriKind.Absolute);
            SiteLink.NavigateUri = myUri;
            DisplayPrice.Text = buss[busId].Price;
            if (buss[busId].ImageUrl != "")
            {
                DisplayImage.Source = new BitmapImage(new Uri(buss[busId].ImageUrl));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (busId <= 1)
            {
                busId = buss.Count-1;
            }
            busId = busId - 1;
            DisplayName.Text = buss[busId].Name;
            DisplayRating.Text = buss[busId].Rating + " Stars";
            DisplayPhone.Text = buss[busId].Phone;
            Uri myUri = new Uri(buss[busId].Url, UriKind.Absolute);
            SiteLink.NavigateUri = myUri;
            DisplayPrice.Text = buss[busId].Price;
            if (buss[busId].ImageUrl != "")
            {
                DisplayImage.Source = new BitmapImage(new Uri(buss[busId].ImageUrl));
            }
        }

        private void LocationSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            DisplayPanel.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            DisplayName.Text = "";
            DisplayRating.Text = "";
            DisplayPhone.Text = "";
            DisplayImage.Source = null;
            SiteLink.Content = "";
            DisplayPrice.Text = "";
        }
    }


}
