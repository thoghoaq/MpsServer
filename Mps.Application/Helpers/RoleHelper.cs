using Mps.Domain.Enums;
using Mps.Domain.Extensions;

namespace Mps.Application.Helpers
{
    public static class RoleHelper
    {
        public static bool InRoles(this string role)
        {
            foreach (Role roleEnum in Enum.GetValues(typeof(Role)))
            {
                if (role == roleEnum.GetDescription())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
