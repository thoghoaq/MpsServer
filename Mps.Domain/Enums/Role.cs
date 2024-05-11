using System.ComponentModel;

namespace Mps.Domain.Constants
{
    public enum Role
    {
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
