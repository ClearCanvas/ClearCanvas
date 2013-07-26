namespace ClearCanvas.Enterprise.Core.Web
{
	public interface IPersistenceEnabledController
	{
		PersistenceScope PersistenceScope { get; set; }

		void PreCommit();
		void PostCommit();
	}
}