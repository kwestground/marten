using System.Linq;
using System.Linq.Expressions;
using Marten.Linq.Fields;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.Parsing.Methods;

public class NgramSearch: IMethodCallParser
{
    public bool Matches(MethodCallExpression expression)
    {
        return expression.Method.Name == nameof(LinqExtensions.NgramSearch)
               && expression.Method.DeclaringType == typeof(LinqExtensions);
    }

    public ISqlFragment Parse(IFieldMapping mapping, IReadOnlyStoreOptions options, MethodCallExpression expression)
    {
        var members = FindMembers.Determine(expression);

        var locator = mapping.FieldFor(members).RawLocator;
        var values = expression.Arguments.Last().Value();

        return new WhereFragment($"{options.DatabaseSchemaName}.mt_grams_vector({locator}) @@ {options.DatabaseSchemaName}.mt_grams_query(?)", values);
    }
}
