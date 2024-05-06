using System.ComponentModel;

namespace Mps.Domain.Constants
{
    public enum Role
    {
        [Description("Admin")]
        Admin,
        [Description("Customer")]
        Customer,
        [Description("Supplier")]
        Supplier
    }
}
