using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Common.Helpers
{
    public static class Settings
    {
        private const string _user = "User";
        private const string _equipment = "Equipment";
        private const string _token = "Token";
        private const string _isRemembered = "IsRemembered";
        private const string _connectionString = "ConnectionString";
        private const string _basePronto = "BasePronto";
        private const string _baseMantenimiento = "BaseMantenimiento";

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

        public static TokenResponse Token2
        {
            get => JsonConvert.DeserializeObject<TokenResponse>(AppSettings.GetValueOrDefault(Token, _stringDefault));
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

        public static string BasePronto
        {
            get => AppSettings.GetValueOrDefault(_basePronto, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_basePronto, value);
        }


        public static string BaseMantenimiento
        {
            get => AppSettings.GetValueOrDefault(_baseMantenimiento, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_baseMantenimiento, value);
        }

    }
}
