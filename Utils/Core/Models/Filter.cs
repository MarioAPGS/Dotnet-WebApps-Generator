using Newtonsoft.Json;

namespace Core.Models
{
    public class Filter
    {
        public static readonly string ID = "Id";
        public string Key { get; set; }
        public object Value { get; set; }
        public FilterType FilterType { get; set; }

        [JsonConstructor]
        public Filter(string key, object value, FilterType filterType) : this(key, value)
        {
            FilterType = filterType;
        }

        public Filter(string key, object value)
        {
            Key = key;
            Value = value;
            FilterType = FilterType.Same;
        }
    }

    public enum FilterType
    {
        Same,
        Contains
    }


}
