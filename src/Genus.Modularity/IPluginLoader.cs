namespace Genus.Modularity
{
    public interface IPluginLoader
    {
        PluginDescriptor LoadPlugin(CandidateDescriptor candidate);
    }
}