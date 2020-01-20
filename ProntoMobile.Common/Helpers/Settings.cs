using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace ProntoMobile.Common.Helpers
{
    public static class Settings
    {
        private const string _user = "User";
        private const string _equipment = "Equipment";
        private const string _token = "Token";
        private const string _isRemembered = "IsRemembered";
        private const string _connectionString = "ConnectionString";

        private static readonly string _stringDefault = string.Empty;
        private static readonly bool _boolDefault = false;

        private static ISettings AppSettings => CrossSettings.Current;

        public static string User
        {
            get => AppSettings.GetValueOrDefault(_user, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_user, value);
        }

        public static string Equipment
        {
            get => AppSettings.GetValueOrDefault(_equipment, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_equipment, value);
        }

        public static string Token
        {
            get => AppSettings.GetValueOrDefault(_token, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_token, value);
        }

        public static bool IsRemembered
        {
            get => AppSettings.GetValueOrDefault(_isRemembered, _boolDefault);
            set => AppSettings.AddOrUpdateValue(_isRemembered, value);
        }

        public static string ConnectionString
        {
            get => AppSettings.GetValueOrDefault(_connectionString, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_connectionString, value);
        }
    }
}
