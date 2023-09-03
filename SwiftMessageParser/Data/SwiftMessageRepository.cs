using SwiftMessageParser.Business;
using SwiftMessageParser.Data.Contracts;
using SwiftMessageParser.Models;
using System.Data;
using System.Data.SQLite;

namespace SwiftMessageParser.Data
{
    public class SwiftMessageRepository : ISwiftMessageRepository
    {
        private readonly IConfiguration configuration;
        private SQLiteConnection connection;

        public SwiftMessageRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SaveSwiftMessage(SwiftMessage swiftMessage)
        {
            MyLogger.GetInstance().Info("Saving a Swift message...");

            connection = new SQLiteConnection(configuration.GetConnectionString("DefaultConnection"));
            OpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var insertCommand = GenerateInsertCommand(swiftMessage);

                    insertCommand.ExecuteNonQuery();
                    swiftMessage.Id = (int) connection.LastInsertRowId;

                    foreach (var tag in swiftMessage.Tags)
                    {
                        tag.SwiftMessageId = swiftMessage.Id;
                        var tagInsertCommand = tag.GenerateInsertCommand(connection);

                        tagInsertCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MyLogger.GetInstance().Info("Swift message saved successfully.");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Error($"Error while saving Swift message: {ex.Message}");
                    transaction.Rollback();

                    throw;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        private SQLiteCommand GenerateInsertCommand(SwiftMessage swiftMessage)
        {
            var insertSql = @"
                      INSERT INTO SwiftMessage (
                                       CreatedOn, 
                                       ApplicationId, 
                                       ServiceId, 
                                       LogicalTerminal, 
                                       SessionNumber, 
                                       SequenceNumber, 
                                       MessageDirection, 
                                       MessageType, 
                                       RecipientBusinessIdentifierCode, 
                                       MessagePriority, 
                                       DeliveryMonitoring, 
                                       NonDeliveryNotificationPeriod, 
                                       BankPriorityCode, 
                                       MessageUserReference
                      ) 
                      VALUES (
                                       @CreatedOn, 
                                       @ApplicationId, 
                                       @ServiceId, 
                                       @LogicalTerminal, 
                                       @SessionNumber, 
                                       @SequenceNumber, 
                                       @MessageDirection, 
                                       @MessageType, 
                                       @RecipientBusinessIdentifierCode, 
                                       @MessagePriority, 
                                       @DeliveryMonitoring, 
                                       @NonDeliveryNotificationPeriod, 
                                       @BankPriorityCode, 
                                       @MessageUserReference
                      )";

            var insertCommand = new SQLiteCommand(insertSql, connection);

            insertCommand.Parameters.AddWithValue("@CreatedOn", swiftMessage.CreatedOn);
            insertCommand.Parameters.AddWithValue("@ApplicationId", swiftMessage.BasicHeader.ApplicationId);
            insertCommand.Parameters.AddWithValue("@ServiceId", swiftMessage.BasicHeader.ServiceId);
            insertCommand.Parameters.AddWithValue("@LogicalTerminal", swiftMessage.BasicHeader.LogicalTerminal);
            insertCommand.Parameters.AddWithValue("@SessionNumber", swiftMessage.BasicHeader.SessionNumber);
            insertCommand.Parameters.AddWithValue("@SequenceNumber", swiftMessage.BasicHeader.SequenceNumber);
            insertCommand.Parameters.AddWithValue("@MessageDirection", swiftMessage.ApplicationHeader.MessageDirection);
            insertCommand.Parameters.AddWithValue("@MessageType", swiftMessage.ApplicationHeader.MessageType);
            insertCommand.Parameters.AddWithValue("@RecipientBusinessIdentifierCode", swiftMessage.ApplicationHeader.RecipientBusinessIdentifierCode);
            insertCommand.Parameters.AddWithValue("@MessagePriority", swiftMessage.ApplicationHeader.MessagePriority);
            insertCommand.Parameters.AddWithValue("@DeliveryMonitoring", swiftMessage.ApplicationHeader.DeliveryMonitoring);
            insertCommand.Parameters.AddWithValue("@NonDeliveryNotificationPeriod", swiftMessage.ApplicationHeader.NonDeliveryNotificationPeriod);
            insertCommand.Parameters.AddWithValue("@BankPriorityCode", swiftMessage.UserHeader.BankPriorityCode);
            insertCommand.Parameters.AddWithValue("@MessageUserReference", swiftMessage.UserHeader.MessageUserReference);

            MyLogger.GetInstance().Info("Generating INSERT command for SwiftMessage...");

            MyLogger.GetInstance().Debug("Generated SQL: ", insertSql);

            return insertCommand;
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                MyLogger.GetInstance().Info("Closing the database connection...");

                connection.Close();
                MyLogger.GetInstance().Info("Database connection closed.");
            }
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                MyLogger.GetInstance().Info("Opening the database connection...");

                connection.Open();
                MyLogger.GetInstance().Info("Database connection opened.");
            }
        }
    }
}