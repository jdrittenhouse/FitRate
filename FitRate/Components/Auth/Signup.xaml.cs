using System;
using Microsoft.Maui.Controls;
using FitRate.Core.Services;
using Microsoft.Extensions.DependencyInjection;


namespace FitRate.Mobile.Components.Auth
{
    public partial class Signup : ContentPage
    {
        // We'll resolve FirebaseAuthService from the DI container.
        private readonly FirebaseAuthService _authService;

        public Signup()
        {
            InitializeComponent();
            // Get the service from the application's service provider.
            // In MAUI, you can access the DI container via Application.Current.Services.
            _authService = App.Services.GetService<FirebaseAuthService>();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            // Basic input validation
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                StatusLabel.Text = "Please fill in all fields.";
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                StatusLabel.Text = "Passwords do not match.";
                return;
            }

            try
            {
                // Call the signup method in your FirebaseAuthService.
                var authResponse = await _authService.SignUpWithEmailAsync(EmailEntry.Text, PasswordEntry.Text);
                StatusLabel.TextColor = Colors.Green;
                StatusLabel.Text = "Sign Up Successful!";

                // Optionally, navigate to the Login page or main page
                // await Shell.Current.GoToAsync("//Login");
            }
            catch (Exception ex)
            {
                StatusLabel.TextColor = Colors.Red;
                StatusLabel.Text = $"Error: {ex.Message}";
            }
        }
    }
}
