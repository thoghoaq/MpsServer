using System.ComponentModel;

namespace Mps.Domain.Enums
{
    public enum Role
    {
        [Description("SuperAdmin")]
        SuperAdmin,
        [Description("Admin")]
        Admin,
        [Description("Staff")]
        Staff,
        [Description("Customer")]
        Customer,
        [Description("ShopOwner")]
        ShopOwner
    }
}
