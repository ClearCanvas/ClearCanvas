#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using System.IO;
using System.Reflection;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Upgrade;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Hibernate.UpgradeScripts
{
    public class BaseUpgradeScript : IPersistentStoreUpgradeScript
    {
		interface IUpgradeBroker : IPersistenceBroker
		{
			void ExecuteSql(string rawSqlText);
		}

		[ExtensionOf(typeof(BrokerExtensionPoint))]
		public class UpgradeBroker : Broker, IUpgradeBroker
		{
			public void ExecuteSql(string rawSqlText)
			{
				var reader = new StringReader(rawSqlText);

				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Length > 0 && !line.Trim().Equals("GO"))
					{
						try
						{
							using (var cmd = this.CreateSqlCommand(line))
							{
								cmd.ExecuteNonQuery();
							}
						}
						catch (Exception e)
						{
							Console.WriteLine("Unexpected error with line in upgrade script: {0}", line);
							Console.WriteLine("Error: {0}", e.Message);
							throw;
						}
					}
				}
			}
		}

        private readonly Version _upgradeFromVersion;
        private readonly Version _upgradeToVersion;
        private readonly string _scriptName;
        public BaseUpgradeScript(Version upgradeFromVersion, Version upgradeToVersion, string scriptName)
        {
            _upgradeToVersion = upgradeToVersion ?? Assembly.GetExecutingAssembly().GetName().Version;
            _upgradeFromVersion = upgradeFromVersion;
            _scriptName = scriptName;
        }
        public string GetScript()
        {
            string sql;

			using (var stream = GetType().Assembly.GetManifestResourceStream(GetType(), _scriptName))
            {
                if (stream == null)
                    throw new ApplicationException("Unable to load script resource (is the script an embedded resource?): " + _scriptName);

				var reader = new StreamReader(stream);
                sql = reader.ReadToEnd();
                stream.Close();
            }
            return sql;
        }

        public Version SourceVersion
        {
            get { return _upgradeFromVersion; }
        }

        public Version DestinationVersion
        {
            get { return _upgradeToVersion; }
        }

        public void Execute()
        {
            // Wrap the upgrade in a single commit.
			using (var updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
				// bug #8924: disable validation to avoid deadlocks
				((UpdateContext)updateContext).DisableValidation();

                // Update SQL
				var upgradeBroker = updateContext.GetBroker<IUpgradeBroker>();
				upgradeBroker.ExecuteSql(GetScript());

                // Update the db version string
				var broker = updateContext.GetBroker<IPersistentStoreVersionBroker>();

				var where = new PersistentStoreVersionSearchCriteria();

                // only 1 row in the database.
				var version = broker.FindOne(where);

                version.Revision = DestinationVersion.Revision.ToString();
                version.Build = DestinationVersion.Build.ToString();
                version.Minor = DestinationVersion.Minor.ToString();
                version.Major = DestinationVersion.Major.ToString();

                updateContext.Commit();
            }
        }


    }
}
