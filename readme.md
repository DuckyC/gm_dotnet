# GSharp

GSharp is a C# Shared project that makes it easy to interact with the C side of GMod.

# Creating a new module

  1. Clone this repo
  2. Create a new dll in visual studio
  3. Go to project properties>Build and set Platform target to x86
  4. Install the nuget package [UnmanagedExports](https://www.nuget.org/packages/UnmanagedExports)
  5. Add the GSharp shared project as a reference
  6. Make a class like the following:
  ```csharp
public class Module
{

    [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
    public static int Open(lua_state L)
    {
        return 0;
    }

    [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
    public static int Close(IntPtr L)
    {
        return 0;
    }
}
```
  7. Start writing your code!