using System;
using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class Store : BaseEntity
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        //[JsonProperty("city")]
        //public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        //[JsonProperty("country_name")]
        //public string CountryName { get; set; }

        [JsonProperty("created_on")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        //[JsonProperty("description")]
        //public string Description { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        //[JsonProperty("force_ssl")]
        //public bool? ForceSSL { get; set; }

        //[JsonProperty("google_apps_domain")]
        //public string GoogleAppsDomain { get; set; }

        //[JsonProperty("google_apps_login_enabled")]
        //public string GoogleAppsLoginEnabled { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("money_format")]
        public string MoneyFormat { get; set; }

        [JsonProperty("money_with_currency_format")]
        public string MoneyWithCurrencyFormat { get; set; }

        [JsonProperty("bizweb_domain")]
        public string BizwebDomain { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("plan_name")]
        public string PlanName { get; set; }

        [JsonProperty("plan_display_name")]
        public string PlanDisplayName { get; set; }

        //[JsonProperty("password_enabled")]
        //public bool? PasswordEnabled { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        //[JsonProperty("primary_locale")]
        //public string PrimaryLocale { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }

        //[JsonProperty("ships_to_countries")]
        //public string ShipsToCountries { get; set; }

        [JsonProperty("store_owner")]
        public string StoreOwner { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        //[JsonProperty("tax_shipping")]
        //public bool? TaxShipping { get; set; }

        //[JsonProperty("taxes_included")]
        //public bool? TaxesIncluded { get; set; }

        //[JsonProperty("county_taxes")]
        //public bool? CountyTaxes { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        //[JsonProperty("iana_timezone")]
        //public string IANATimezone { get; set; }

        //[JsonProperty("zip")]
        //public string Zip { get; set; }

        //[JsonProperty("has_storefront")]
        //public bool HasStorefront { get; set; }

        //[JsonProperty("setup_required")]
        //public bool SetupRequired { get; set; }

        [JsonProperty("used_volumn")]
        public long UsedVolumn { get; set; }

        [JsonProperty("max_volumn")]
        public long MaxVolumn { get; set; }
    }
}