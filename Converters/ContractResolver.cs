using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;

namespace MsbtUtility.Converters;

internal sealed class ContractResolver : DefaultContractResolver
{
    public static readonly ContractResolver Instance = new();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        //handle empty collections
        if (member.MemberType is MemberTypes.Property && member.GetCustomAttribute<IgnoreEmptyCollectionAttribute>() is not null && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        {
            property.ShouldSerialize = instance =>
            {
                var enumerator = ((IEnumerable?)instance.GetType().GetProperty(member.Name)?.GetValue(instance, null))?.GetEnumerator();
                if (enumerator is null) return property.NullValueHandling is NullValueHandling.Include;

                var canIterate = enumerator.MoveNext();
                if (enumerator is IDisposable disposable) disposable.Dispose();
                return canIterate;
            };
        }

        return property;
    }
}
