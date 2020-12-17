// ReSharper disable VirtualMemberCallInConstructor
namespace CloudPhoto.Data.Models
{
    using System;
    using System.Collections.Generic;

    using CloudPhoto.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();

            this.Votes = new HashSet<Vote>();
            this.Images = new HashSet<Image>();
            this.UserSubscribes = new HashSet<UserSubscribe>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PayPalEmail { get; set; }

        public string Description { get; set; }

        public virtual string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    return this.FirstName + " " + this.LastName;
                }
                else
                {
                    return this.UserName;
                }
            }
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<UserSubscribe> UserSubscribes { get; set; }
    }
}
