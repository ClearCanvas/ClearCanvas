#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;
using Castle.Core.Interceptor;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Tests
{
	[TestFixture]
	public class ResponseCachingAdviceBaseTests
	{
		#region TestExtensionFactory
		class TestExtensionFactory : IExtensionFactory
		{
			public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
			{
				if (extensionPoint is CacheProviderExtensionPoint)
				{
					return new[] { new TestCacheProvider() };
				}

				throw new NotImplementedException();
			}

			public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
			{
				if (extensionPoint is CacheProviderExtensionPoint)
				{
					return new[] { new ExtensionInfo(typeof(TestCacheProvider), typeof(CacheProviderExtensionPoint), null, null, true) };
				}
				throw new NotImplementedException();
			}
		}

		#endregion

		#region TestCacheProvider
		class TestCacheProvider : ICacheProvider
		{
			public void Initialize(CacheProviderInitializationArgs args)
			{
			}

			public ICacheClient CreateClient(string cacheId)
			{
				return new TestCacheClient();
			}
		}
		#endregion

		#region TestCacheClient
		class TestCacheClient : ICacheClient
		{
			class ValueOptionsPair
			{
				public object Value { get; set; }
				public CacheOptionsBase Options { get; set; }
			}


			private static readonly Dictionary<string, ValueOptionsPair> _cache = new Dictionary<string, ValueOptionsPair>();

			public string GetRegion(string key)
			{
				ValueOptionsPair value;
				return _cache.TryGetValue(key, out value) ? value.Options.Region : null;
			}


			public string CacheID
			{
				get { return "test"; }
			}

			public object Get(string key, CacheGetOptions options)
			{
				ValueOptionsPair value;
				return _cache.TryGetValue(key, out value) ? value.Value : null;
			}

			public object Get(string key)
			{
				return Get(key, new CacheGetOptions());
			}

			public void Put(string key, object value, CachePutOptions options)
			{
				_cache[key] = new ValueOptionsPair { Value = value, Options = options };
			}

			public void Remove(string key, CacheRemoveOptions options)
			{
				_cache.Remove(key);
			}

			public void Remove(string key)
			{
				Remove(key, new CacheRemoveOptions());
			}

			public bool RegionExists(string region)
			{
				return true;
			}

			public void ClearRegion(string region)
			{
				_cache.Clear();
			}

			public void ClearCache()
			{
				_cache.Clear();
			}

			public void Dispose()
			{
			}
		}
		#endregion

		#region TestInvocation
		class TestInvocation : IInvocation
		{
			private Type _targetType;
			public object Request { get; set; }
			public object Response { get; set; }
			public object Target { get; set; }

			public bool DidProceed { get; private set; }


			#region IInvocation Members

			public MethodInfo Method { get; set; }
			public object ReturnValue { get; set; }

			public object[] Arguments
			{
				get { return new[] { Request }; }
			}

			public void Proceed()
			{
				// put response into return value field
				ReturnValue = Response;
				DidProceed = true;
			}

			public Type TargetType
			{
				get { return _targetType ?? Target.GetType(); }
				set { _targetType = value; }
			}

			public Type[] GenericArguments
			{
				get { throw new NotImplementedException(); }
			}

			public object GetArgumentValue(int index)
			{
				throw new NotImplementedException();
			}

			public MethodInfo GetConcreteMethod()
			{
				throw new NotImplementedException();
			}

			public MethodInfo GetConcreteMethodInvocationTarget()
			{
				throw new NotImplementedException();
			}

			public object InvocationTarget
			{
				get
				{
					// not sure about this... 
					return this.Target;
				}
			}

			public MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public object Proxy
			{
				get { throw new NotImplementedException(); }
			}

			public void SetArgumentValue(int index, object value)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
		#endregion

		#region ConcreteResponseCachingAdvice
		class ConcreteResponseCachingAdvice : ResponseCachingAdviceBase, IInterceptor
		{
			private readonly ResponseCachingDirective _directive;

			public void Intercept(IInvocation invocation)
			{
				ProcessInvocation(invocation, "foo");
			}

			public ConcreteResponseCachingAdvice(ResponseCachingDirective directive)
			{
				_directive = directive;
			}

			protected override ResponseCachingDirective GetCachingDirective(IInvocation invocation)
			{
				return _directive;
			}

			protected override void CacheResponse(IInvocation invocation, object request, ICacheClient cacheClient, string region, ResponseCachingDirective directive)
			{
				PutResponseInCache(invocation, request, cacheClient, region, directive);
			}
		}
		#endregion

		class TestConfigurationService : IConfigurationService
		{
			public ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request)
			{
				throw new NotImplementedException();
			}

			public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
			{
				throw new NotImplementedException();
			}

			public ListConfigurationDocumentsResponse ListConfigurationDocuments(ListConfigurationDocumentsRequest request)
			{
				throw new NotImplementedException();
			}

			public GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request)
			{
				throw new NotImplementedException();
			}

			public ImportSettingsGroupResponse ImportSettingsGroup(ImportSettingsGroupRequest request)
			{
				throw new NotImplementedException();
			}

			public SetConfigurationDocumentResponse SetConfigurationDocument(SetConfigurationDocumentRequest request)
			{
				throw new NotImplementedException();
			}

			public RemoveConfigurationDocumentResponse RemoveConfigurationDocument(RemoveConfigurationDocumentRequest request)
			{
				throw new NotImplementedException();
			}
		}

		class MyService
		{
			public object MyServiceOperation(object request)
			{
				throw new NotImplementedException();
			}
		}

		class CacheableRequest : IDefinesCacheKey
		{

			#region IDefinesCacheKey Members

			public string GetCacheKey()
			{
				return GetHashCode().ToString();
			}

			#endregion
		}

		class NonCacheableRequest
		{

		}

		public ResponseCachingAdviceBaseTests()
		{
			Platform.SetExtensionFactory(
				new UnitTestExtensionFactory(
					new Dictionary<Type, Type> {{typeof (CacheProviderExtensionPoint), typeof (TestCacheProvider)}}
				)
			);
		}

		[Test]
		public void TestCacheConfigurationDocument()
		{
			var cache = new TestCacheClient();
			cache.ClearCache();

			var documentKey = new ConfigurationDocumentKey("Test", new Version(1, 0), null, "");
			var cacheKey = ((IDefinesCacheKey) documentKey).GetCacheKey();

			var service = new TestConfigurationService();
			object request = new GetConfigurationDocumentRequest(documentKey);
			object response = new GetConfigurationDocumentResponse(documentKey, DateTime.Now, DateTime.Now, "Test");
			var invocation = new TestInvocation
			{
				Target = service,
				Method = typeof(IApplicationConfigurationReadService).GetMethod("GetConfigurationDocument", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
				TargetType = typeof(IApplicationConfigurationReadService),
				Request = request,
				Response = response
			};

			var directive = new ResponseCachingDirective(true, TimeSpan.FromMinutes(1), ResponseCachingSite.Server);
			var advice = new ConcreteResponseCachingAdvice(directive);
			advice.Intercept(invocation);

			var cacheEntry = cache.Get(cacheKey, new CacheGetOptions(""));
			Assert.IsNotNull(cacheEntry);
			Assert.AreEqual(response, cacheEntry);

			request = new SetConfigurationDocumentRequest(documentKey, "Test");
			response = new SetConfigurationDocumentResponse();

			invocation = new TestInvocation
			{
				Target = service,
				Method = typeof(IConfigurationService).GetMethod("SetConfigurationDocument", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
				TargetType = typeof(IConfigurationService),
				Request = request,
				Response = response
			};

			advice = new ConcreteResponseCachingAdvice(null);
			advice.Intercept(invocation);

			cacheEntry = cache.Get(cacheKey, new CacheGetOptions(""));
			Assert.IsNull(cacheEntry);
		}

		[Test]
		public void TestCacheListSettingsGroups()
		{
			var cache = new TestCacheClient();
			cache.ClearCache();

			var service = new TestConfigurationService();
			object request = new ListSettingsGroupsRequest();
			object response = new ListSettingsGroupsResponse(new List<SettingsGroupDescriptor>());
			var invocation = new TestInvocation
			{
				Target = service,
				Method = typeof(IApplicationConfigurationReadService).GetMethod("ListSettingsGroups", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
				TargetType = typeof(IApplicationConfigurationReadService),
				Request = request,
				Response = response
			};

			var directive = new ResponseCachingDirective(true, TimeSpan.FromMinutes(1), ResponseCachingSite.Server);
			var advice = new ConcreteResponseCachingAdvice(directive);
			advice.Intercept(invocation);

			var cacheEntry = cache.Get("ListSettingsGroups", new CacheGetOptions(""));
			Assert.IsNotNull(cacheEntry);

			request = new ImportSettingsGroupRequest(
				new SettingsGroupDescriptor("Test", new Version(1,0), "Test", "Test", true), 
				new List<SettingsPropertyDescriptor>(new[]{new SettingsPropertyDescriptor("Test", "Test", "Test", SettingScope.User, "Test") }));
			response = new ImportSettingsGroupResponse();

			invocation = new TestInvocation
			{
				Target = service,
				Method = typeof(IConfigurationService).GetMethod("ImportSettingsGroup", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
				TargetType = typeof(IConfigurationService),
				Request = request,
				Response = response
			};

			advice = new ConcreteResponseCachingAdvice(null);
			advice.Intercept(invocation);

			cacheEntry = cache.Get("ListSettingsGroups", new CacheGetOptions(""));
			Assert.IsNull(cacheEntry);
		}

		[Test]
		public void Test_UsingTestCacheClient()
		{
			// simple test to ensure the extension point stubbing is working
			using (Cache.CreateClient("foo"))
			{

			}
		}

		[Test]
		public void Test_NonCacheableRequest_NullCacheDirective()
		{
			var target = new MyService();
			var request = new NonCacheableRequest();
			var response = new object();
			var invocation = new TestInvocation
								{
									Target = target,
									Method = target.GetType().GetMethod("MyServiceOperation"),
									Request = request,
									Response = response
								};

			var advice = new ConcreteResponseCachingAdvice(null);
			advice.Intercept(invocation);

			Assert.IsTrue(invocation.DidProceed);
			Assert.AreEqual(invocation.ReturnValue, response);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_NonCacheableRequest_TypicalCacheDirective()
		{
			var target = new MyService();
			var request = new NonCacheableRequest();
			var response = new object();
			var directive = new ResponseCachingDirective(true, TimeSpan.FromMinutes(1), ResponseCachingSite.Server);
			var invocation = new TestInvocation
			{
				Target = target,
				Method = target.GetType().GetMethod("MyServiceOperation"),
				Request = request,
				Response = response
			};

			// a non-null cache directive on a non-cacheable request type should throw
			var advice = new ConcreteResponseCachingAdvice(directive);
			advice.Intercept(invocation);
		}

		[Test]
		public void Test_CacheableRequest_NullCacheDirective()
		{
			var target = new MyService();
			var request = new CacheableRequest();
			var response = new object();
			var invocation = new TestInvocation
			{
				Target = target,
				Method = target.GetType().GetMethod("MyServiceOperation"),
				Request = request,
				Response = response
			};

			var advice = new ConcreteResponseCachingAdvice(null);
			advice.Intercept(invocation);

			Assert.IsTrue(invocation.DidProceed);
			Assert.AreEqual(invocation.ReturnValue, response);

			// check that response was not cached
			var cache = new TestCacheClient();
			var cacheEntry = cache.Get(request.GetCacheKey(), new CacheGetOptions(""));
			Assert.IsNull(cacheEntry);
		}

		[Test]
		public void Test_CacheableRequest_DoNotCacheDirective()
		{
			var target = new MyService();
			var request = new CacheableRequest();
			var response = new object();
			var directive = new ResponseCachingDirective(false, TimeSpan.FromMinutes(1), ResponseCachingSite.Server);
			var invocation = new TestInvocation
			{
				Target = target,
				Method = target.GetType().GetMethod("MyServiceOperation"),
				Request = request,
				Response = response
			};

			var advice = new ConcreteResponseCachingAdvice(directive);
			advice.Intercept(invocation);

			Assert.IsTrue(invocation.DidProceed);
			Assert.AreEqual(invocation.ReturnValue, response);

			// check that response was not cached
			var cache = new TestCacheClient();
			var cacheEntry = cache.Get(request.GetCacheKey(), new CacheGetOptions(""));
			Assert.IsNull(cacheEntry);
		}

		[Test]
		public void Test_CacheableRequest_TtlZeroCacheDirective()
		{
			var target = new MyService();
			var request = new CacheableRequest();
			var response = new object();
			var directive = new ResponseCachingDirective(true, TimeSpan.Zero, ResponseCachingSite.Server);
			var invocation = new TestInvocation
			{
				Target = target,
				Method = target.GetType().GetMethod("MyServiceOperation"),
				Request = request,
				Response = response
			};

			var advice = new ConcreteResponseCachingAdvice(directive);
			advice.Intercept(invocation);

			Assert.IsTrue(invocation.DidProceed);
			Assert.AreEqual(invocation.ReturnValue, response);

			// check that response was not cached
			var cache = new TestCacheClient();
			var cacheEntry = cache.Get(request.GetCacheKey(), new CacheGetOptions(""));
			Assert.IsNull(cacheEntry);
		}

		[Test]
		public void Test_CacheableRequest_TypicalCacheDirective()
		{
			var target = new MyService();
			var request = new CacheableRequest();
			var response = new object();
			var directive = new ResponseCachingDirective(true, TimeSpan.FromMinutes(1), ResponseCachingSite.Server);
			var invocation = new TestInvocation
			{
				Target = target,
				Method = target.GetType().GetMethod("MyServiceOperation"),
				Request = request,
				Response = response
			};

			var advice = new ConcreteResponseCachingAdvice(directive);
			advice.Intercept(invocation);

			// check invocation proceeded and return value set
			Assert.IsTrue(invocation.DidProceed);
			Assert.AreEqual(invocation.ReturnValue, response);

			// check that response was cached
			var cache = new TestCacheClient();
			var cacheEntry = cache.Get(request.GetCacheKey(), new CacheGetOptions(""));
			Assert.AreEqual(response, cacheEntry);

			// check that it was cached in the correct region
			var region = cache.GetRegion(request.GetCacheKey());
			Assert.AreEqual(typeof(MyService).FullName + ".MyServiceOperation", region);

			// second invocation
			var invocation2 = new TestInvocation
			{
				Target = target,
				Method = target.GetType().GetMethod("MyServiceOperation"),
				Request = request,
				Response = response
			};

			// check 2nd invocation did not proceed, but return value is still set correctly from cache
			Assert.IsFalse(invocation2.DidProceed);
			Assert.AreEqual(invocation.ReturnValue, response);
		}
	}
}

#endif
