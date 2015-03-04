#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.Common.Caching.Tests
{
	[TestFixture]
	class AspWebCacheProviderTests : CacheProviderTestsBase
	{
		protected override ICacheProvider NewProvider()
		{
			return new AspWebCacheProvider();
		}
	}
}

#endif
