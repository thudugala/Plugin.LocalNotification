using Foundation;

#if XAMARINIOS
[assembly: LinkerSafe]
#elif IOS
[assembly: System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
#endif

