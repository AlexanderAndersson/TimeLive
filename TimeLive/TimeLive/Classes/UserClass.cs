using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.Security.Principal;

namespace TimeLive.Classes
{
    public class UserClass
    {
        public class User
        {
            public string Domain { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
        }
        public static User GetUserByIdentity(WindowsIdentity identity)
        {
            var newUser = new User();
            var userStrings = identity.Name.Split('\\');

            userStrings[0] = "OPTIVASYS";
            userStrings[1] = "joha";
            newUser.Domain = userStrings[0];
            newUser.Username = userStrings[1];
            newUser.FullName = GetFullName(newUser.Domain, newUser.Username);
            return newUser;
        }
        private static string GetFullName(string domain, string username)
        {
            try
            {
                var dEntry = new DirectoryEntry("LDAP://DC=optivasys,DC=local");
                var searcher = new DirectorySearcher(dEntry);
                searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                var result = searcher.FindOne();
                return result.GetDirectoryEntry().Properties["displayName"].Value.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}