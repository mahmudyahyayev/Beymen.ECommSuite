namespace BuildingBlocks.Core.Persistence.Efcore
{
    public static class EfConstants
    {
        public const string UuidGenerator = "uuid-ossp";
        public const string UuidAlgorithm = "uuid_generate_v4()";
        public const string DateAlgorithm = "now()";
        public const int Zero = 0;
       
        public static class ColumnTypes
        {
            public const string PriceDecimal = "decimal(18,2)";
            public const string TinyText = "varchar(15)";
            public const string CitizenIdText = "varchar(11)";
            public const string ShortText = "varchar(25)";
            public const string NormalText = "varchar(50)";
            public const string MediumText = "varchar(100)";
            public const string LongText = "varchar(255)";
            public const string ExtraLongText = "varchar(500)";
            public const string SoLongText = "varchar(2000)";
            public const string DateTimeUtc = "timestamptz";
        }

        public static class Lenght
        {
            public const int StandartByte = 8;
            public const int CitizenId = 11;
            public const int Tiny = 15;
            public const int ExtraTiny = 40;
            public const int Short = 25;
            public const int StandartShort = 32;
            public const int Normal = 50;
            public const int Medium = 50;
            public const int ExtraMedium = 100;
            public const int Long = 255;
            public const int ExtraLong = 500;
            public const int SoLong = 2000;
            public const int ipAddress = 15;
        }
    }
}
