﻿using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.Pgsql
{
    public abstract class PgsqlMultiValueCondition<T, TProperty> : IMultiValueCondition<T, TProperty>, ISingleAttributeCondition
    {
        public string AttributeName { get; set; } = string.Empty;

        public string ParameterName { get; set; } = string.Empty;

        public TProperty[]? Values { get; set; }

        public NpgsqlParameter[]? GetSqlParameters(string expressionSymbol)
        {
            return Values?.Select((value, index) => new NpgsqlParameter($"{expressionSymbol}{ParameterName}_{index}", value)).ToArray();
        }

        public ISingleAttributeCondition SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression(string expressionSymbol);

        public DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);
    }
}