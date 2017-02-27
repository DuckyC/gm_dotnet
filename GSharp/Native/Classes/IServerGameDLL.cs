using System;
using System.Runtime.InteropServices;
using GSharp.Attributes;
namespace GSharp.Native.Classes
{
    public class IGet { }
    public class CSaveRestoreData { }
    public class CGlobalVars { }
    public class EQueryCvarValueStatus { }
    public partial struct edict_t { }
    public partial struct datamap_t { }
    public partial struct QueryCvarCookie_t { }
    public partial struct typedescription_t { }

    [ModuleName("server")]
    [InterfaceVersion("ServerGameDLL009")]
    public interface IServerGameDLL
    {
        [VTableSlot(0)]
        void PreInit(CreateInterfaceDelegate param0, out IGet param1);

        [VTableSlot(1)]
        bool DLLInit(CreateInterfaceDelegate engineFactory, CreateInterfaceDelegate physicsFactory, CreateInterfaceDelegate fileSystemFactory, out CGlobalVars pGlobals);

        [VTableSlot(2)]
        bool ReplayInit(CreateInterfaceDelegate fnReplayFactory);

        [VTableSlot(3)]
        bool GameInit();

        [VTableSlot(4)]
        bool LevelInit([MarshalAs(UnmanagedType.LPStr)] string pMapName, [MarshalAs(UnmanagedType.LPStr)] string pMapEntities, [MarshalAs(UnmanagedType.LPStr)] string pOldLevel, [MarshalAs(UnmanagedType.LPStr)] string pLandmarkName, bool loadGame, bool background);

        [VTableSlot(5)]
        void ServerActivate(out edict_t pEdictList, int edictCount, int clientMax);

        [VTableSlot(6)]
        void GameFrame(bool simulating);

        [VTableSlot(7)]
        void PreClientUpdate(bool simulating);

        [VTableSlot(8)]
        void LevelShutdown();

        [VTableSlot(9)]
        void GameShutdown();

        [VTableSlot(10)]
        void DLLShutdown();

        [VTableSlot(11)]
        float GetTickInterval();

        [VTableSlot(12)]
        IntPtr GetAllServerClasses();

        [VTableSlot(13)]
        string GetGameDescription();

        [VTableSlot(14)]
        void CreateNetworkStringTables();

        [VTableSlot(15)]
        IntPtr SaveInit(int size);

        [VTableSlot(16)]
        void SaveWriteFields(out CSaveRestoreData param0, [MarshalAs(UnmanagedType.LPStr)] string param1, IntPtr param2, out datamap_t param3, out typedescription_t param4, int param5);

        [VTableSlot(17)]
        void SaveReadFields(out CSaveRestoreData param0, [MarshalAs(UnmanagedType.LPStr)] string param1, IntPtr param2, out datamap_t param3, out typedescription_t param4, int param5);

        [VTableSlot(18)]
        void SaveGlobalState(out CSaveRestoreData param0);

        [VTableSlot(19)]
        void RestoreGlobalState(out CSaveRestoreData param0);

        [VTableSlot(20)]
        void PreSave(out CSaveRestoreData param0);

        [VTableSlot(21)]
        void Save(out CSaveRestoreData param0);

        [VTableSlot(22)]
        void GetSaveComment(IntPtr comment, int maxlength, float flMinutes, float flSeconds, bool bNoTime);

        [VTableSlot(23)]
        void WriteSaveHeaders(out CSaveRestoreData param0);

        [VTableSlot(24)]
        void ReadRestoreHeaders(out CSaveRestoreData param0);

        [VTableSlot(25)]
        void Restore(out CSaveRestoreData param0, bool param1);

        [VTableSlot(26)]
        bool IsRestoring();

        [VTableSlot(27)]
        int CreateEntityTransitionList(out CSaveRestoreData param0, int param1);

        [VTableSlot(28)]
        void BuildAdjacentMapList();

        [VTableSlot(29)]
        bool GetUserMessageInfo(int msg_type, IntPtr name, int maxnamelength, ref int size);

        [VTableSlot(30)]
        IntPtr GetStandardSendProxies();

        [VTableSlot(31)]
        void PostInit();

        [VTableSlot(32)]
        void Think(bool finalTick);

        [VTableSlot(33)]
        void PreSaveGameLoaded([MarshalAs(UnmanagedType.LPStr)] string pSaveName, bool bCurrentlyInGame);

        [VTableSlot(34)]
        bool ShouldHideServer();

        [VTableSlot(35)]
        void InvalidateMdlCache();

        [VTableSlot(36)]
        void OnQueryCvarValueFinished(QueryCvarCookie_t iCookie, out edict_t pPlayerEntity, EQueryCvarValueStatus eStatus, [MarshalAs(UnmanagedType.LPStr)] string pCvarName, [MarshalAs(UnmanagedType.LPStr)] string pCvarValue);

        [VTableSlot(37)]
        void GameServerSteamAPIActivated();

        [VTableSlot(38)]
        void GameServerSteamAPIShutdown();

        [VTableSlot(39)]
        void SetServerHibernation(bool bHibernating);

        [VTableSlot(40)]
        IntPtr GetServerGCLobby();

        [VTableSlot(41)]
        string GetServerBrowserMapOverride();

        [VTableSlot(42)]
        string GetServerBrowserGameData();

        [VTableSlot(43)]
        bool GMOD_CheckPassword(ulong param0, [MarshalAs(UnmanagedType.LPStr)] string param1, [MarshalAs(UnmanagedType.LPStr)] string param2, [MarshalAs(UnmanagedType.LPStr)] string param3, [MarshalAs(UnmanagedType.LPStr)] string param4, IntPtr param5, uint param6);

    }
}
