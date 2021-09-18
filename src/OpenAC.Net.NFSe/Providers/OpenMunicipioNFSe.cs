// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-19-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-03-2017
// ***********************************************************************
// <copyright file="OpenMunicipioNFSe.cs" company="OpenAC .Net">
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
using System.IO;
using System.Runtime.Serialization;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.Providers
{
    [Serializable]
    [DataContract(Name = "Municipio", Namespace = "")]
    public sealed class OpenMunicipioNFSe : ICloneable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenMunicipioNFSe"/> class.
        /// </summary>
        public OpenMunicipioNFSe()
        {
            UrlHomologacao = new NFSeUrlDictionary(10)
            {
                { TipoUrl.Enviar, string.Empty },
                { TipoUrl.EnviarSincrono, string.Empty },
                { TipoUrl.CancelarNFSe, string.Empty },
                { TipoUrl.CancelarNFSeLote, string.Empty },
                { TipoUrl.ConsultarNFSe, string.Empty },
                { TipoUrl.ConsultarNFSeRps, string.Empty },
                { TipoUrl.ConsultarLoteRps, string.Empty },
                { TipoUrl.ConsultarSituacao, string.Empty },
                { TipoUrl.ConsultarSequencialRps, string.Empty },
                { TipoUrl.SubstituirNFSe, string.Empty},
                { TipoUrl.Autenticacao, string.Empty}
            };

            UrlProducao = new NFSeUrlDictionary(10)
            {
                { TipoUrl.Enviar, string.Empty },
                { TipoUrl.EnviarSincrono, string.Empty },
                { TipoUrl.CancelarNFSe, string.Empty },
                { TipoUrl.CancelarNFSeLote, string.Empty },
                { TipoUrl.ConsultarNFSe, string.Empty },
                { TipoUrl.ConsultarNFSeRps, string.Empty },
                { TipoUrl.ConsultarLoteRps, string.Empty },
                { TipoUrl.ConsultarSituacao, string.Empty },
                { TipoUrl.ConsultarSequencialRps, string.Empty },
                { TipoUrl.SubstituirNFSe, string.Empty },
                { TipoUrl.Autenticacao, string.Empty }
            };
        }

        #endregion Constructors

        #region Propriedades

        /// <summary>
        /// Define ou retorna o codigo IBGE do municipio
        /// </summary>
        /// <value>The codigo.</value>
        [DataMember]
        public int Codigo { get; set; }

        /// <summary>
        /// Define ou retorna o codigo Siafi do municipio
        /// Obrigatorio para municipios com provedor DSF.
        /// </summary>
        /// <value>The codigo siafi.</value>
        [DataMember]
        public int CodigoSiafi { get; set; }

        /// <summary>
        /// Define ou retorna o identificador do município no provedor Equiplano
        /// </summary>
        /// <value>The Id Entidade.</value>
        [DataMember]
        public int IdEntidade { get; set; }

        /// <summary>
        /// Define ou retorna o nome do municipio
        /// </summary>
        /// <value>The nome.</value>
        [DataMember]
        public string Nome { get; set; }

        /// <summary>
        /// Define ou retorna a UF do municipio.
        /// </summary>
        /// <value>The uf.</value>
        [DataMember]
        public DFeSiglaUF UF { get; set; }

        /// <summary>
        /// Define ou retorna o provedor de NFSe.
        /// </summary>
        /// <value>The provedor.</value>
        [DataMember]
        public NFSeProvider Provedor { get; set; }

        /// <summary>
        /// Define ou retorna o tamanho da inscrição municipal
        /// Para validação em alguns provedores
        /// </summary>
        /// <value>The tamanho im.</value>
        [DataMember]
        public int TamanhoIm { get; set; }

        /// <summary>
        /// Lista de url de homologação dos serviços.
        /// </summary>
        /// <value>The URL homologacao.</value>
        [DataMember]
        public NFSeUrlDictionary UrlHomologacao { get; set; }

        /// <summary>
        /// Lista de url de produção dos serviços.
        /// </summary>
        /// <value>The URL producao.</value>
        [DataMember]
        public NFSeUrlDictionary UrlProducao { get; set; }

        #endregion Propriedades

        #region Methods

        /// <summary>
        /// Cria um novo objeto que é uma copia da instancia atual.
        /// </summary>
        /// <returns>T.</returns>
        public OpenMunicipioNFSe Clone()
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new DataContractSerializer(typeof(OpenMunicipioNFSe));
                formatter.WriteObject(memoryStream, this);
                memoryStream.Position = 0;
                return (OpenMunicipioNFSe)formatter.ReadObject(memoryStream);
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion Methods
    }
}