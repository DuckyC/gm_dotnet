namespace GSharp.LuaLibraries
{

    public interface IEntity
    {
        void Ignite(int length, int radius);
        void Extinguish();
    }

    public interface IPlayer : IEntity
    {
        string SteamID();
        void Say(string text, bool teamOnly = false);
    }
}
