﻿using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleAggregateSelectQuery<T, ResultType> : IAggregateSelectQuery<T, ResultType> where T : IFluentModel, new()
    where ResultType : IFluentModel, new()
{
    public OracleConditionsManager<T> ConditionsManager { get; } = new();
    public OracleFieldsSelectionManager<T> GroupByFieldsManager { get; } = new();
    public AggregateFieldsSelectionManager<T> AggregatesManager { get; } = new();
    public AggregateOrderByManager<T> OrderByManager { get; } = new();
    public OracleHavingConditionsManager<T> HavingConditionsManager { get; } = new();

    public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();
    public string ExpressionSymbol => ":";

    public OracleConnection? SqlConnection { get; set; }

    DbConnection? IAggregateSelectQuery<T, ResultType>.Connection => SqlConnection;

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    IHavingConditionsManager<T> IAggregateSelectQuery<T, ResultType>.HavingConditionsManager => HavingConditionsManager;

    IFieldsSelectionManager<T> IAggregateSelectQuery<T, ResultType>.GroupByFieldsManager => GroupByFieldsManager;

    public OracleAggregateSelectQuery(OracleConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
        GroupByFieldsManager.FieldsList.Clear();
    }

    public IEnumerable<ResultType> Execute(OracleConnection? sqlConnection = null, int pageNumber = 0, int recordsCount = 0)
    {
        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
            connection.Open();

        List<string> headerColumns = new();
        if (AggregatesManager.FieldsList?.Count > 0)
            headerColumns.AddRange(AggregatesManager.FieldsList.GetFinalHeaderStrings());

        if (GroupByFieldsManager.FieldsList?.Count > 0)
            headerColumns.AddRange(GroupByFieldsManager.FieldsList.GetFinalPropertyNames());

        string includeColumns = headerColumns.Count == 0 ? "count(*) records_count" : string.Join(",", headerColumns);

        try
        {
            StringBuilder query = new($"SELECT {(recordsCount > 0 && pageNumber <= 0 ? $"TOP {recordsCount}" : "")} {includeColumns} FROM {TableName}");
            var parameters = new List<OracleParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query.Append(" WHERE " + string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol))));

                foreach (var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

            if (GroupByFieldsManager.FieldsList?.Count > 0)
                query.Append($" Group BY {string.Join(", ", GroupByFieldsManager.FieldsList.GetFinalPropertyNames())}");

            if (HavingConditionsManager.HavingConditions.Count > 0)
            {
                query.Append(" HAVING " + string.Join(" AND ", HavingConditionsManager.HavingConditions.Select(w => w.GetExpression(ExpressionSymbol))));

                foreach (var condition in HavingConditionsManager.HavingConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

            if (OrderByManager.OrderByColumns?.Count > 0)
                query.Append(" ORDER BY " + string.Join(", ", OrderByManager.OrderByColumns.Select(o => o.Expression)));

            if (pageNumber > 0 && recordsCount > 0)
                query.Append($" {(OrderByManager.OrderByColumns?.Count > 0 ? "" : "order by (select null)")} OFFSET {pageNumber * recordsCount} ROWS FETCH NEXT {recordsCount} ROWS ONLY");

            using var command = new OracleCommand(query.ToString(), connection) { BindByName = true };

            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());

            using var dataReader = command.ExecuteReader();

            OracleDtoMapper<ResultType> dtoMapper = new();

            while (dataReader.Read())
            {
                var student = dtoMapper.ToDtoModel(dataReader);
                yield return student;
            }
        }
        finally
        {
            if (shouldCloseConnection)
                connection.Close();
        }
    }

    public OracleAggregateSelectQuery<T, ResultType> Where(Action<OracleConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> OrderBy(Action<AggregateOrderByManager<T>> fn)
    {
        fn(OrderByManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> Aggregate(Action<AggregateFieldsSelectionManager<T>> fn)
    {
        fn(AggregatesManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> GroupBy(Action<OracleFieldsSelectionManager<T>> fn)
    {
        fn(GroupByFieldsManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> Having(Action<OracleHavingConditionsManager<T>> fn)
    {
        fn(HavingConditionsManager);
        return this;
    }

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Aggregate(Action<AggregateFieldsSelectionManager<T>> fn) => Aggregate(fn);

    IEnumerable<ResultType> IAggregateSelectQuery<T, ResultType>.Execute(DbConnection? sqlConnection, int pageNumber, int recordsCount) => Execute((OracleConnection?)sqlConnection, pageNumber, recordsCount);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.GroupBy(Action<IFieldsSelectionManager<T>> fn) => GroupBy(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Having(Action<IHavingConditionsManager<T>> fn) => Having(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.OrderBy(Action<AggregateOrderByManager<T>> fn) => OrderBy(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}