﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Common;

namespace Wyam.Modules.CodeAnalysis.Tests
{
    internal class TestDocument : IDocument
    {
        private readonly IDictionary<string, object> _metadata = new Dictionary<string, object>();

        public TestDocument(IEnumerable<KeyValuePair<string, object>> metadata)
        {
            foreach (KeyValuePair<string, object> item in metadata)
            {
                _metadata[item.Key] = item.Value;
            }
        }

        public IMetadata<T> MetadataAs<T>()
        {
            throw new NotSupportedException();
        }

        public bool ContainsKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _metadata.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (_metadata.TryGetValue(key, out value))
            {
                value = GetValue(key, value);
                return true;
            }
            return false;
        }

        public object Get(string key, object defaultValue = null)
        {
            object value;
            return TryGetValue(key, out value) ? value : defaultValue;
        }

        public T Get<T>(string key)
        {
            return (T) Get(key);
        }

        public T Get<T>(string key, T defaultValue)
        {
            return (T) Get(key, (object)defaultValue);
        }

        public string String(string key, string defaultValue = null)
        {
            return Get<string>(key, defaultValue);
        }

        public string Link(string key, string defaultValue = null, bool pretty = true)
        {
            string value = Get<string>(key, defaultValue);
            value = value == null ? null : PathHelper.ToRootLink(value);
            return value != null && pretty && (value.EndsWith("/index.html") || value.EndsWith("/index.htm"))
                ? value.Substring(0, value.LastIndexOf("/", StringComparison.Ordinal))
                : value;
        }

        public object this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                object value;
                if (!TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException();
                }
                return value;
            }
        }

        public IEnumerable<string> Keys
        {
            get { return _metadata.Keys; }
        }

        public IEnumerable<object> Values
        {
            get { return _metadata.Select(x => GetValue(x.Key, x.Value)); }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _metadata.Select(GetItem).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _metadata.Count; }
        }

        // This resolves the metadata value by expanding IMetadataValue
        private object GetValue(string key, object value)
        {
            IMetadataValue metadataValue = value as IMetadataValue;
            return metadataValue != null ? metadataValue.Get(key, this) : value;
        }

        // This resolves the metadata value by expanding IMetadataValue
        // To reduce allocations, it returns the input KeyValuePair if value is not IMetadataValue
        private KeyValuePair<string, object> GetItem(KeyValuePair<string, object> item)
        {
            IMetadataValue metadataValue = item.Value as IMetadataValue;
            return metadataValue != null ? new KeyValuePair<string, object>(item.Key, metadataValue.Get(item.Key, this)) : item;
        }

        public string Source
        {
            get { throw new NotSupportedException(); }
        }

        public IMetadata Metadata
        {
            get { throw new NotSupportedException(); }
        }

        public string Content
        {
            get { throw new NotSupportedException(); }
        }

        public Stream GetStream()
        {
            throw new NotSupportedException();
        }

        public IDocument Clone(string source, string content, IEnumerable<KeyValuePair<string, object>> metadata = null)
        {
            throw new NotSupportedException();
        }

        public IDocument Clone(string content, IEnumerable<KeyValuePair<string, object>> metadata = null)
        {
            throw new NotSupportedException();
        }

        public IDocument Clone(string source, Stream stream, IEnumerable<KeyValuePair<string, object>> metadata = null, bool disposeStream = true)
        {
            throw new NotSupportedException();
        }

        public IDocument Clone(Stream stream, IEnumerable<KeyValuePair<string, object>> metadata = null, bool disposeStream = true)
        {
            throw new NotSupportedException();
        }

        public IDocument Clone(IEnumerable<KeyValuePair<string, object>> metadata)
        {
            throw new NotSupportedException();
        }
    }
}
