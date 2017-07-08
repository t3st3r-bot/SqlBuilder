﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class BuiltSelectCommand
    {
        private List<string> _fromTables;
        private List<string> _selectedColumns;
        private List<SqlJoin> _joins;
        private BuiltSqlSort _sort;
        private BuiltSqlCondition<BuiltSelectCommand> _condition;
        

        internal BuiltSelectCommand()
        {
            _fromTables = new List<string>();
            _selectedColumns = new List<string>();
            _joins = new List<SqlJoin>();
        }
        
        /// <summary>
        /// Adds a table to used in the FROM clause.
        /// </summary>
        /// <param name="table">The table to be added.</param>
        public BuiltSelectCommand AddTable(string table)
        {
            _fromTables.Add(table);
            return this;
        }
        
        /// <summary>
        /// Add columns to be selected.
        /// </summary>
        /// <param name="tableName">The table containing the columns.</param>
        /// <param name="columns">The columns to be selected.</param>
        public BuiltSelectCommand AddColumns(string tableName, params string[] columns)
        {
            foreach (string column in columns)
            {
                string fullyQualified = $"{tableName}.{column}";
                if(!_selectedColumns.Contains(fullyQualified))
                    _selectedColumns.Add(fullyQualified);
            }
            return this;
        }

        /// <summary>
        /// Adds a JOIN to this BuiltSelectCommand.
        /// </summary>
        /// <param name="existingTable">The existing table of the join.</param>
        /// <param name="existingColumn">The column to use from the existing table.</param>
        /// <param name="newTable">The new table of this join.</param>
        /// <param name="newColumn">The column to use from the new table.</param>
        public BuiltSelectCommand Join(string existingTable, string existingColumn, string newTable, string newColumn)
        {
            _joins.Add(new SqlJoin
            {
                FirstTable = existingTable,
                SecondTable = newTable,
                FirstColumn = existingColumn,
                SecondColumn = newColumn
            });
            return this;
        }

        /// <summary>
        /// Creates a WHERE clause for this BuiltSqlCommand. 
        /// Calling this twice on a BuiltSelectCommand will overwrite the first call.
        /// </summary>
        public BuiltSqlCondition<BuiltSelectCommand> Where()
        {
            _condition = new BuiltSqlCondition<BuiltSelectCommand>(this);
            return _condition;
        }

        /// <summary>
        /// Creates a sort for this BuiltSelectCommand.
        /// Calling this twice on a BuiltSelectCommand overwrites the first call.
        /// </summary>
        public BuiltSqlSort OrderBy()
        {
            _sort = new BuiltSqlSort(this);
            return _sort;
        }

        /// <summary>
        /// Generates the SELECT command string.
        /// </summary>
        public string Generate()
        {
            if (_fromTables.Count == 0)
                throw new FormatException("Use .AddTable at least once before generating the Sql_Server string.");

            StringBuilder sb = new StringBuilder("SELECT ");
            sb.Append(_selectedColumns.Count == 0 ? "*" : $"{_selectedColumns.Zip(", ")}");

            sb.Append($" FROM {_fromTables.Zip(", ")}");


            if (_condition != null)
            {
                sb.Append($" {_condition}");
            }
            if (_joins.Count > 0)
            {
                sb.Append($" {_joins.Select(j => j.ToString()).ToList().Zip(" ")}");
            }

            if (_sort != null)
            {
                sb.Append($" {_sort}");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }
    }

    public struct SqlJoin
    {
        public string FirstTable { get; set; }
        public string SecondTable { get; set; }
        public string FirstColumn { get; set; }
        public string SecondColumn { get; set; }

        public override string ToString()
        {
            return $"JOIN {SecondTable} ON {FirstTable}.{FirstColumn}={SecondTable}.{SecondColumn}";
        }
    }
}
