using System;
using System.Collections.Generic;
using System.Linq;
using MikyM.Common.DataAccessLayer_Net5.Repositories;
using MikyM.Common.Utilities_Net5.Extensions;

namespace MikyM.Common.DataAccessLayer_Net5.Helpers
{
    internal static class UoFCache
    {
        static UoFCache()
        {
            CachedRepositoryClassTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t =>
                    t.IsClass && !t.IsAbstract && t.GetInterface(nameof(IBaseRepository)) is not null))
                .ToList();
            CachedRepositoryInterfaceTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t =>
                    t.IsInterface && t.GetInterface(nameof(IBaseRepository)) is not null))
                .ToList();
            CachedRepositoryInterfaceImplTypes ??= CachedRepositoryInterfaceTypes.ToDictionary(intr => intr,
                intr => CachedRepositoryClassTypes.FirstOrDefault(intr.IsDirectAncestor))!;
        }

        internal static IEnumerable<Type> CachedRepositoryClassTypes { get; }
        internal static IEnumerable<Type> CachedRepositoryInterfaceTypes { get; }
        internal static Dictionary<Type, Type> CachedRepositoryInterfaceImplTypes { get; }
    }
}
