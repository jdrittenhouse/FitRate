using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitRate.Core.Models;
using FitRate.Core.Models.UserModels;

namespace FitRate.Core.Services
{
    public class FirebaseDataService
    {
        private readonly HttpClient _httpClient;
        // Set your Firestore base URL.
        // Note: For Firestore, the database is identified as (default)
        private readonly string _firebaseDatabaseUrl = "https://firestore.googleapis.com/v1/projects/fitrateco/databases/(default)/documents";

        public FirebaseDataService(HttpClient httpClient, string firebaseDatabaseUrl)
        {
            _httpClient = httpClient;
            _firebaseDatabaseUrl = firebaseDatabaseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Sets the OAuth 2.0 Bearer token on the HttpClient.
        /// Call this after signing in with Firebase Auth to include the token in subsequent requests.
        /// </summary>
        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Saves (or updates) the user profile data to Firestore.
        /// </summary>
        public async Task<bool> SaveUserProfileAsync(UserProfile profile, bool isUpdate)
        {
            if (string.IsNullOrEmpty(profile.UserId))
                throw new ArgumentException("UserId cannot be null or empty.", nameof(profile.UserId));

            // Convert the UserProfile to Firestore JSON format.
            var json = MapUserProfileToFirestoreJson(profile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            string endpoint;

            if (!isUpdate)
            {
                // Creating a new document:
                // POST to the collection endpoint and pass documentId as a query parameter.
                endpoint = $"{_firebaseDatabaseUrl}/userProfiles?documentId={profile.UserId}";
                response = await _httpClient.PostAsync(endpoint, content);
            }
            else
            {
                // Updating an existing document:
                // Use PATCH on the document endpoint.
                endpoint = $"{_firebaseDatabaseUrl}/userProfiles/{profile.UserId}";
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };
                response = await _httpClient.SendAsync(request);
            }
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieves the user profile from Firestore given the userId.
        /// </summary>
        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            // Construct the endpoint for retrieving the document.
            var endpoint = $"{_firebaseDatabaseUrl}/userProfiles/{userId}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return ParseUserProfileFromFirestoreJson(json);
            }
            return null;
        }
        private UserProfile ParseUserProfileFromFirestoreJson(string json)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                if (!root.TryGetProperty("fields", out JsonElement fields))
                    return null;

                var profile = new UserProfile();

                if (fields.TryGetProperty("userId", out JsonElement userIdElem) &&
                    userIdElem.TryGetProperty("stringValue", out JsonElement userIdVal))
                    profile.UserId = userIdVal.GetString();

                if (fields.TryGetProperty("email", out JsonElement emailElem) &&
                    emailElem.TryGetProperty("stringValue", out JsonElement emailVal))
                    profile.Email = emailVal.GetString();

                if (fields.TryGetProperty("firstName", out JsonElement firstNameElem) &&
                    firstNameElem.TryGetProperty("stringValue", out JsonElement firstNameVal))
                    profile.FirstName = firstNameVal.GetString();

                if (fields.TryGetProperty("lastName", out JsonElement lastNameElem) &&
                    lastNameElem.TryGetProperty("stringValue", out JsonElement lastNameVal))
                    profile.LastName = lastNameVal.GetString();

                // Compute fullName if not explicitly provided.
                if (fields.TryGetProperty("fullName", out JsonElement fullNameElem) &&
                    fullNameElem.TryGetProperty("stringValue", out JsonElement fullNameVal))
                {
                    profile.FullName = fullNameVal.GetString();
                }
                else
                {
                    profile.FullName = $"{profile.FirstName} {profile.LastName}".Trim();
                }

                if (fields.TryGetProperty("units", out JsonElement unitsElem) &&
                    unitsElem.TryGetProperty("stringValue", out JsonElement unitsVal))
                    profile.Units = unitsVal.GetString();

                if (fields.TryGetProperty("age", out JsonElement ageElem) &&
                    ageElem.TryGetProperty("integerValue", out JsonElement ageVal))
                    profile.Age = int.Parse(ageVal.GetString());

                if (fields.TryGetProperty("gender", out JsonElement genderElem) &&
                    genderElem.TryGetProperty("stringValue", out JsonElement genderVal))
                    profile.Gender = genderVal.GetString();

                if (fields.TryGetProperty("height", out JsonElement heightElem) &&
                    heightElem.TryGetProperty("doubleValue", out JsonElement heightVal))
                    profile.Height = heightVal.GetDouble();

