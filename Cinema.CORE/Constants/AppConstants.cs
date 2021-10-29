namespace Cinema.COMMON.Constants
{
    public static class AppConstants
    {
        public static class Roles
        {
            public const string SUPER_ADMIN = "SuperAdmin";
            public const string ADMIN = "Admin";
            public const string MANAGER = "Manager";
            public const string USER = "User";
            public static class Groups
            {
                public static readonly string[] ROLES = { SUPER_ADMIN, ADMIN, MANAGER, USER };
                public const string ADMINSGROUP = SUPER_ADMIN + "," + ADMIN;
            }
        }
    }
}
