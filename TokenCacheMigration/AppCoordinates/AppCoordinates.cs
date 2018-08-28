using System;

namespace AppCoordinates
{
    public class AppCoordinates
    {
        public string ClientId { get; set; }
        public string Tenant { get; set; }
        public string Authority { get { return $"https://login.microsoftonline.com/{Tenant}/";  } }
        public Uri RedirectUri { get; set; }
    }

    public static class PreRegisteredApps
    {
        public static AppCoordinates GetV1App(bool useInMsal)
        {
            return new AppCoordinates()
            {
                ClientId = "f0e0429e-060c-42d3-9375-913eb7c7a62d",
                Tenant = useInMsal ? "organizations" : "common", // Multi-tenant: you can try it out in your AAD organization
                RedirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob")
            };
        }

        // Resources
        public static string MsGraph = "https://graph.microsoft.com";
    }
}
