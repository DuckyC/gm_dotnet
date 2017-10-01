using GSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{

    [ModuleName("engine")]
    [InterfaceVersion("VEngineServer021")]
    public interface IVEngineServer
    {
        // Tell engine to change level ( "changelevel s1\n" or "changelevel2 s1 s2\n" )
        void ChangeLevel(string s1, string s2);

        // Ask engine whether the specified map is a valid map file (exists and has valid version number).
        int IsMapValid(string filename);

        // Is this a dedicated server?
        bool IsDedicatedServer();

        // Is in Hammer editing mode?
        int IsInEditMode();

        // Add to the server/client lookup/precache table, the specified string is given a unique index
        // NOTE: The indices for PrecacheModel are 1 based
        //  a 0 returned from those methods indicates the model or sound was not correctly precached
        // However, generic and decal are 0 based
        // If preload is specified, the file is loaded into the server/client's cache memory before level startup, otherwise
        //  it'll only load when actually used (which can cause a disk i/o hitch if it occurs during play of a level).
        int PrecacheModel(string s, bool preload = false);
        int PrecacheSentenceFile(string s, bool preload = false);
        int PrecacheDecal(string name, bool preload = false);
        int PrecacheGeneric(string s, bool preload = false);

        // Check's if the name is precached, but doesn't actually precache the name if not...
        bool IsModelPrecached(string s);
        bool IsDecalPrecached(string s);
        bool IsGenericPrecached(string s);

        // Note that sounds are precached using the IEngineSound interface

        // Special purpose PVS checking
        // Get the cluster # for the specified position
        int GetClusterForOrigin(IntPtr Vectororg);
        // Get the PVS bits for a specified cluster and copy the bits into outputpvs.  Returns the number of bytes needed to pack the PVS
        int GetPVSForCluster(int cluster, int outputpvslength, byte[] outputpvs);
        // Check whether the specified origin is inside the specified PVS
        bool CheckOriginInPVS(IntPtr Vectororg, byte[] checkpvs, int checkpvssize);
        // Check whether the specified worldspace bounding box is inside the specified PVS
        bool CheckBoxInPVS(IntPtr Vectormins, IntPtr Vectormaxs, byte[] checkpvs, int checkpvssize);

        // Returns the server assigned userid for this player.  Useful for logging frags, etc.  
        //  returns -1 if the edict couldn't be found in the list of players.
        int GetPlayerUserId(IntPtr edict_e);
        string GetPlayerNetworkIDString(IntPtr edict_e);

        // Return the current number of used edict slots
        int GetEntityCount();
        // Given an edict, returns the entity index
        int IndexOfEdict(IntPtr edict_pEdict);
        // Given and entity index, returns the corresponding edict pointer
        IntPtr PEntityOfEntIndex(int iEntIndex);

        // Get stats info interface for a client netchannel
        IntPtr GetPlayerNetInfo(int playerIndex); // returns INetChannelInfo*

        // Allocate space for string and return index/offset of string in global string list
        // If iForceEdictIndex is not -1, then it will return the edict with that index. If that edict index
        // is already used, it'll return null.
        IntPtr CreateEdict(int iForceEdictIndex = -1);
        // Remove the specified edict and place back into the free edict list
        void RemoveEdict(IntPtr edict_e);

        // Memory allocation for entity class data
        IntPtr PvAllocEntPrivateData(long cb);
        void FreeEntPrivateData(IntPtr pEntity);

        // Save/restore uses a special memory allocator (which zeroes newly allocated memory, etc.)
        IntPtr SaveAllocMemory(UInt32 num, UInt32 size);
        void SaveFreeMemory(IntPtr pSaveMem);

        // Emit an ambient sound associated with the specified entity
        void EmitAmbientSound(int entindex, IntPtr Vectorpos, string samp, float vol, IntPtr soundlevel, int fFlags, int pitch, float delay = 0.0f);

        // Fade out the client's volume level toward silence (or fadePercent)
        void FadeClientVolume(IntPtr edict_pEdict, float fadePercent, float fadeOutSeconds, float holdTime, float fadeInSeconds);

        // Sentences / sentence groups
        int SentenceGroupPick(int groupIndex, string name, int nameBufLen);
        int SentenceGroupPickSequential(int groupIndex, string name, int nameBufLen, int sentenceIndex, int reset);
        int SentenceIndexFromName(string pSentenceName);
        string SentenceNameFromIndex(int sentenceIndex);
        int SentenceGroupIndexFromName(string pGroupName);
        string SentenceGroupNameFromIndex(int groupIndex);
        float SentenceLength(int sentenceIndex);

        // Issue a command to the command parser as if it was typed at the server console.	
        void ServerCommand(string str);
        // Execute any commands currently in the command parser immediately (instead of once per frame)
        void ServerExecute();
        // Issue the specified command to the specified client (mimics that client typing the command at the console).
        void ClientCommand();

        // Set the lightstyle to the specified value and network the change to any connected clients.  Note that val must not 
        //  change place in memory (use MAKE_STRING) for anything that's not compiled into your mod.
        void LightStyle(int style, string val);

        // Project a static decal onto the specified entity / model (for level placed decals in the .bsp)
        void StaticDecal(IntPtr VectororiginInEntitySpace, int decalIndex, int entityIndex, int modelIndex, bool lowpriority);

        // Given the current PVS(or PAS) and origin, determine which players should hear/receive the message
        void Message_DetermineMulticastRecipients(bool usepas, IntPtr origin, IntPtr playerbits);

        // Begin a message from a server side entity to its client side counterpart (func_breakable glass, e.g.)
        IntPtr EntityMessageBegin(int ent_index, IntPtr ent_class, bool reliable);
        // Begin a usermessage from the server to the client .dll
        IntPtr UserMessageBegin(IntPtr filter, int msg_type);
        // Finish the Entity or UserMessage and dispatch to network layer
        void MessageEnd();

        // Print szMsg to the client console.
        void ClientPrintf(IntPtr edict_pEdict, string szMsg);

        // SINGLE PLAYER/LISTEN SERVER ONLY (just matching the client .dll api for this)
        // Prints the formatted string to the notification area of the screen ( down the right hand edge
        //  numbered lines starting at position 0
        void Con_NPrintf();
        // SINGLE PLAYER/LISTEN SERVER ONLY(just matching the client .dll api for this)
        // Similar to Con_NPrintf, but allows specifying custom text color and duration information
        void Con_NXPrintf();

        // Change a specified player's "view entity" (i.e., use the view entity position/orientation for rendering the client view)
        void SetView(IntPtr edict_pClient, IntPtr edict_pViewent);

        // Get a high precision timer for doing profiling work
        float Time();

        // Set the player's crosshair angle
        void CrosshairAngle(IntPtr edict_pClient, float pitch, float yaw);

        // Get the current game directory (hl2, tf2, hl1, cstrike, etc.)
        void GetGameDir(string szGetGameDir, int maxlength);

        // Used by AI node graph code to determine if .bsp and .ain files are out of date
        int CompareFileTime(string filename1, string filename2, IntPtr iCompare);

        // Locks/unlocks the network string tables (.e.g, when adding bots to server, this needs to happen).
        // Be sure to reset the lock after executing your code!!!
        bool LockNetworkStringTables(bool @lock);

        // Create a bot with the given name.  Returns NULL if fake client can't be created
        IntPtr edict_CreateFakeClient(string netname);

        // Get a convar keyvalue for s specified client
        string GetClientConVarValue(int clientIndex, string name);

        // Parse a token from a file
        string ParseFile(string data, string token, int maxlen);
        // Copies a file
        bool CopyFile(string source, string destination);

        // Reset the pvs, pvssize is the size in bytes of the buffer pointed to by pvs.
        // This should be called right before any calls to AddOriginToPVS
        void ResetPVS(IntPtr pvs, int pvssize);
        // Merge the pvs bits into the current accumulated pvs based on the specified origin ( not that each pvs origin has an 8 world unit fudge factor )
        void AddOriginToPVS(IntPtr Vectororigin);
        // Mark a specified area portal as open/closes
        void SetAreaPortalState(int portalNumber, int isOpen);
        // Queue a temp entity for transmission
        void PlaybackTempEntity(IntPtr filter, float delay, IntPtr pSender, IntPtr pST, int classID);
        // Given a node number and the specified PVS, return with the node is in the PVS
        int CheckHeadnodeVisible(int nodenum, IntPtr pvs, int vissize);
        // Using area bits, cheeck whether area1 flows into area2 and vice versa (depends on area portal state)
        int CheckAreasConnected(int area1, int area2);
        // Given an origin, determine which area index the origin is within
        int GetArea(IntPtr Vectororigin);
        // Get area portal bit set
        void GetAreaBits(int area, byte[] bits, int buflen);
        // Given a view origin (which tells us the area to start looking in) and a portal key,
        // fill in the plane that leads out of this area (it points into whatever area it leads to).
        bool GetAreaPortalPlane(IntPtr vViewOrigin, int portalKey, IntPtr pPlane);

        // Save/restore wrapper - FIXME:  At some point we should move this to it's own interface
        bool LoadGameState(string pMapName, bool createPlayers);
        void LoadAdjacentEnts(string pOldLevel, string pLandmarkName);
        void ClearSaveDir();

        // Get the pristine map entity lump string.  (e.g., used by CS to reload the map entities when restarting a round.)
        string GetMapEntitiesString();

        // Text message system -- lookup the text message of the specified name
        IntPtr TextMessageGet(string pName);

        // Print a message to the server log file
        void LogPrint(string msg);

        // Builds PVS information for an entity
        void BuildEntityClusterList(IntPtr edict_pEdict, IntPtr pPVSInfo);

        // A solid entity moved, update spatial partition
        void SolidMoved(IntPtr edict_pSolidEnt, IntPtr pSolidCollide, IntPtr pPrevAbsOrigin);
        // A trigger entity moved, update spatial partition
        void TriggerMoved(IntPtr edict_pTriggerEnt);

        // Create/destroy a custom spatial partition
        IntPtr CreateSpatialPartition(IntPtr worldmin, IntPtr worldmax);
        void DestroySpatialPartition(IntPtr ISpatialPartition);

        // Draw the brush geometry in the map into the scratch pad.
        // Flags is currently unused.
        void DrawMapToScratchPad(IntPtr pPad, ulong iFlags);

        // This returns which entities, to the best of the server's knowledge, the client currently knows about.
        // This is really which entities were in the snapshot that this client last acked.
        // This returns a bit vector with one bit for each entity.
        //
        // USE WITH CARE. Whatever tick the client is really currently on is subject to timing and
        // ordering differences, so you should account for about a quarter-second discrepancy in here.
        // Also, this will return NULL if the client doesn't exist or if this client hasn't acked any frames yet.
        // 
        // iClientIndex is the CLIENT index, so if you use pPlayer->entindex(), subtract 1.
        IntPtr GetEntityTransmitBitsForClient(int iClientIndex);

        // Is the game paused?
        bool IsPaused();

        // Marks the filename for consistency checking.  This should be called after precaching the file.
        void ForceExactFile(string s);
        void ForceModelBounds(string s, IntPtr Vectormins, IntPtr Vectormaxs);
        void ClearSaveDirAfterClientLoad();

        // Sets a USERINFO client ConVar for a fakeclient
        void SetFakeClientConVarValue(IntPtr edict_pEntity, string cvar, string value);

        // Marks the material (vmt file) for consistency checking.  If the client and server have different
        // contents for the file, the client's vmt can only use the VertexLitGeneric shader, and can only
        // contain $baseTexture and $bumpmap vars.
        void ForceSimpleMaterial(string materiual);

        // Is the engine in Commentary mode?
        int IsInCommentaryMode();


        // Mark some area portals as open/closed. It's more efficient to use this
        // than a bunch of individual SetAreaPortalState calls.
        void SetAreaPortalStates(int[] portalNumbers, int[] isOpen, int nPortals);

        // Called when relevant edict state flags change.
        void NotifyEdictFlagsChange(int iEdict);

        // Only valid during CheckTransmit. Also, only the PVS, networked areas, and
        // m_pTransmitInfo are valid in the returned strucutre.
        IntPtr GetPrevCheckTransmitInfo(IntPtr edict_pPlayerEdict);

        IntPtr GetSharedEdictChangeInfo();

        // Tells the engine we can immdiately re-use all edict indices
        // even though we may not have waited enough time
        void AllowImmediateEdictReuse();

        // Returns true if the engine is an internal build. i.e. is using the internal bugreporter.
        bool IsInternalBuild();

        IntPtr GetChangeAccessor(IntPtr edict_pEdict);

        // Name of most recently load .sav file
        string GetMostRecentlyLoadedFileName();
        string GetSaveFileName();

        // Matchmaking
        void MultiplayerEndGame();
        void ChangeTeam(string pTeamName);

        // Cleans up the cluster list
        void CleanUpEntityClusterList(IntPtr pPVSInfo);

        void SetAchievementMgr(IntPtr pAchievementMgr);
        IntPtr GetAchievementMgr();

        int GetAppID();

        bool IsLowViolence();

        // Call this to find out the value of a cvar on the client.
        //
        // It is an asynchronous query, and it will call IServerGameDLL::OnQueryCvarValueFinished when
        // the value comes in from the client.
        //
        // Store the return value if you want to match this specific query to the OnQueryCvarValueFinished call.
        // Returns InvalidQueryCvarCookie if the entity is invalid.
        IntPtr StartQueryCvarValue(IntPtr edict_pPlayerEntity, string pName);

        void InsertServerCommand(string str);
        // Fill in the player info structure for the specified player index (name, model, etc.)
        bool GetPlayerInfo(int ent_num, IntPtr pinfo);

        // Returns true if this client has been fully authenticated by Steam
        bool IsClientFullyAuthenticated(IntPtr pEdict);

        // This makes the host run 1 tick per frame instead of checking the system timer to see how many ticks to run in a certain frame.
        // i.e. it does the same thing timedemo does.
        void SetDedicatedServerBenchmarkMode(bool bBenchmarkMode);

        // Methods to set/get a gamestats data container so client & server running in same process can send combined data
        void SetGamestatsData(IntPtr pGamestatsData);
        IntPtr GetGamestatsData();

        // Returns the SteamID of the specified player. It'll be NULL if the player hasn't authenticated yet.
        Int64 GetClientSteamID(IntPtr edict_pPlayerEdict);

        // Returns the SteamID of the game server
        Int64 GetGameServerSteamID();

        // Send a client command keyvalues
        // keyvalues are deleted inside the function
        void ClientCommandKeyValues(IntPtr pEdict, IntPtr pCommand);

        // Returns the SteamID of the specified player. It'll be NULL if the player hasn't authenticated yet.
        Int64 GetClientSteamIDByPlayerIndex(int entnum);
        // Gets a list of all clusters' bounds.  Returns total number of clusters.
        int GetClusterCount();
        int GetAllClusterBounds(IntPtr pBBoxList, int maxBBox);

        // Create a bot with the given name.  Returns NULL if fake client can't be created
        IntPtr CreateFakeClientEx(string netname, bool bReportFakeClient = true);

        // Server version from the steam.inf, this will be compared to the GC version
        int GetServerVersion();

        IntPtr GMOD_SetTimeManipulator(float fScaleFramerate);
        void GMOD_SendToClient(IntPtr filter, IntPtr data, int dataSize);
        void GMOD_SendToClient(int client, IntPtr data, int dataSize);
        void GMOD_RawServerCommand(string command);
        IntPtr GMOD_CreateDataTable();
        void GMOD_DestroyDataTable(IntPtr dataTable);
        string GMOD_GetServerAddress();
        IntPtr GMOD_LoadModel( string path );

        IntPtr GetReplay();
    }
}
