using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.LdapExtension;
using Shop.Common.Models;

namespace Shop.Common.Extensions
{
    public static class LdapIdentityServerExtension
    {
        public static ExtensionConfig MapToExtensionConfig(this IdentityServerLdap ldapConfig)
        {
            return new ExtensionConfig 
            { 
                Connections = new List<LdapConfig> 
                {
                    new LdapConfig
                    {
                        FriendlyName = ldapConfig.FriendlyName,
                        Url = ldapConfig.Url,
                        Port = ldapConfig.Port,
                        Ssl = ldapConfig.Ssl,
                        BindDn = ldapConfig.BindDn,
                        BindCredentials = ldapConfig.BindCredentials,
                        SearchBase = ldapConfig.SearchBase,
                        SearchFilter = ldapConfig.SearchFilter,
                        Redis = string.Empty,
                        PreFilterRegex = ldapConfig.PreFilterRegex
                    }
                } 
            };
        }
    }
}