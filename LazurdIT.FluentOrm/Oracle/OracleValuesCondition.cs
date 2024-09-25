﻿using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public abstract class OracleValuesCondition<T, TProperty> : ValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
{
    public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

    public virtual OracleParameter[]? GetSqlParameters(string _)
    {
        return new[] { new OracleParameter($"{ParameterName}", Value) };
    }
}