using System.ComponentModel;

namespace Mps.Domain.Enums
{
    public enum ImageType
    {
        [Description("UserAvatars")]
        UserAvatars = 1,
        [Description("ProductImages")]
        ProductImages = 2,
        [Description("General")]
        General = 3,
    }
}
