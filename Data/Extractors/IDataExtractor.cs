namespace AAG.Global.Data.Extractors
{
    public interface IDataExtractor
    {
        public object Extract(string value);
    }
}