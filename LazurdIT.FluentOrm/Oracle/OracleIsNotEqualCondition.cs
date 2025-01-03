﻿using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleIsNotEqualCondition<T, TProperty> : OracleValuesCondition<T, TProperty> where T : IFluentModel

    {
        public override bool HasParameters => true;

        public override string GetExpression() => $"({AttributeName} != {ExpressionSymbol}{ParameterName})";
    }
}