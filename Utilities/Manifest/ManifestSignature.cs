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

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    internal static class ManifestSignature
    {
       
        // Sign an XML file and save the signature in a new file.
        public static void SignXmlFile(XmlDocument doc, string signedFileName, RSA key)
        {
            // Check the arguments.  
            if (doc == null)
                throw new ArgumentNullException("doc");
            if (signedFileName == null)
                throw new ArgumentNullException("signedFileName");
            if (key == null)
                throw new ArgumentNullException("key");

            
            // Format the document to ignore white spaces.
            doc.PreserveWhitespace = false;
            
            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(doc)
                                      {
                                          // Add the key to the SignedXml document. 
                                          SigningKey = key
                                      };            


            // Create a reference to be signed.
            Reference reference = new Reference
                                      {
                                          Uri = string.Empty
                                      };

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Add an RSAKeyValue KeyInfo (optional; helps recipient find key to validate).
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new RSAKeyValue(key));
            signedXml.KeyInfo = keyInfo;

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            if (doc.DocumentElement != null)
                doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            // Save the signed XML document to a file specified
            // using the passed string.
            XmlTextWriter xmltw = new XmlTextWriter(signedFileName, new UTF8Encoding(false));
            doc.WriteTo(xmltw);
            xmltw.Close();
        }

        // Verify the signature of an XML file against an asymmetric 
        // algorithm and return the result.
        public static Boolean VerifyXmlSignature(XmlDocument doc)
        {
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            XmlElement keyNode = (XmlElement)CollectionUtils.FirstElement(doc.GetElementsByTagName("KeyValue"));

            // Use the key in the document to verify
            key.FromXmlString(keyNode.InnerXml);

            // Check arguments.
            if (doc == null)
                throw new ArgumentException("doc");
          
            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(doc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = doc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.
            return signedXml.CheckSignature(key);
        }
    }
}
