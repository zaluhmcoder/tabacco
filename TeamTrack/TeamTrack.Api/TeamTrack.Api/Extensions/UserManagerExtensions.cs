using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamTrack.Core.Entities;

namespace TeamTrack.Api.Extensions
{
    public static class UserManagerExtensions
    {
        public static Task<User> FindAsync(this UserManager<User> userManager,string userName, string password)
        {

            return null;
        }
    }
}
