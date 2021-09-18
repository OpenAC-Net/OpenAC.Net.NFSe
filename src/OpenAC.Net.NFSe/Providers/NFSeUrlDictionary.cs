// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-19-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-03-2017
// ***********************************************************************
// <copyright file="NFSeUrlDictionary.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenAC.Net.NFSe.Providers
{
    [Serializable]
    public sealed class NFSeUrlDictionary : Dictionary<TipoUrl, string>, IXmlSerializable
    {
        #region Constructors

        public NFSeUrlDictionary() : base()
        {
        }

        public NFSeUrlDictionary(int capacity) : base(capacity)
        {
        }

        public NFSeUrlDictionary(IEqualityComparer<TipoUrl> comparer) : base(comparer)
        {
        }

        public NFSeUrlDictionary(int capacity, IEqualityComparer<TipoUrl> comparer) : base(capacity, comparer)
        {
        }

        public NFSeUrlDictionary(IDictionary<TipoUrl, string> dictionary) : base(dictionary)
        {
        }

        public NFSeUrlDictionary(IDictionary<TipoUrl, string> dictionary, IEqualityComparer<TipoUrl> comparer) : base(dictionary, comparer)
        {
        }

        #endregion Constructors

        #region Methods

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;

            reader.Read();

            if (wasEmpty) return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Item");

                var key = (TipoUrl)Enum.Parse(typeof(TipoUrl), reader.ReadElementString("TipoUrl"));
                var value = reader.ReadElementString("Url");

                Add(key, value);

                reader.ReadEndElement();

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in Keys)
            {
                writer.WriteStartElement("Item");

                writer.WriteStartElement("TipoUrl");

                writer.WriteString(key.ToString());

                writer.WriteEndElement();

                writer.WriteStartElement("Url");

                var value = this[key];

                writer.WriteString(value);

                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion Methods
    }
}