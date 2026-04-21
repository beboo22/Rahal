using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Auth
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
    public class UserDto
    {
        public Token Token { get; set; }
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public int Age { get; set; }

        #region Blocked
        public bool? IsBlocked { get; set; } = false;
        public DateTime? BlockedEndDate { get; set; }
        public DateTime? BlockedStartDate { get; set; }
        #endregion

        public bool Isverified { get; set; } = false;


        public string Email { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public decimal? FinancialBalance { get; set; }
    }
}
