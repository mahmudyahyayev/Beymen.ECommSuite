namespace BuildingBlocks.Abstractions.Template
{
    public interface ITemplate
    {
        string Name { get; }
        static string AssemblyQualifiedName { get; }
        static string Version { get; }
    }
}
