using System.Collections.Generic;

namespace Sonar
{
    /// <summary>
    /// A builder for building a Data object.
    /// </summary>
    public class DataBuilder
    {
        private readonly Dictionary<string, object> _data;

        public DataBuilder()
        {
            _data = new Dictionary<string, object>();
        }

        public void SetData(string key, object data)
        {
            _data[key] = data;
        }

        public Data Build()
        {
            return new Data(_data);
        }
    }

    /// <summary>
    /// Global data set by initialization that can be used by modules.
    /// </summary>
    public class Data
    {
        private readonly Dictionary<string, object> _data;

        public Data(Dictionary<string, object> data)
        {
            _data = data;
        }

        public T GetData<T>(string key)
        {
            return (T) _data[key];
        }
    }
}