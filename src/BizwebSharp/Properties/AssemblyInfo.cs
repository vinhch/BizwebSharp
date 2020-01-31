using System.Runtime.CompilerServices;

#if (DEBUG)
// Allow "Friend" assemblies access to internals
// https://docs.microsoft.com/en-us/dotnet/standard/assembly/friend
[assembly: InternalsVisibleTo("BizwebSharp.Tests.xUnit")]
#endif
