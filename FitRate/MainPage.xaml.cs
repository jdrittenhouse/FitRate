using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel; // For Launcher

namespace FitRate.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Navigate to the Login page (ensure Shell route "Login" is registered)
        private async void OnSignInClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Login");
        }

        // Navigate to the Sign Up page (ensure Shell route "Signup" is registered)
        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Signup");
        }

        // Open an external website using the Launcher API
        private async void OnVisitWebsiteClicked(object sender, EventArgs e)
        {
            var uri = new Uri("https://www.example.com");
            await Launcher.OpenAsync(uri);
        }
    }
}
