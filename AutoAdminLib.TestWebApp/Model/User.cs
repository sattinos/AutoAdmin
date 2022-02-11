using System;
using AutoAdminLib.Model;

namespace AutoAdminLib.TestWebApp.Model {
    public class User : BaseEntity<uint> {
        public User():base(true)
        {}
        public string FullName { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
    }
}
