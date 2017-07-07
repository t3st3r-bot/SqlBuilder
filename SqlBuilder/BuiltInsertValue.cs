﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public class BuiltInsertValue
    {
        private List<string> _columns;
        private Dictionary<string, string> _values;
        private BuiltInsertCommand _parent;

        internal BuiltInsertValue(BuiltInsertCommand parent, List<string> columns)
        {
            _parent = parent;
            _columns = new List<string>(columns);
            _values = new Dictionary<string, string>();
        }

        public BuiltInsertValue AddValueFor(string column, string value, char putAroundValue = '\0')
        {
            if (!_columns.Contains(column))
                throw new Exception($"This BuiltInsertValue does not contain a column named \"{column}\"");

            if (putAroundValue != '\0')
                value = $"{putAroundValue}{value}{putAroundValue}";

            if (_values.ContainsKey(column))
                _values[column] = value;
            else
                _values.Add(column, value);

            return this;
        }

        public BuiltInsertCommand Finish()
        {
            return _parent;
        }

        public string Generate()
        {
            if (!_columns.All(c => _values.ContainsKey(c)))
                throw new Exception("Please use AddValueFor to set a value for each column before calling ToString.");

            StringBuilder sb = new StringBuilder("(");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_values[_columns[i]]);
                if (i < _columns.Count - 1)
                    sb.Append(", ");
            }

            sb.Append(")");
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }

    }
}