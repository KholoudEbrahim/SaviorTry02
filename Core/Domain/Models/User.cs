using Domain.Models.OrderEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Fname { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Lname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;


        [Required]
        public string Password { get; set; } = string.Empty;
        [NotMapped]
        public string ConfirmPassword { get; set; } = string.Empty;


        public string? ResetCode { get; set; }
        public DateTime? ResetCodeExpiry { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Emergency> EmergencyRequests { get; set; } = new List<Emergency>();
        public virtual ICollection<MedicalStaffMember> MedicalStaffMembers { get; set; } = new List<MedicalStaffMember>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
