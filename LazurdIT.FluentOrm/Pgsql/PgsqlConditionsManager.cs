﻿using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlConditionsManager<T> : IFluentConditionsManager<T> where T : IFluentModel, new()
    {
        public List<IFluentCondition> WhereConditions { get; } = new();

        public PgsqlConditionsManager()
        {
        }

        public virtual PgsqlConditionsManager<T> Clone(PgsqlConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<IFluentCondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual PgsqlConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.Eq(property, value));
            return this;
        }

        public virtual PgsqlConditionsManager<T> NotEq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.NotEq(property, value));
            return this;
        }

        public virtual PgsqlConditionsManager<T> Custom(IFluentSingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual PgsqlConditionsManager<T> Or(Action<PgsqlConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.Or };
            PgsqlConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual PgsqlConditionsManager<T> And(Action<PgsqlConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.And };
            PgsqlConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual PgsqlConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(PgsqlOp.Raw(property, whereCondition));
            return this;
        }

        public virtual PgsqlConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new PgsqlRawCondition() { RawCondition = whereCondition });
            return this;
        }

        public PgsqlConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2)
        {
            WhereConditions.Add(PgsqlOp.Between(property, value1, value2));
            return this;
        }

        public PgsqlConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.Gt(property, value));
            return this;
        }

        public PgsqlConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.Gte(property, value));
            return this;
        }

        public PgsqlConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
        {
            WhereConditions.Add(PgsqlOp.In(property, values));
            return this;
        }

        public PgsqlConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property)
        {
            WhereConditions.Add(PgsqlOp.IsNotNull(property));
            return this;
        }

        public PgsqlConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property)
        {
            WhereConditions.Add(PgsqlOp.IsNull(property));
            return this;
        }

        public PgsqlConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.Like(property, value));
            return this;
        }

        public PgsqlConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.Lt(property, value));
            return this;
        }

        public PgsqlConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.Lte(property, value));
            return this;
        }

        public PgsqlConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2)
        {
            WhereConditions.Add(PgsqlOp.NotBetween(property, values1, values2));
            return this;
        }

        public PgsqlConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
        {
            WhereConditions.Add(PgsqlOp.NotIn(property, values));
            return this;
        }

        public PgsqlConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(PgsqlOp.NotLike(property, value));
            return this;
        }

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Clone(IFluentConditionsManager<T> sourceIConditionsManager) => Clone((PgsqlConditionsManager<T>)sourceIConditionsManager);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Custom(IFluentSingleAttributeCondition condition) => Custom(condition);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.And(Action<IFluentConditionsManager<T>> conditionGroup) => And(conditionGroup);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Or(Action<IFluentConditionsManager<T>> conditionGroup) => Or(conditionGroup);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Eq(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotEq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => NotEq(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Gt(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Gte(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) => In(property, values);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.IsNotNull<TProperty>(Expression<Func<T, TProperty>> property) => IsNotNull(property);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.IsNull<TProperty>(Expression<Func<T, TProperty>> property) => IsNull(property);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Like(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Lt(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Lte(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) => NotBetween(property, values1, values2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) => NotIn(property, values);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => NotLike(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Raw(string whereCondition) => Raw(whereCondition);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition) => Raw(property, whereCondition);
    }
}