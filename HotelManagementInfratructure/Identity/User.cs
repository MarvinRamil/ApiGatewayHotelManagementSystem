using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Identity
{
    public class User : IdentityUser
    {
        public Guid? EmployeeId { get; set; }
    }
}
