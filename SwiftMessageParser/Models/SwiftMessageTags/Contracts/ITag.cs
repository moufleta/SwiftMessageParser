namespace SwiftMessageParser.Models.SwiftMessageTags.Contracts
{
    public interface ITag
    {
        int Id { get; set; }

        string TagCode { get; }

        string TagValue { get; set; }

        bool IsValid();
    }
}