using System;
using Microsoft.Maui.Controls;
using FitRate.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FitRate.Mobile.Components.Auth
{
    public partial class Login : ContentPage
    {
        private readonly FirebaseAuthService _authService;
        private readonly FirebaseDataService _dataService;

        public Login()
        {
            InitializeComponent();
            // Cast Application.Current to your custom App type to access the Services property.
            _authService = App.Services.GetService<FirebaseAuthService>();
            _dataService = App.Services.GetService<FirebaseDataService>();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                StatusLabel.Text = "Please enter both email and password.";
                return;
            }

            try
            {
                // Sign in with Firebase Auth
                var authResponse = await _authService.SignInWithEmailAsync(EmailEntry.Text, PasswordEntry.Text);
                StatusLabel.TextColor = Colors.Green;
                StatusLabel.Text = "Login Successful!";

                // Retrieve the user profile using the user ID from Firebase Auth
                // Assume authResponse contains a property LocalId or UserId
                _dataService.SetAuthToken(authResponse.IdToken);
                var profile = await _dataService.GetUserProfileAsync(authResponse.LocalId);

                if (profile == null)
                {
                    // No profile exists; redirect to profile completion page
                    await Shell.Current.GoToAsync($"UserProfile?userId={authResponse.LocalId}&email={authResponse.Email}&isUpdate=false");

                }
                else
                {
                    // Optionally, you can validate if the profile is complete by checking required fields.
                    if (string.IsNullOrWhiteSpace(profile.FullName) ||
                        profile.Age <= 0 ||
                        profile.Goals == null || profile.Goals.Count == 0)
                    {
                        // Incomplete profile
                        await Shell.Current.GoToAsync($"UserProfile?userId={authResponse.LocalId}&email={authResponse.Email}&isUpdate=true");
                    }
                    else
                    {
                        // Profile exists and is considered complete; navigate to main dashboard.
                        await Shell.Current.GoToAsync("MainDashboard");
                    }
                }
            }
            catch (Exception ex)
            {
                StatusLabel.TextColor = Colors.Red;
                StatusLabel.Text = $"Error: {ex.Message}";
            }
        }

    }
}
