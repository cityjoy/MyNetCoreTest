using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;

namespace User.Identity
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("gateway_api","gateway_api")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    RefreshTokenExpiration= TokenExpiration.Sliding,
                    AllowOfflineAccess=true,
                    RequireClientSecret=false,
                    AllowedGrantTypes = new List<string>{"sms_auth_code" },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes =new List<string> {
                        "gateway_api",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId }
                    
                } 
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser{
                    SubjectId = "1",
                    Username = "test",
                    Password = "1234"
                }
            };
        }
    }
}