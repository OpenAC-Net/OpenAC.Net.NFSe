// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 21-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 21-01-2020
// ***********************************************************************
// <copyright file="ProviderSmarAPDABRASF.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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

using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSmarAPD204 : ProviderABRASF204
{
    #region Constructors

    public ProviderSmarAPD204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "SmarAPD";
    }

    #endregion Constructors

    #region Methods

    #region Protected Methods

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new SmarAPD204ServiceClient(this, tipo, Certificado);
    }

    protected override void LoadRps(NotaServico nota, XElement rpsRoot)
    {
        base.LoadRps(nota, rpsRoot);

        var rps = rpsRoot.ElementAnyNs("Rps");
        if (rps != null)
        {
            var situacao = rps.ElementAnyNs("Status")?.GetValue<string>();

            //SmarAPD, a situação para cancelamento é 2
            if (situacao == "1")
                nota.Situacao = SituacaoNFSeRps.Normal;
            else
                nota.Situacao = SituacaoNFSeRps.Cancelado;
        }
    }

    #endregion Protected Methods

    #endregion Methods
}