                if (fields.TryGetProperty("weight", out JsonElement weightElem) &&
                    weightElem.TryGetProperty("doubleValue", out JsonElement weightVal))
                    profile.Weight = weightVal.GetDouble();

                if (fields.TryGetProperty("createdOn", out JsonElement createdOnElem) &&
                    createdOnElem.TryGetProperty("timestampValue", out JsonElement createdOnVal))
                    profile.CreatedOn = DateTime.Parse(createdOnVal.GetString());

                if (fields.TryGetProperty("lastUpdated", out JsonElement lastUpdatedElem) &&
                    lastUpdatedElem.TryGetProperty("timestampValue", out JsonElement lastUpdatedVal))
                    profile.LastUpdated = DateTime.Parse(lastUpdatedVal.GetString());

                // For simplicity, goals parsing is omitted here. You can add similar logic if needed.

                return profile;
            }
        }

        /// <summary>
        /// Maps a UserProfile object to the Firestore JSON format required for the REST API.
        /// Fields with empty strings will be mapped using "nullValue".
        /// The fullName field is computed as FirstName + " " + LastName if not provided.
        /// </summary>
        private string MapUserProfileToFirestoreJson(UserProfile profile)
        {
            // Create a dictionary to hold the "fields" object.
            var fields = new Dictionary<string, object>();

            // Helper function: maps a string field.
            Dictionary<string, object> MapStringField(string value)
            {
                return !string.IsNullOrWhiteSpace(value)
                    ? new Dictionary<string, object> { { "stringValue", value } }
                    : new Dictionary<string, object> { { "nullValue", null } };
            }

            // userId (should always be non-null)
            fields["userId"] = new Dictionary<string, object> { { "stringValue", profile.UserId } };

            // email
            fields["email"] = MapStringField(profile.Email);

            // firstName
            fields["firstName"] = MapStringField(profile.FirstName);

            // lastName
            fields["lastName"] = MapStringField(profile.LastName);

            // fullName: if profile.FullName is provided, use it; otherwise compute it.
            string computedFullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(profile.FirstName) || !string.IsNullOrWhiteSpace(profile.LastName))
            {
                computedFullName = $"{profile.FirstName?.Trim()} {profile.LastName?.Trim()}".Trim();
            }
            string fullNameValue = !string.IsNullOrWhiteSpace(profile.FullName) ? profile.FullName : computedFullName;
            fields["fullName"] = MapStringField(fullNameValue);

            // units
            fields["units"] = MapStringField(profile.Units);

            // age (always include; assuming age is a valid integer)
            fields["age"] = new Dictionary<string, object> { { "integerValue", profile.Age.ToString() } };

            // gender
            fields["gender"] = MapStringField(profile.Gender);

            // height and weight as numeric values
            fields["height"] = new Dictionary<string, object> { { "doubleValue", profile.Height } };
            fields["weight"] = new Dictionary<string, object> { { "doubleValue", profile.Weight } };

            // createdOn and lastUpdated as timestamps in ISO 8601 format
            fields["createdOn"] = new Dictionary<string, object> { { "timestampValue", profile.CreatedOn.ToString("o") } };
            fields["lastUpdated"] = new Dictionary<string, object> { { "timestampValue", profile.LastUpdated.ToString("o") } };

            // Goals: map to an arrayValue; if none exist, use nullValue.
            if (profile.Goals != null && profile.Goals.Count > 0)
            {
                var goalValues = new List<object>();
                foreach (var goal in profile.Goals)
                {
                    var goalMap = new Dictionary<string, object>
                    {
                        { "mapValue", new Dictionary<string, object>
                            {
                                { "fields", new Dictionary<string, object>
                                    {
                                        { "type", new Dictionary<string, object> { { "stringValue", goal.Type.ToString() } } },
                                        { "priority", new Dictionary<string, object> { { "integerValue", goal.Priority.ToString() } } }
                                    }
                                }
                            }
                        }
                    };
                    goalValues.Add(goalMap);
                }
                fields["goals"] = new Dictionary<string, object>
                {
                    { "arrayValue", new Dictionary<string, object> { { "values", goalValues } } }
                };
            }
            else
            {
                fields["goals"] = new Dictionary<string, object> { { "nullValue", null } };
            }

            // Wrap the fields dictionary in a document object.
            var document = new Dictionary<string, object>
            {
                { "fields", fields }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Serialize(document, options);
        }
    }
}
