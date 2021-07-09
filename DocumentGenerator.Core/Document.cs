namespace DocumentGenerator.Core
{
    public class Document
    {
        public string ContentType { get; }
        public byte[] Content { get; }
        
        public Document(string contentType, byte[] content)
        {
            ContentType = contentType;
            Content = content;
        }
    }
}