using GSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{
    public interface IServerSafe
    {
        [VTableSlot(6)]
        int GetMaxClients(); // returns current client limit

    }

    public interface IServer : IConnectionlessPacketHandler
    {

        void dtorIServer();

        int GetNumClients(); // returns current number of clients
        int GetNumProxies(); // returns number of attached HLTV proxies
        int GetNumFakeClients(); // returns number of fake clients/bots
        IntPtr GetClient(int index); // returns interface to client IClient
        int GetClientCount(); // returns number of clients slots (used & unused)
        int GetUDPPort(); // returns current used UDP port
        float GetTime(); // returns game world time
        int GetTick();   // returns game world tick
        float GetTickInterval(); // tick interval in seconds
        string GetName();    // public server name
        string GetMapName(); // current map name (BSP)
        int GetSpawnCount();
        int GetNumClasses();
        int GetClassBits();
        void GetNetStats(ref float avgIn, ref float avgOut); // total net in/out in bytes/sec
        int GetNumPlayers();
        bool GetPlayerInfo(int nClientIndex, IntPtr player_info_t_pinfo);
        bool IsActive();
        bool IsLoading();
        bool IsDedicated();
        bool IsPaused();
        bool IsMultiplayer();
        bool IsPausable();
        bool IsHLTV();
        bool IsReplay();
        string GetPassword();    // returns the password or NULL if none set	
        void SetPaused(bool paused);
        void SetPassword(string password); // set password (NULL to disable)

        //void BroadcastMessage(INetMessage &msg, bool onlyActive = false, bool reliable = false);
        //void BroadcastMessage(INetMessage &msg, IRecipientFilter &filter );
        [VTableOffset(2)]
        void DisconnectClient(IntPtr IClient_client, string reason);
    };
}
