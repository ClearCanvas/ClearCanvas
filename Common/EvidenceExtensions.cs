#if __MonoCS__

using System;
using System.Security.Policy;
using System.Linq;

namespace ClearCanvas.Common
{
	public static class EvidenceExtensions
	{
		public static void AddHostEvidence(this Evidence evidence, EvidenceBase hostEvidence)
		{
			evidence.AddHost (hostEvidence);
		}

		public static T GetHostEvidence<T>(this Evidence evidence) where T : EvidenceBase
		{
			return evidence.OfType<T>().FirstOrDefault();
		}
	}
}
	
#endif