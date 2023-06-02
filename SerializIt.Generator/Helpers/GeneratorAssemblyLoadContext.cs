using System.Reflection;
using System.Runtime.Loader;

namespace SerializIt.Generator.Helpers;

internal class GeneratorAssemblyLoadContext : AssemblyLoadContext
{

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return Assembly.Load(assemblyName);
    }
}
