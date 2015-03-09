#if UNIT_TESTS

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Caching.Tests
{
	internal abstract class CacheProviderTestsBase
	{
		private ICacheProvider CreateProvider()
		{
			var p = NewProvider();
			p.Initialize(new CacheProviderInitializationArgs());
			return p;
		}

		protected abstract ICacheProvider NewProvider();

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_null_cacheID_not_legal()
		{
			var provider = CreateProvider();
			provider.CreateClient(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_empty_cacheID_not_legal()
		{
			var provider = CreateProvider();
			provider.CreateClient("");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Get_options_required()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				client.Get("a", null);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Get_Put_default_region()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(null, retrievedValue);

				var value = new object();
				client.Put("a", value, new CachePutOptions(TimeSpan.FromSeconds(10), true));
				
				retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Get_Put_named_region()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(null, retrievedValue);

				var value = new object();
				client.Put("a", value, new CachePutOptions("r1", TimeSpan.FromSeconds(10), true));

				retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Get_Put_different_regions()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(null, retrievedValue);

				var value = new object();
				client.Put("a", value, new CachePutOptions(TimeSpan.FromSeconds(10), true));

				// attempt to retrieve from r1 is still null, since we put it in with no region
				retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(null, retrievedValue);

				retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		// In order to minimize chances of programming error, we want to treat
		// Region == null and Region == "" to mean the same thing.
		[Test]
		public void Test_empty_string_region_maps_to_default_region()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var value = new object();

				// add value into region ""
				client.Put("a", value, new CachePutOptions(string.Empty, TimeSpan.FromSeconds(10), true));

				// retrieve value from default region
				var retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_no_options_maps_to_default_region()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var value = new object();

				// add value into default region
				client.Put("a", value, new CachePutOptions(TimeSpan.FromSeconds(10), true));

				var retrievedValue = client.Get("a");
				Assert.AreEqual(value, retrievedValue);

				client.Remove("a");

				retrievedValue = client.Get("a");
				Assert.AreEqual(null, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Clear_default_region()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var value = new object();
				client.Put("a", value, new CachePutOptions(TimeSpan.FromSeconds(1000), true));

				var retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(value, retrievedValue);

				client.ClearRegion(null);

				retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(null, retrievedValue);

				client.Put("a", value, new CachePutOptions(TimeSpan.FromSeconds(1000), true));
				retrievedValue = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Clear_named_region()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var value = new object();
				client.Put("a", value, new CachePutOptions("r1", TimeSpan.FromSeconds(1000), true));

				var retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(value, retrievedValue);

				client.ClearRegion("r1");

				retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(null, retrievedValue);

				client.Put("a", value, new CachePutOptions("r1", TimeSpan.FromSeconds(1000), true));
				retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Clear_different_regions()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var value = new object();
				client.Put("a", value, new CachePutOptions("r1", TimeSpan.FromSeconds(1000), true));

				var retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(value, retrievedValue);

				client.ClearRegion("r2");

				// clearing r2 doesn't affect r1
				retrievedValue = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(value, retrievedValue);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}

		[Test]
		public void Test_Clear_cache()
		{
			var provider = CreateProvider();
			using (var client = provider.CreateClient("foo"))
			{
				var value0 = new object();
				var value1 = new object();
				var value2 = new object();
				client.Put("a", value0, new CachePutOptions(TimeSpan.FromSeconds(1000), true));
				client.Put("a", value1, new CachePutOptions("r1", TimeSpan.FromSeconds(1000), true));
				client.Put("a", value2, new CachePutOptions("r2", TimeSpan.FromSeconds(1000), true));

				var r0Value = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(value0, r0Value);

				var r1Value = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(value1, r1Value);

				var r2Value = client.Get("a", new CacheGetOptions("r2"));
				Assert.AreEqual(value2, r2Value);

				client.ClearCache();

				r0Value = client.Get("a", new CacheGetOptions());
				Assert.AreEqual(null, r0Value);

				r1Value = client.Get("a", new CacheGetOptions("r1"));
				Assert.AreEqual(null, r1Value);

				r2Value = client.Get("a", new CacheGetOptions("r2"));
				Assert.AreEqual(null, r2Value);

				client.ClearCache(); // clear cache, so it doesn't mess up other tests
			}
		}
	}
}
#endif