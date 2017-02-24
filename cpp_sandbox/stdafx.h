#pragma once

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#ifdef __cplusplus
#define EXTERN_C extern "C"
#else
#define EXTERN_C
#endif

#define EXPORT EXTERN_C __declspec(dllexport)