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
                }
                catch (Exception)
                {
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

            return insertCommand;
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
    }
}