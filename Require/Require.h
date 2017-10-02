// Require.h
#pragma once

#define GMOD_DLL_EXPORT extern "C" __declspec( dllexport )
#define GMOD_MODULE_OPEN()  GMOD_DLL_EXPORT int gmod13_open( void* state )
#define GMOD_MODULE_CLOSE() GMOD_DLL_EXPORT int gmod13_close( void* state )