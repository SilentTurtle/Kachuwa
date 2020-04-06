using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KachuwaApp
{
    public static class EmailExtensions
    {
        public static string ResetPasswordCallbackLink(this IUrlHelper url, string userId, string code,string scheme)
        {
            return String.Empty;
            ;
        }
        public static string EmailConfirmationLink(this IUrlHelper url, string userId, string code, string scheme)
        {
            return String.Empty;
            ;
        }
        
    }
}
