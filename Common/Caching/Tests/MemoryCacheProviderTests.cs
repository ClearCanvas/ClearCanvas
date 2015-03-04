#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.Common.Caching.Tests
{
	[TestFixture]
	class MemoryCacheProviderTests : CacheProviderTestsBase
	{
		protected override ICacheProvider NewProvider()
		{
			return new MemoryCacheProvider();
		}
	}
}

#endif
