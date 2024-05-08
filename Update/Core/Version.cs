namespace Update.Core
{
    internal struct Version
    {
        private short _major;
        private short _minor;
        private short _build;
        private short _revision;

        public short Major { get => _major; }
        public short Minor { get => _minor; }
        public short Build { get => _build; }
        public short Revision { get => _revision; }

        public Version(short major, short minor, short build, short revision)
        {
            _major = major;
            _minor = minor;
            _build = build;
            _revision = revision;
        }

        public Version(string version)
        {
            string[] parts = version.Split('.');
            _major = short.Parse(parts[0]);
            _minor = short.Parse(parts[1]);
            _build = short.Parse(parts[2]);
            _revision = (parts.Length > 3 ? short.Parse(parts[3]) : short.Parse("0"));
        }

        public bool IsDifferent(Version version)
        {
            return Major != version.Major || Minor != version.Minor || Build != version.Build || Revision != version.Revision;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }
    }
}
