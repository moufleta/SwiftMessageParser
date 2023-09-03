using System.Data.SQLite;

namespace SwiftMessageParser.Data
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration configuration;
        private readonly string databasePath;

        public DatabaseInitializer(IConfiguration configuration)
        {
            this.configuration = configuration;

            databasePath = configuration.GetConnectionString("DBFileLocation");
        }

        public void SetUp()
        {
            if (File.Exists(databasePath))
            {
                return;
            }

            SQLiteConnection.CreateFile(databasePath);
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createSwiftMessageTableQuery = @"
                                CREATE TABLE SwiftMessage (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    CreatedOn DATETIME NOT NULL,
                                    ApplicationId TEXT NOT NULL,
                                    ServiceId TEXT NOT NULL,
                                    LogicalTerminal TEXT NOT NULL,
                                    SessionNumber TEXT NOT NULL,
                                    SequenceNumber TEXT NOT NULL,
                                    MessageDirection TEXT NOT NULL,
                                    MessageType TEXT NOT NULL,
                                    RecipientBusinessIdentifierCode TEXT NOT NULL,
                                    MessagePriority TEXT NOT NULL,
                                    DeliveryMonitoring TEXT,
                                    NonDeliveryNotificationPeriod TEXT,
                                    BankPriorityCode TEXT,
                                    MessageUserReference TEXT NOT NULL
                                )";

                string createTransactionReferenceNumberTableQuery = @"
                                CREATE TABLE TransactionReferenceNumber (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    SwiftMessageId INTEGER NOT NULL,
                                    TagCode TEXT NOT NULL,
                                    TagValue TEXT NOT NULL,
                                    FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                )";

                string createRelatedReferenceTableQuery = @"
                                CREATE TABLE RelatedReference (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    SwiftMessageId INTEGER NOT NULL,
                                    TagCode TEXT NOT NULL,
                                    TagValue TEXT NOT NULL,
                                    FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                )";

                string createNarrativeTableQuery = @"
                                CREATE TABLE Narrative (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    SwiftMessageId INTEGER NOT NULL,
                                    TagCode TEXT NOT NULL,
                                    TagValue TEXT NOT NULL,
                                    FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                )";

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createSwiftMessageTableQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createTransactionReferenceNumberTableQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createRelatedReferenceTableQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createNarrativeTableQuery;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}