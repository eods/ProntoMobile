using System.Collections.Generic;

namespace ProntoMobile.Common.Models
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Document { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<DetalleUserBDResponse> DetalleUserBDs { get; set; }

    }
}
