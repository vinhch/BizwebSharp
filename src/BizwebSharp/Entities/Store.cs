using Newtonsoft.Json;

namespace BizwebSharp
{
    public class Store : BaseEntityWithTimeline
    {
        /// <summary>
        /// The shop's street address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// A human-friendly unique string for this resource automatically generated from its title.
        /// It is used in resource's URL.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; set; }

        //[JsonProperty("city")]
        //public string City { get; set; }

        /// <summary>
        /// The shop's country (by default equal to the two-letter country code).
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// The two-letter country code corresponding to the shop's country.
        /// </summary>
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        //[JsonProperty("country_name")]
        //public string CountryName { get; set; }

        /// <summary>
        /// The customer's email.
        /// </summary>
        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// The three-letter code for the currency that the shop accepts.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        //[JsonProperty("description")]
        //public string Description { get; set; }

        /// <summary>
        /// The shop's domain.
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// The contact email address for the shop.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        //[JsonProperty("force_ssl")]
        //public bool? ForceSSL { get; set; }

        //[JsonProperty("google_apps_domain")]
        //public string GoogleAppsDomain { get; set; }

        //[JsonProperty("google_apps_login_enabled")]
        //public string GoogleAppsLoginEnabled { get; set; }

        /// <summary>
        /// Geographic coordinate specifying the north/south location of a shop.
        /// </summary>
        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// Geographic coordinate specifying the east/west location of a shop.
        /// </summary>
        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// A string representing the way currency is formatted when the currency isn't specified.
        /// </summary>
        [JsonProperty("money_format")]
        public string MoneyFormat { get; set; }

        /// <summary>
        /// A string representing the way currency is formatted when the currency is specified.
        /// </summary>
        [JsonProperty("money_with_currency_format")]
        public string MoneyWithCurrencyFormat { get; set; }

        /// <summary>
        /// The shop's 'bizwebvietnam.net' domain.
        /// </summary>
        [JsonProperty("bizweb_domain")]
        public string BizwebDomain { get; set; }

        /// <summary>
        /// The name of the shop.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name of the Bizweb plan the shop is on.
        /// </summary>
        [JsonProperty("plan_name")]
        public string PlanName { get; set; }

        /// <summary>
        /// The display name of the Bizweb plan the shop is on.
        /// </summary>
        [JsonProperty("plan_display_name")]
        public string PlanDisplayName { get; set; }

        //[JsonProperty("password_enabled")]
        //public bool? PasswordEnabled { get; set; }

        /// <summary>
        /// The contact phone number for the shop.
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        //[JsonProperty("primary_locale")]
        //public string PrimaryLocale { get; set; }

        /// <summary>
        /// The shop's normalized province or state name.
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// The two-letter code for the shop's province or state.
        /// </summary>
        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }

        //[JsonProperty("ships_to_countries")]
        //public string ShipsToCountries { get; set; }

        /// <summary>
        /// The username of the shop owner.
        /// </summary>
        [JsonProperty("store_owner")]
        public string StoreOwner { get; set; }

        /// <summary>
        /// Unkown. Bizweb documentation does not currently indicate what this property actually is.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        //[JsonProperty("tax_shipping")]
        //public bool? TaxShipping { get; set; }

        //[JsonProperty("taxes_included")]
        //public bool? TaxesIncluded { get; set; }

        //[JsonProperty("county_taxes")]
        //public bool? CountyTaxes { get; set; }

        /// <summary>
        /// The name of the timezone the shop is in.
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        //[JsonProperty("iana_timezone")]
        //public string IANATimezone { get; set; }

        //[JsonProperty("zip")]
        //public string Zip { get; set; }

        //[JsonProperty("has_storefront")]
        //public bool? HasStorefront { get; set; }

        //[JsonProperty("setup_required")]
        //public bool? SetupRequired { get; set; }

        [JsonProperty("used_volumn")]
        public long? UsedVolumn { get; set; }

        [JsonProperty("max_volumn")]
        public long? MaxVolumn { get; set; }
    }
}