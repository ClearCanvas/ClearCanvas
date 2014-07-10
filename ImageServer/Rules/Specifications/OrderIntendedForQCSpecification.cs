using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules.Specifications;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.Specifications
{
	/// <summary>
	/// Specification that will be satisfied if the study has an order and it is intended for QC.
	/// </summary>
	class OrderIntendedForQCSpecification : PrimitiveSpecification
	{
		
		protected override TestResult InnerTest(object data, object root)
		{
			var study = Study.Load(ServerExecutionContext.Current.PrimaryStudyKey);

			if (study!=null)
			{
				if (study.OrderKey != null)
				{
					var order = Order.Load(study.OrderKey);
					if (order.QCExpected)
						return DefaultTestResult(true);
				}
			}

			Platform.Log("QC", LogLevel.Info, "Study does not have an order or the order is not intended for QC.");
			return DefaultTestResult(false);
		}
	}

	[ExtensionOf(typeof (DicomRuleSpecificationCompilerOperatorExtensionPoint))]
	class OrderIntendedForQCSpecificationOperator : IXmlSpecificationCompilerOperator
	{
		public string OperatorTag { get { return "order-intended-for-qc"; } }

		public Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context)
		{
			return new OrderIntendedForQCSpecification();
		}

		public XmlSchemaElement GetSchema()
		{
			var type = new XmlSchemaComplexType();

			return new XmlSchemaElement
			{
				Name = OperatorTag,
				SchemaType = type
			};
		}
	}
}
