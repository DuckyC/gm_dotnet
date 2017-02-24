using GSharp.Attributes;
using GSharp.GLuaNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{
    public interface ILuaInterface
    {
        [VTableSlot(1)]
        int GetIRef();
        [VTableSlot(2)]
        bool Init();
        [VTableSlot(3)]
        void Shutdown();
        [VTableSlot(4)]
        void Cycle();
        [VTableSlot(5)]
        IntPtr GetLuaState();
        // Stack
        [VTableSlot(6)]
        void Pop(int i = 1);
        // Get
        [VTableSlot(7)]
        void GetGlobal(IntPtr ILuaObject, string name );
        [VTableSlot(8)]
        IntPtr GetGlobal( string name ); //returns ILuaObject

        [VTableSlot(9)]
        IntPtr GetObject(int i = -1);
        [VTableSlot(10)]
        string GetString(int i = -1 );
        [VTableSlot(11)]
        int GetInteger(int i = -1);
        [VTableSlot(12)]
        float GetNumber(int i = -1);
        [VTableSlot(13)]
        bool GetBool(int i = -1);

        [VTableSlot(14)]
        IntPtr GetUserDataPtr(int i = -1); //returns void**
        [VTableSlot(15)]
        IntPtr GetUserData(int i = -1);

        [VTableSlot(16)]
        void GetTable(int i = -1);

        // References
        [VTableSlot(17)]
        int GetReference(int i = -1, bool bPopValue = false);
        [VTableSlot(18)]
        void FreeReference(int i);
        [VTableSlot(19)]
        void PushReference(int i) ;

        // Push
        [VTableSlot(20)]
        void Push(IntPtr ILuaObject );
        [VTableSlot(21)]
        void Push(string str );
        [VTableSlot(22)]
        void PushVA(string str, object param1 );
        [VTableSlot(23)]
        void Push(float f);
        [VTableSlot(24)]
        void Push(bool b);
        [VTableSlot(25)]
        void Push(lua_CFunction f);

        [VTableSlot(26)]
        void SetGlobal( string namename, IntPtr ILuaObject = default(IntPtr) );
        [VTableSlot(27)]
        void SetGlobal( string namename, bool b );
        [VTableSlot(28)]
        void SetGlobal( string namename, float f );
        [VTableSlot(29)]
        void SetGlobal( string namename, string s );
        [VTableSlot(30)]
        void SetGlobal( string namename, lua_CFunction f );
        [VTableSlot(31)]
        void NewTable();

        [VTableSlot(32)]
        void LuaError( string param1, int argument = -1 );
        [VTableSlot(33)]
        void TypeError( string name, int argument );
        [VTableSlot(34)]
        int GetType(int iStackPos);
        [VTableSlot(35)]
        string GetTypeName(int iType );

        [VTableSlot(36)]
        bool Call(int args, int returns = 0);
        [VTableSlot(37)]
        bool Call(IntPtr ILuaObject_func, IntPtr LArgList_in, IntPtr LArgList_out = default(IntPtr));
        //[VTableSlot(38)]
        //bool Call(IntPtr ILuaObject_func, IntPtr LArgList_in, IntPtr ILuaObject_member); idk?

        [VTableSlot(39)]
        void SetMember(IntPtr ILuaObject_table, string name );
        [VTableSlot(40)]
        void SetMember(IntPtr ILuaObject_table, string namename, IntPtr ILuaObject_member );

        [VTableSlot(41)]
        int Top();

        [VTableSlot(42)]
        IntPtr NewUserData(IntPtr ILuaObject_obj); //returns ILuaObject
        [VTableSlot(43)]
        void PushUserData(IntPtr ILuaObject_metatable, IntPtr v);

        [VTableSlot(44)]
        void NewGlobalTable( string param1 );
        [VTableSlot(45)]
        IntPtr NewTemporaryObject(); //returns ILuaObject

        [VTableSlot(46)]
        bool isUserData(int i = -1);

        // GetMetaTable creates the meta table if it doesn't exist. Make sure type is unique to your
        // meta type. The default types are defined above (TYPE_INVALID etc). You should ideally make your type
        // some random large number that is in the 1000's - to be sure that it won't conflict with the base
        // meta types when more are added at a later date. Name isn't so important - it's just used for GetTypeName
        // and in Lua using type().
        [VTableSlot(47)]
        IntPtr GetMetaTable( string strName, int Type ); //returns ILuaObject
        [VTableSlot(48)]
        IntPtr GetMetaTable(int i); // Returns the metatable of an object on the stack //returns ILuaObject
        [VTableSlot(49)]
        void SetMetaTable(IntPtr ILuaObject_obj, IntPtr ILuaObject_metatable);
        [VTableSlot(50)]
        void CheckType(int i, int iType);

        [VTableSlot(51)]
        IntPtr GetReturn(int iNum); //returns ILuaObject

        // Added 10th December 2006
        [VTableSlot(52)]
        bool IsServer();
        [VTableSlot(53)]
        bool IsClient();
        [VTableSlot(54)]
        bool IsDedicatedServer();

        // Added 20th December 2006
        [VTableSlot(55)]
        void SetMember(IntPtr ILuaObject_table, float name);
        [VTableSlot(56)]
        void SetMember(IntPtr ILuaObject_table, float namename, IntPtr ILuaObject_member);

        // Added 30th December 2006
        [VTableSlot(57)]
        IntPtr GetNewTable();      // Makes a new table and returns it //returns ILuaObject

        // Added 09/Jan/2007
        [VTableSlot(58)]
        void SetMember(IntPtr ILuaObject_table, IntPtr ILuaObject_key, IntPtr ILuaObject_value);

        // Added 12/01/2007
        [VTableSlot(59)]
        void DebugPoint();

        // See IModuleManager.h 
        [VTableSlot(60)]
        IntPtr GetModuleManager(); //ILuaModuleManager

        // Set whether this is serverside, which prevents things like filename translating
        [VTableSlot(61)]
        void SetIsServer(bool b);

        // Pushes a LONG (CRCs etc)
        [VTableSlot(62)]
        void PushLong(long f);
        // Pushes value on stack to the top of the stack
        [VTableSlot(63)]
        void PushValue(int i);
        // Pushes a nil onto the stack
        [VTableSlot(64)]
        void PushNil();
        // Returns a number. Item at stackpos can be a table, in which case all the 
        // numbers in the table will be |'d together to make the flag
        [VTableSlot(65)]
        int GetFlags(int iStackPos);

        // Quickly searches metatable of iObject for members iKey
        [VTableSlot(66)]
        bool FindOnObjectsMetaTable(int iObject, int iKey);
        // Quickly searches table using item on stack
        [VTableSlot(67)]
        bool FindObjectOnTable(int iTable, int iKey);
        // Quickly sets table member using items on the stack
        [VTableSlot(68)]
        void SetMemberFast(IntPtr ILuaObject_table, int iKey, int iValue);

        // If bRun is false the string is converted to a function, which is left on top of the stack.
        [VTableSlot(69)]
        bool RunString( string strFilename, string strPath, string strStringToRun, bool bRun, bool bShowErrors );

        // Returns true if the objects are equal. May call metatables.
        [VTableSlot(70)]
        bool IsEqual(IntPtr ILuaObject_pObjectA, IntPtr ILuaObject_pObjectB);

        // Throws an error. Interrupts execution.
        [VTableSlot(71)]
        void Error( string strError );

        // Throws a type error if the string is NULL..
        [VTableSlot(72)]
        string GetStringOrError(int i );


        // C++ equivilent of 'require'
        [VTableSlot(73)]
        bool RunModule( string strName );

        // Finds and runs a script.
        // If bRun is false, the script isn't run, but the 'function' block is left on the stack, ready to be run
        // Returns false if run was not successful
        [VTableSlot(74)]
        bool FindAndRunScript( string strFilename, bool bRun, bool bReportErrors );

        // Internal use. Sets the search path ID to load Lua from.
        [VTableSlot(75)]
        void SetPathID( string param1);
        [VTableSlot(76)]
        string GetPathID();

        // Errors in t7he console without actually stopping the code from running
        [VTableSlot(77)]
        void ErrorNoHalt( string fmt, string param2 );

        // Returns the Lua string length of string on stack. Lua strings can have NULLs.
        [VTableSlot(78)]
        int StringLength(int i);

        // Simply sets the named global to nil
        [VTableSlot(79)]
        void RemoveGlobal( string strName );

        // How many items are there on the stack
        [VTableSlot(80)]
        int GetStackTop();

        // Gets the members from table on stack
        // Note: You MUST free the result when you're done using DeleteLuaVector.

        [VTableSlot(81)]
        IntPtr GetAllTableMembers(int iTable);
        [VTableSlot(82)]
        void DeleteLuaVector(IntPtr pVector);

        // Simple Lua Msg. Is redirected to the ILuaCallback class, which
        // will display the text differently depending on which Lua instrance you're using.
        [VTableSlot(83)]
        void Msg( string fmt, string param2 );

        // Used to push the path of the current folder onto a stack.
        // This allows us to load files relative to that folder.
        [VTableSlot(84)]
        void PushPath( string strPath );
        [VTableSlot(85)]
        void PopPath();
        [VTableSlot(86)]
        string GetPath();

        // Used by the Lua file loading logic to determine whether it should
        // try to use downloaded files or just the regular ones.
        [VTableSlot(87)]
        bool ShouldTranslateLuaNames();
        [VTableSlot(88)]
        void SetShouldTranslateLuaNames(bool bTranslate);

        // Push/Get a simple pointer. Not garbage collected, no metatables.
        [VTableSlot(89)]
        void PushLightUserData(IntPtr pData);
        [VTableSlot(90)]
        IntPtr GetLightUserData(int i);

        // Thread safety. 
        [VTableSlot(91)]
        void Lock();
        [VTableSlot(92)]
        void UnLock();

        [VTableSlot(93)]
        void SetGlobalDouble( string namename, double iValue );

        [VTableSlot(94)]
        double GetDouble(int iPos);
        [VTableSlot(95)]
        void PushDouble(double iInt);

        [VTableSlot(107)]
        void RunStringEx(IntPtr _this, string filename, string path, string torun, bool run, bool showerrors, bool idk, bool idk2); // thanks to gm_roc
    }
}
