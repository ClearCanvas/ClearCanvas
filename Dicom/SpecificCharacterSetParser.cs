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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Default Specific Character Set parser for the DICOM library
    /// </summary>
    public class SpecificCharacterSetParser : IDicomCharacterSetParser
    {
        private class Last
        {
            public Last(string specificCharacterSet, CharacterSetInfo defaultRepertoire, 
                Dictionary<string, CharacterSetInfo> extensionRepertoires)
            {
                SpecificCharacterSet = specificCharacterSet;
                DefaultRepertoire = defaultRepertoire;
                ExtensionRepertoires = extensionRepertoires;
            }

            public readonly string SpecificCharacterSet;
            public readonly CharacterSetInfo DefaultRepertoire;
            public readonly Dictionary<string, CharacterSetInfo> ExtensionRepertoires;
        }

        [ThreadStatic]
        private static Last _last;
		[ThreadStatic]
		private static Encoding _isomorphicEncoding;

        #region IDicomCharacterSetParser Members

        public byte[] Encode(string dataInUnicode, string specificCharacterSet)
        {
            byte[] rawBytes;
            Unparse(specificCharacterSet, dataInUnicode, out rawBytes);
            return rawBytes;
        }

        public string EncodeAsIsomorphicString(string dataInUnicode, string specificCharacterSet)
        {
            return Unparse(specificCharacterSet, dataInUnicode);
        }

        public string Decode(byte[] rawData, string specificCharacterSet)
        {
            return Parse(specificCharacterSet, rawData);
        }

        public string DecodeFromIsomorphicString(string repertoireStringAsUnicode, string specificCharacterSet)
        {
            return Parse(specificCharacterSet, repertoireStringAsUnicode);
        }

        public string ConvertRawToIsomorphicString(byte[] value)
        {
            return GetIsomorphicString(value);
        }

        public bool IsVRRelevant(string vr)
        {
            return DoesSpecificCharacterSetApplyToThisVR(vr);
        }

        #endregion

        #region Static Methods
        static SpecificCharacterSetParser()
        {
            _characterSetInfo = new Dictionary<string, CharacterSetInfo>();
            _characterSetInfo.Add("ISO_IR 100", new CharacterSetInfo("ISO_IR 100", 28591, "", "", "Latin Alphabet No. 1 Unextended"));
            _characterSetInfo.Add("ISO_IR 101", new CharacterSetInfo("ISO_IR 101", 28592, "", "", "Latin Alphabet No. 2 Unextended"));
            _characterSetInfo.Add("ISO_IR 109", new CharacterSetInfo("ISO_IR 109", 28593, "", "", "Latin Alphabet No. 3 Unextended"));
            _characterSetInfo.Add("ISO_IR 110", new CharacterSetInfo("ISO_IR 110", 28594, "", "", "Latin Alphabet No. 4 Unextended"));
            _characterSetInfo.Add("ISO_IR 144", new CharacterSetInfo("ISO_IR 144", 28595, "", "", "Cyrillic Unextended"));
            _characterSetInfo.Add("ISO_IR 127", new CharacterSetInfo("ISO_IR 127", 28596, "", "", "Arabic Unextended"));
            _characterSetInfo.Add("ISO_IR 126", new CharacterSetInfo("ISO_IR 126", 28597, "", "", "Greek Unextended"));
            _characterSetInfo.Add("ISO_IR 138", new CharacterSetInfo("ISO_IR 138", 28598, "", "", "Hebrew Unextended"));
            _characterSetInfo.Add("ISO_IR 148", new CharacterSetInfo("ISO_IR 148", 28599, "", "", "Latin Alphabet No. 5 (Turkish) Unextended"));
            _characterSetInfo.Add("ISO_IR 13", new CharacterSetInfo("ISO_IR 13", 932, "", "", "JIS X 0201 (Shift JIS) Unextended"));
            _characterSetInfo.Add("ISO_IR 166", new CharacterSetInfo("ISO_IR 166", 874, "", "", "TIS 620-2533 (Thai) Unextended"));
            _characterSetInfo.Add("ISO_IR 192", new CharacterSetInfo("ISO_IR 192", 65001, "", "", "Unicode in UTF-8"));
            _characterSetInfo.Add("ISO 2022 IR 6", new CharacterSetInfo("ISO 2022 IR 6", 28591, "\x1b\x28\x42", "", "Default"));
            _characterSetInfo.Add("ISO 2022 IR 100", new CharacterSetInfo("ISO 2022 IR 100", 28591, "\x1b\x28\x42", "\x1b\x2d\x41", "Latin Alphabet No. 1 Extended"));
            _characterSetInfo.Add("ISO 2022 IR 101", new CharacterSetInfo("ISO 2022 IR 101", 28592, "\x1b\x28\x42", "\x1b\x2d\x42", "Latin Alphabet No. 2 Extended"));
            _characterSetInfo.Add("ISO 2022 IR 109", new CharacterSetInfo("ISO 2022 IR 109", 28593, "\x1b\x28\x42", "\x1b\x2d\x43", "Latin Alphabet No. 3 Extended"));
            _characterSetInfo.Add("ISO 2022 IR 110", new CharacterSetInfo("ISO 2022 IR 110", 28594, "\x1b\x28\x42", "\x1b\x2d\x44", "Latin Alphabet No. 4 Extended"));
            _characterSetInfo.Add("ISO 2022 IR 144", new CharacterSetInfo("ISO 2022 IR 144", 28595, "\x1b\x28\x42", "\x1b\x2d\x4c", "Cyrillic Extended"));
            _characterSetInfo.Add("ISO 2022 IR 127", new CharacterSetInfo("ISO 2022 IR 127", 28596, "\x1b\x28\x42", "\x1b\x2d\x47", "Arabic Extended"));
            _characterSetInfo.Add("ISO 2022 IR 126", new CharacterSetInfo("ISO 2022 IR 126", 28597, "\x1b\x28\x42", "\x1b\x2d\x46", "Greek Extended"));
            _characterSetInfo.Add("ISO 2022 IR 138", new CharacterSetInfo("ISO 2022 IR 138", 28598, "\x1b\x28\x42", "\x1b\x2d\x48", "Hebrew Extended"));
            _characterSetInfo.Add("ISO 2022 IR 148", new CharacterSetInfo("ISO 2022 IR 148", 28599, "\x1b\x28\x42", "\x1b\x2d\x4d", "Latin Alphabet No. 5 (Turkish) Extended"));
            _characterSetInfo.Add("ISO 2022 IR 13", new CharacterSetInfo("ISO 2022 IR 13", 50222, "\x1b\x28\x4a", "\x1b\x29\x49", "JIS X 0201 (Shift JIS) Extended"));
            _characterSetInfo.Add("ISO 2022 IR 166", new CharacterSetInfo("ISO 2022 IR 166", 874, "\x1b\x28\x42", "\x1b\x2d\x54", "TIS 620-2533 (Thai) Extended"));
            _characterSetInfo.Add("ISO 2022 IR 87", new CharacterSetInfo("ISO 2022 IR 87", 50222, "\x1b\x24\x42", "", "JIS X 0208 (Kanji) Extended"));
            _characterSetInfo.Add("ISO 2022 IR 159", new CharacterSetInfo("ISO 2022 IR 159", 50222, "\x1b\x24\x28\x44", "", "JIS X 0212 (Kanji) Extended"));
            _characterSetInfo.Add("ISO 2022 IR 149", new CharacterSetInfo("ISO 2022 IR 149", 20949, "", "\x1b\x24\x29\x43", "KS X 1001 (Hangul and Hanja) Extended"));
            _characterSetInfo.Add("GB18030", new CharacterSetInfo("GB18030", 54936, "", "", "Chinese (Simplified) Extended"));

            _repertoireAppliesVRDictionary = new Dictionary<string, bool>();
            _repertoireAppliesVRDictionary.Add("EVR_SH", true);
            _repertoireAppliesVRDictionary.Add("EVR_LO", true);
            _repertoireAppliesVRDictionary.Add("EVR_ST", true);
            _repertoireAppliesVRDictionary.Add("EVR_LT", true);
            _repertoireAppliesVRDictionary.Add("EVR_PN", true);
            _repertoireAppliesVRDictionary.Add("EVR_UT", true);
            _repertoireAppliesVRDictionary.Add("SH", true);
            _repertoireAppliesVRDictionary.Add("LO", true);
            _repertoireAppliesVRDictionary.Add("ST", true);
            _repertoireAppliesVRDictionary.Add("LT", true);
            _repertoireAppliesVRDictionary.Add("PN", true);
            _repertoireAppliesVRDictionary.Add("UT", true);

        }

        // What does "isomorphic code page" mean? And why use the Arabic code page (Windows-1256) as 
        // isomorphic code page?
        //
        // The use of the isomorphic code page is in enabling us to store DICOM strings in Unicode form,
        // for example, storing the Patient's Name tag data in a SQL Server 2005 database, so that 
        // the tag data can be, e.g. conveyed unaltered to a C-FIND query SCU. Note that the goal isn't 
        // to convert the DICOM string into a Unicode string, i.e. not to convert a string of double-byte
        // characters into Unicode Kanji characters; the goal is to represent the double-byte characters
        // in Unicode so that it can be stored or transmitted as if we were dealing with the original 
        // DICOM source data. For example, if the DICOM string contains an escape sequence, the 
        // escape sequence should be preserved in the Unicode string.
        // 
        // In order to accomplish this, an appropriate code page must be used. How does the code page 
        // fit into this? To convert to and from DICOM string data and Unicode data, you must specify to
        // the converter which 'code page' the DICOM data is or is to be represented in, since there is, 
        // and there will not be anything inherent in the string that conveys encoding information.
        //
        // Why must the appropriate code page be used? A code page basically serves as a mapping from
        // single bytes to characters (in a logical sense). For example, the byte \x1a represents the
        // Escape character. The problem arises, however, when for certain code pages, certain bytes
        // have no mapping to characters. Thus, when this code page is used to convert DICOM data 
        // to Unicode data, if the converter runs into these problem bytes, it cannot represent the
        // byte as a Unicode character (and will represent it as a question mark '?').
        //
        // This applies in the reverse direction as well. Which finally brings us to Windows-1256. 
        // As it turns out, the Arabic code page is the only one for which every byte value (00 to ff)
        // has a character representation in Unicode. Therefore, I call it the Isomorphic Code Page:
        // it allows characters to be encoded in both directions and still 'look' the same.
        const string IsomorphicCodePage = "Windows-1256";

        public static bool DoesSpecificCharacterSetApplyToThisVR(string vr)
        {
            return _repertoireAppliesVRDictionary.ContainsKey(vr);
        }

        public static string GetIsomorphicString(byte[] rawBytes)
        {
			return IsomorphicEncoding.GetString(rawBytes);
        }

        public static byte[] GetIsomorphicBytes(string rawBytesEncodedAsString)
        {
            // add a null terminator, otherwise we're going to have problems in the unamanged world
            if (rawBytesEncodedAsString == null)
                return null;

			return IsomorphicEncoding.GetBytes(rawBytesEncodedAsString);
        }

        public static Encoding GetEncoding(string specificCharacterSet)
        {
            CharacterSetInfo defaultRepertoire;
            Dictionary<string, CharacterSetInfo> extensionRepertoires;
            GetRepertoires(specificCharacterSet, out defaultRepertoire, out extensionRepertoires);

            return Encoding.GetEncoding(defaultRepertoire.MicrosoftCodePage);
        }

        public static string Unparse(string specificCharacterSet, string dataInUnicode)
        {
            if (null == specificCharacterSet || "" == specificCharacterSet)
                return dataInUnicode;

            CharacterSetInfo defaultRepertoire;
            Dictionary<string, CharacterSetInfo> extensionRepertoires;
            GetRepertoires(specificCharacterSet, out defaultRepertoire, out extensionRepertoires);

            // TODO: here's where the hack starts
            // pick the first one and use that for decoding
            foreach (CharacterSetInfo info in extensionRepertoires.Values)
            {
                return Encode(dataInUnicode, info);
            }

            // if nothing happened with extension repertoires, use default repertoire
            if (null != defaultRepertoire)
            {
                return Encode(dataInUnicode, defaultRepertoire);
            }
            else
            {
                return dataInUnicode;
            }
        }

        public static void Unparse(string specificCharacterSet, string dataInUnicode, out byte[] rawBytes)
        {
            if (null == specificCharacterSet || "" == specificCharacterSet)
            {
                rawBytes = GetIsomorphicBytes(dataInUnicode);
                return;
            }

            CharacterSetInfo defaultRepertoire;
            Dictionary<string, CharacterSetInfo> extensionRepertoires;
            GetRepertoires(specificCharacterSet, out defaultRepertoire, out extensionRepertoires);

            // TODO: here's where the hack starts
            // pick the first one and use that for decoding
            foreach (CharacterSetInfo info in extensionRepertoires.Values)
            {
                Encode(dataInUnicode, info, out rawBytes);
                return;
            }

            // if nothing happened with extension repertoires, use default repertoire
            if (null != defaultRepertoire)
            {
                Encode(dataInUnicode, defaultRepertoire, out rawBytes);
                return;
            }
            else
            {
                rawBytes = GetIsomorphicBytes(dataInUnicode);
                return;
            }
        }

        public static string Parse(string specificCharacterSet, byte[] rawData)
        {
            if (null == specificCharacterSet || "" == specificCharacterSet)
            {
                // this takes the raw bytes, and converts it into a Unicode string
                // represention of the original raw bytes
                return GetIsomorphicString(rawData);
            }

            CharacterSetInfo defaultRepertoire;
            Dictionary<string, CharacterSetInfo> extensionRepertoires;
            GetRepertoires(specificCharacterSet, out defaultRepertoire, out extensionRepertoires);

            // TODO: here's where the hack starts
            // pick the first one and use that for decoding
            foreach (CharacterSetInfo info in extensionRepertoires.Values)
            {
                return Decode(rawData, info);
            }

            // if nothing happened with extension repertoires, use default repertoire
            if (null != defaultRepertoire)
            {
                return Decode(rawData, defaultRepertoire);
            }
            else
            {
                // this takes the raw bytes, and converts it into a Unicode string
                // represention of the original raw bytes
                return GetIsomorphicString(rawData);

            }
        }

        public static string Parse(string specificCharacterSet, string rawData)
        {
            if (null == specificCharacterSet || "" == specificCharacterSet)
                return rawData;

            CharacterSetInfo defaultRepertoire;
            Dictionary<string, CharacterSetInfo> extensionRepertoires;
            GetRepertoires(specificCharacterSet, out defaultRepertoire, out extensionRepertoires);

            // TODO: here's where the hack starts
            // pick the first one and use that for decoding
            foreach (CharacterSetInfo info in extensionRepertoires.Values)
            {
                return Decode(rawData, info);
            }

            // if nothing happened with extension repertoires, use default repertoire
            if (null != defaultRepertoire)
            {
                return Decode(rawData, defaultRepertoire);
            }
            else
            {
                return rawData;
            }
        }

        /// <summary>
        /// Gets the description of the specific character sets
        /// </summary>
        /// <param name="specificCharacterSet"></param>
        /// <returns></returns>
        public static string[] GetCharacterSetInfoDescriptions(string specificCharacterSet)
        {
            CharacterSetInfo cs;
                
            if (string.IsNullOrEmpty(specificCharacterSet))
            {
                if (_characterSetInfo.TryGetValue(DefaultCharacterSet, out cs))
                    return new[]{ cs.Description };
                else
                    return null;
            }

            string[] specificCharacterSetValues = specificCharacterSet.Split('\\');
            List<string> descriptions = new List<string>();
            foreach(var value in specificCharacterSetValues)
            {
                if (_characterSetInfo.TryGetValue(value, out cs))
                    descriptions.Add(cs.Description);
            }

            return descriptions.ToArray();
        }

        private static void GetRepertoires(string specificCharacterSet, out CharacterSetInfo defaultRepertoire, out Dictionary<string, CharacterSetInfo> extensionRepertoires)
        {
            //Most of the time, especially on the same thread, the specific character set will be the same.
            //This simple check avoids having to figure it out over and over again, which gets expensive.
            var last = _last;
            if (last != null && specificCharacterSet == last.SpecificCharacterSet)
            {
                defaultRepertoire = last.DefaultRepertoire;
                extensionRepertoires = last.ExtensionRepertoires;
                return;
            }

            // TODO:
            // Specific Character Set may have up to n values if 
            // Code Extensions are used. We accomodate for that here
            // by parsing out all the different possible defined terms.
            // At this point, however, we're not going to handle escaping
            // between character sets from different code pages within
            // a single string. For example, DICOM implies that you should
            // be able to have JIS-encoded Japanese, ISO European characters,
            // Thai characters and Korean characters on the same line, using
            // Code Extensions (escape sequences). (Chinese is not included
            // since the only support for Chinese is through GB18030 and
            // UTF-8, both of which do not support Code Extensions.)
            string[] specificCharacterSetValues = specificCharacterSet.Split('\\');
            defaultRepertoire = null;

            // set the default repertoire from Value 1 
            if (specificCharacterSetValues.GetUpperBound(0) >= 0)
            {
                if (!CharacterSetDatabase.TryGetValue(specificCharacterSetValues[0], out defaultRepertoire))
                    // we put in the default repertoire. Technically, it may
                    // not be ISO 2022 IR 6, but ISO_IR 6, but the information
                    // we want to use is the same
                    defaultRepertoire = CharacterSetDatabase["ISO 2022 IR 6"];
            }

            // Here we are accounting for cases where the same character sets are repeated, so
            // we need to select out the unique ones.  It should never really happen, but it 
            // does happen with a particular dataset when querying JDicom.
            List<string> uniqueExtensionRepertoireDefinedTerms = new List<string>();
            for (int i = 1; i < specificCharacterSetValues.Length; ++i)
            {
                string value = specificCharacterSetValues[i];
                if (value != defaultRepertoire.DefinedTerm && !uniqueExtensionRepertoireDefinedTerms.Contains(value))
                    uniqueExtensionRepertoireDefinedTerms.Add(value);
            }

            // parse out the extension repertoires
            extensionRepertoires = new Dictionary<string, CharacterSetInfo>();
            foreach (string value in uniqueExtensionRepertoireDefinedTerms)
            {
                if (CharacterSetDatabase.ContainsKey(value) && !extensionRepertoires.ContainsKey(value))
                {
                    // special robustness handling of GB18030 and UTF-8
                    if ("GB18030" == value || "ISO_IR 192" == value)
                    {
                        // these two character sets can't use code extensions, so there should really only be 1
                        // character set in the repertoire
                        extensionRepertoires.Clear();
                        extensionRepertoires.Add(value, CharacterSetDatabase[value]);
                        break;
                    }

                    extensionRepertoires.Add(value, CharacterSetDatabase[value]);
                }
                else if (!extensionRepertoires.ContainsKey("ISO 2022 IR 6"))
                {
                    // we put in the default repertoire. Technically, it may
                    // not be ISO 2022 IR 6, but ISO_IR 6, but the information
                    // we want to use is the same
                    extensionRepertoires.Add(value, CharacterSetDatabase["ISO 2022 IR 6"]);
                }
            }

            _last = new Last(specificCharacterSet, defaultRepertoire, extensionRepertoires);
        }

        /// <summary>
        /// Changes fully translated Unicode data, e.g. with Japanaese Kanji characters, into
        /// a string of characters that represents the raw, ISO character repertoire representation, i.e.
        /// with escape sequences, but encoded as a Unicode string
        /// </summary>
        /// <param name="unicodeData">Fully translated Unicode data</param>
        /// <param name="repertoire">Target repertoire to be transformed into</param>
        /// <returns></returns>
        private static string Encode(string unicodeData, CharacterSetInfo repertoire)
        {
            byte[] rawBytes;
            Encode(unicodeData, repertoire, out rawBytes);
			char[] rawCharacters = IsomorphicEncoding.GetChars(rawBytes);
            return new string(rawCharacters);
        }

        /// <summary>
        /// Changes fully translated Unicode data, e.g. with Japanese Kanji characters, into
        /// a raw byte array containing the 8-bit representation in the target repertoire
        /// </summary>
        /// <param name="unicodeData">Fully translated Unicode data</param>
        /// <param name="repertoire">Target repertoire to be transformed into</param>
        /// <param name="encoded">Output: byte array to hold the results</param>
        private static void Encode(string unicodeData, CharacterSetInfo repertoire, out byte[] encoded)
        {
            byte[] rawBytes = Encoding.GetEncoding(repertoire.MicrosoftCodePage).GetBytes(unicodeData);
            encoded = rawBytes;
        }

        /// <summary>
        /// Takes a string that is a representation of the raw sequence of bytes 
        /// encoded using an ISO repertoire, but current encoded as a Unicode string
        /// and gives back a true Unicode string, e.g. containing the Japanese Kanji 
        /// characters
        /// </summary>
        /// <param name="rawData">Sequence of bytes formatted in Unicode</param>
        /// <param name="repertoire">Original ISO repertoire used in the encoding</param>
        /// <returns>True Unicode string</returns>
        private static string Decode(string rawData, CharacterSetInfo repertoire)
        {
            // get it back to byte array form using a character set that includes 
            // both GR and GL areas (characters up to \xff in binary value)
            // and it seems Windows-1252 works better than ISO-8859-1
			byte[] rawBytes = IsomorphicEncoding.GetBytes(rawData);
            return Decode(rawBytes, repertoire);
        }

        /// <summary>
        /// Takes a string that is encoded using the ISO repertoires, as a raw sequence
        /// of bytes, and then gives back a fully translated Unicode representation, e.g.
        /// with the correct Japanese Kanji characters in Unicode
        /// </summary>
        /// <param name="rawData">Byte sequence encoded using ISO repertoire</param>
        /// <param name="repertoire">Repertoire the byte sequence is encoded using</param>
        /// <returns>Unicode string</returns>
        private static string Decode(byte[] rawData, CharacterSetInfo repertoire)
        {
            Encoding rawEncoding = Encoding.GetEncoding(repertoire.MicrosoftCodePage);
            string rawDataDecoded = new string(rawEncoding.GetChars(rawData));

            // get rid of any escape sequences, if they appear in the decoded string,
            // like the case of Korean, using code page 20949 for some reason
            if ("" != repertoire.G1Sequence)
                return rawDataDecoded.Replace(repertoire.G1Sequence, "");
            else
                return rawDataDecoded;
        }

        #endregion

        protected class CharacterSetInfo
        {
            public CharacterSetInfo(string definedTerm, int codePage, string g0Sequence, string g1Sequence, string description)
            {
                _definedTerm = definedTerm;
                _microsoftCodePage = codePage;
                _g0Sequence = g0Sequence;
                _g1Sequence = g1Sequence;
                _description = description;
            }


            #region Properties
            private string _definedTerm;
            private int _microsoftCodePage;
            private string _description;
            private string _g0Sequence;
            private string _g1Sequence;

            public string G1Sequence
            {
                get { return _g1Sequence; }
                set { _g1Sequence = value; }
            }

            public string G0Sequence
            {
                get { return _g0Sequence; }
                set { _g0Sequence = value; }
            }

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }


            public int MicrosoftCodePage
            {
                get { return _microsoftCodePage; }
                set { _microsoftCodePage = value; }
            }

            public string DefinedTerm
            {
                get { return _definedTerm; }
                set { _definedTerm = value; }
            }

            #endregion
        }

        #region Properties
        // private string _specificCharacterSet;
        //public string SpecificCharacterSet
        //{
        //    get { return _specificCharacterSet; }
        //    set { _specificCharacterSet = value; }
        //}	

        protected static Dictionary<string, CharacterSetInfo> CharacterSetDatabase
        {
            get { return SpecificCharacterSetParser._characterSetInfo; }
        }

        protected static string DefaultCharacterSet
        {
            get { return "ISO 2022 IR 6"; }    // this is the default    
        }

        #endregion

        #region Private fields
        private static Dictionary<string, CharacterSetInfo> _characterSetInfo;
        private static Dictionary<string, bool> _repertoireAppliesVRDictionary;
        #endregion

	    private static Encoding IsomorphicEncoding
	    {
		    get { return _isomorphicEncoding ?? (_isomorphicEncoding = Encoding.GetEncoding(IsomorphicCodePage)); }
	    }
    }
    
}
