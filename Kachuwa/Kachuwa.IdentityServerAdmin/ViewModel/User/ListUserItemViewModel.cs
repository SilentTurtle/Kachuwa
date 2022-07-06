using System;

namespace Kachuwa.IdentityServerAdmin.ViewModel
{
    public class ListUserItemViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
        
        public bool IsLockedOut { get; set; }
        
        public Guid Id { get; set; }
    }
}