namespace Ach.Fulfillment.Data
{
    using System.Diagnostics.CodeAnalysis;

    public static class MetadataInfo
    {
        #region Constants

        public const string DefaultUserRole = "DefaultUser";

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = "normal")]
        public const int StringLong = 4000;

        public const int StringNormal = 255;

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "short", Justification = "normal")]
        public const int StringShort = 20;

        public const int StringTiny = 3;

        #endregion
    }
}