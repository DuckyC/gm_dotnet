#include <GarrysMod/Lua/Interface.h>
#include <stdio.h>

GMOD_DLL_EXPORT int gmod13_open(lua_State* L) GMOD_NOEXCEPT        
{
	printf("adress of luabase: %X", (long)L->luabase);
	return 0;
}

GMOD_MODULE_CLOSE()
{
	
	return 0;
}