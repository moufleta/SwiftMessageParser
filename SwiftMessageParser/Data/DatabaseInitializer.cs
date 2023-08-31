using System.Data.SQLite;

namespace SwiftMessageParser.Data
{
    public class DatabaseInitializer
    {
        private const string DatabasePath = @"C:\\SwiftMessageParser\\SwiftMessageParser\\Data\\SwiftMessageParser.db";
        private readonly IConfiguration configuration;

        public DatabaseInitializer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SetUp()
        {
            if (!File.Exists(DatabasePath))
            {
                SQLiteConnection.CreateFile(DatabasePath);
                string connectionString = configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string createSwiftMessageTableQuery = @"
                                 CREATE TABLE SwiftMessage (
                                     Id INTEGER PRIMARY KEY,
                                     CreatedOn DATETIME NOT NULL,
                                     BasicHeaderId INTEGER NOT NULL,
                                     ApplicationHeaderId INTEGER NOT NULL,
                                     UserHeaderId INTEGER NOT NULL,
                                     FOREIGN KEY (BasicHeaderId) REFERENCES BasicHeader(Id),
                                     FOREIGN KEY (ApplicationHeaderId) REFERENCES ApplicationHeader(Id),
                                     FOREIGN KEY (UserHeaderId) REFERENCES UserHeader(Id)
                                 )";

                    string createBasicHeaderTableQuery = @"
                                 CREATE TABLE BasicHeader (
                                     Id INTEGER PRIMARY KEY,
                                     SwiftMessageId INTEGER NOT NULL,
                                     ApplicationId TEXT NOT NULL,
                                     ServiceId TEXT NOT NULL,
                                     LogicalTerminal TEXT NOT NULL,
                                     SessionNumber TEXT NOT NULL,
                                     SequenceNumber TEXT NOT NULL,
                                     FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                 )";

                    string createApplicationHeaderTableQuery = @"
                                 CREATE TABLE ApplicationHeader (
                                     Id INTEGER PRIMARY KEY,
                                     SwiftMessageId INTEGER NOT NULL,
                                     SwiftMessageDirection TEXT NOT NULL,
                                     SwiftMessageType TEXT NOT NULL,
                                     RecipientBusinessIdentifierCode TEXT NOT NULL,
                                     SwiftMessagePriority TEXT NOT NULL,
                                     DeliveryMonitoring TEXT,
                                     NonDeliveryNotificationPeriod TEXT,
                                     FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                 )";

                    string createUserHeaderTableQuery = @"
                                 CREATE TABLE UserHeader (
                                     Id INTEGER PRIMARY KEY,
                                     SwiftMessageId INTEGER NOT NULL,
                                     PriorityCode TEXT,
                                     SwiftMessageUserReference TEXT NOT NULL,
                                     FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                 )";

                    string createTransactionReferenceNumberTableQuery = @"
                                 CREATE TABLE TransactionReferenceNumber (
                                     Id INTEGER PRIMARY KEY,
                                     SwiftMessageId INTEGER NOT NULL,
                                     TagCode TEXT NOT NULL,
                                     TagValue TEXT NOT NULL,
                                     FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                 )";

                    string createRelatedReferenceTableQuery = @"
                                 CREATE TABLE RelatedReference (
                                     Id INTEGER PRIMARY KEY,
                                     SwiftMessageId INTEGER NOT NULL,
                                     TagCode TEXT NOT NULL,
                                     TagValue TEXT NOT NULL,
                                     FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                 )";

                    string createNarrativeTableQuery = @"
                                 CREATE TABLE Narrative (
                                     Id INTEGER PRIMARY KEY,
                                     SwiftMessageId INTEGER NOT NULL,
                                     TagCode TEXT NOT NULL,
                                     TagValue TEXT NOT NULL,
                                     FOREIGN KEY (SwiftMessageId) REFERENCES SwiftMessage(Id)
                                 )";

                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createSwiftMessageTableQuery;
                        command.ExecuteNonQuery();

                        command.CommandText = createBasicHeaderTableQuery;
                        command.ExecuteNonQuery();

                        command.CommandText = createApplicationHeaderTableQuery;
                        command.ExecuteNonQuery();

                        command.CommandText = createUserHeaderTableQuery;
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
}