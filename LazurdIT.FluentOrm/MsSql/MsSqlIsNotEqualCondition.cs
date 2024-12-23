﻿using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsNotEqualCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty> where T : IFluentModel
    {
        public override bool HasParameters => true;

        public override string GetExpression() => $"({AttributeName} != {ExpressionSymbol}{ParameterName})";
    }
}