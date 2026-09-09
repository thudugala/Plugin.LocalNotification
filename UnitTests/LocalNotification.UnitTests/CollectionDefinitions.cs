using Xunit.Sdk;
using Xunit.v3;

[assembly: Parallelization(Mode = ParallelMode.None)]

namespace Plugin.LocalNotification.UnitTests;

/// <summary>
/// Defines a collection for test classes that share the <see cref="LocalNotificationCenter.Serializer"/>
/// static state. Tests within this collection run sequentially to prevent race conditions.
/// </summary>
[CollectionDefinition("SharedStaticState")]
public class SharedStaticStateCollection { }
