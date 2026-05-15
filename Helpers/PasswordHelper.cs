using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TECNOKARNY.Models;

namespace Unidad_IV.Helpers
{
    public class PasswordHelper
    {
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<Usuarios> _hasher =
       new PasswordHasher<Usuarios>();
 
        public string HashPassword(Usuarios user, string password)
        {
            return _hasher.HashPassword(user, password);
        }
 
        public bool VerifyPassword(Usuarios user, string storedPwd, string password)
        {
            try
            {
                var result = _hasher.VerifyHashedPassword(user, storedPwd, password);
 
                return result == PasswordVerificationResult.Success
                    || result == PasswordVerificationResult.SuccessRehashNeeded;
            }
            catch (FormatException)
            {
                return storedPwd == password;
            }
        }
    }
}