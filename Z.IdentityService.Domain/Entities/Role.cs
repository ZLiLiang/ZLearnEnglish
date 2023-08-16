using Microsoft.AspNetCore.Identity;

namespace Z.IdentityService.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
