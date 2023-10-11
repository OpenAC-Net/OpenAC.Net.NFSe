using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers.Sigep
{
    internal class ProviderSigep : ProviderABRASF200
    {
        #region Constructors

        public ProviderSigep(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Sigep";
        }

        #endregion Constructors

        #region Methods

        #region Protected Methods

        #region RPS
        
        protected override XElement WriteRps(NotaServico nota)
        {
            var rootRps = new XElement("Rps");

            var infServico = new XElement("InfDeclaracaoPrestacaoServico");
            rootRps.Add(infServico);

            infServico.Add(WriteRpsRps(nota));

            infServico.AddChild(WriteServicosRps(nota));
            infServico.AddChild(WritePrestadorRps(nota));
            infServico.AddChild(WriteTomadorRps(nota));
            infServico.AddChild(WriteIntermediarioRps(nota));
            infServico.AddChild(WriteConstrucaoCivilRps(nota));

            return rootRps;
        }

        protected override XElement WriteRpsRps(NotaServico nota)
        {
            var rps = new XElement("Rps");

            rps.Add(WriteIdentificacaoRps(nota));

            string status = null;
            //Algumas prefeituras não permitem controle de série de RPS
            switch (nota.Situacao)
            {
                case SituacaoNFSeRps.Normal:
                    status = "CO";
                    break;

                case SituacaoNFSeRps.Cancelado:
                    status = "CA";
                    break;
            }

            rps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "Status", 1, 1, Ocorrencia.Obrigatoria, status));

            rps.AddChild(WriteSubstituidoRps(nota));

            return rps;
        }

        protected override XElement WriteIdentificacaoRps(NotaServico nota)
        {
            var indRps = new XElement("IdentificacaoRps");

            indRps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));

            var serie = nota.IdentificacaoRps.Serie;

            string tipo = null;
            //Algumas prefeituras não permitem controle de série de RPS
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipo = "R1";
                    break;

                case TipoRps.NFConjugada:
                    tipo = "R2";
                    break;
                case TipoRps.Cupom:
                    tipo = "R3";
                    break;
            }

            indRps.AddChild(AdicionarTag(TipoCampo.Str, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipo));

            return indRps;
        }

        protected override XElement WriteServicosRps(NotaServico nota)
        {
            var servico = new XElement("Servico");

            servico.Add(WriteValoresRps(nota));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

            var exigibilidadeISS = (int)nota.Servico.ExigibilidadeIss + 1;
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, "0"+ exigibilidadeISS));

            return servico;
        }

        protected override XElement WriteValoresRps(NotaServico nota)
        {
            var valores = new XElement("Valores");

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 6, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            return valores;
        }

        protected override XElement WriteTomadorRps(NotaServico nota)
        {
            if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

            var tomador = new XElement("Tomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "tsInscricaoEstadual", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
                !nota.Tomador.Endereco.Numero.IsEmpty() ||
                !nota.Tomador.Endereco.Complemento.IsEmpty() ||
                !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                nota.Tomador.Endereco.CodigoMunicipio > 0 ||
                !nota.Tomador.Endereco.Uf.IsEmpty() ||
                nota.Tomador.Endereco.CodigoPais > 0 ||
                !nota.Tomador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouro", 1, 50, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.TipoLogradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Logradouro", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, CorrelacaoCidadeGoianiaXCodigoIBGE.GetCodigoCidadeFromCodigoIBGE(nota.Tomador.Endereco.CodigoMunicipio.ToString())));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPaisEstrangeiro", 5, 5, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoPais));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "EstadoPaisEstrangeiro", 1, 60, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.EstadoPaisEstrangeiro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CidadePaisEstrangeiro", 1, 60, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CidadePaisEstrangeiro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            if (!nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                !nota.Tomador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("Contato");
                tomador.Add(contato);

                if (nota.Tomador.DadosContato.Telefone.Length > 8)
                    nota.Tomador.DadosContato.Telefone = nota.Tomador.DadosContato.Telefone.Substring(0, 8);

                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Ddd", 3, 3, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD.PadLeft(3, '0')));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
            }

            return tomador;
        }

        #endregion RPS

        #region Services

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Any()) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append($"<GerarNfseEnvio {GetNamespace()}>");
            xmlLote.Append($"<credenciais>");
            xmlLote.Append($"<usuario>{Configuracoes.WebServices.Usuario}</usuario>");
            xmlLote.Append($"<senha>{Configuracoes.WebServices.Senha}</senha>");
            xmlLote.Append($"<chavePrivada>{Configuracoes.WebServices.ChavePrivada}</chavePrivada>");
            xmlLote.Append($"</credenciais>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</GerarNfseEnvio>");

            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        /// <inheritdoc />
        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "", Certificado);
            //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "GerarNfseEnvio", "Rps", Certificado);
        }

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Any()) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
                xmlLoteRps.Append(xmlRps);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append($"<EnviarLoteRpsSincronoEnvio {GetNamespace()}>");
            xmlLote.Append($"<credenciais>");
            xmlLote.Append($"<usuario>{Configuracoes.WebServices.Usuario}</usuario>");
            xmlLote.Append($"<senha>{Configuracoes.WebServices.Senha}</senha>");
            xmlLote.Append($"<chavePrivada>{Configuracoes.WebServices.ChavePrivada}</chavePrivada>");
            xmlLote.Append($"</credenciais>");
            xmlLote.Append($"<LoteRps {GetVersao()}>");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            if (UsaPrestadorEnvio) xmlLote.Append("<Prestador>");
            xmlLote.Append("<CpfCnpj>");
            xmlLote.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
                ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
            xmlLote.Append("</CpfCnpj>");
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            if (UsaPrestadorEnvio) xmlLote.Append("</Prestador>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsSincronoEnvio>");

            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Rps", "", Certificado);
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlDocument = XElement.Parse(retornoWebservice.XmlRetorno);
            var reader = xmlDocument.ElementAnyNs("gerarNfseResponse").CreateReader();
            reader.MoveToContent();
            var xml = reader.ReadInnerXml().Replace("ns2:", string.Empty);

            XmlDocument XmlRetorno = new XmlDocument();
            XmlRetorno.LoadXml(xml);

            XmlDocument xmlMensagem = new XmlDocument();
            var xmlRet = XDocument.Parse(XmlRetorno.LastChild.InnerText);

            MensagemErro(retornoWebservice, xmlRet, "GerarNfseResposta");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root.ElementAnyNs("ListaNfse") != null;

            if (!retornoWebservice.Sucesso) return;

            retornoWebservice.Data = xmlRet.Root.ElementAnyNs("ListaNfse").ElementAnyNs("CompNfse").ElementAnyNs("Nfse").ElementAnyNs("InfNfse").ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet.Root.ElementAnyNs("ListaNfse").ElementAnyNs("CompNfse").ElementAnyNs("Nfse").ElementAnyNs("InfNfse").ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? "";

            var listaNfse = xmlRet.Root.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return;
            }

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
                var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                                    .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                                    .ElementAnyNs("Rps")?
                                    .ElementAnyNs("IdentificacaoRps")?
                                    .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

                GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    nota = notas.Load(compNfse.ToString());
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = chaveNFSe;
                }

                nota.Protocolo = retornoWebservice.Protocolo;
            }
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "AC0001", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
                return;
            }

            var loteBuilder = new StringBuilder();

            loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append($"<credenciais>");
            loteBuilder.Append($"<usuario>{Configuracoes.WebServices.Usuario}</usuario>");
            loteBuilder.Append($"<senha>{Configuracoes.WebServices.Senha}</senha>");
            loteBuilder.Append($"<chavePrivada>{Configuracoes.WebServices.ChavePrivada}</chavePrivada>");
            loteBuilder.Append($"</credenciais>");
            loteBuilder.Append("<Pedido>");
            loteBuilder.Append($"<InfPedidoCancelamento>");
            loteBuilder.Append("<IdentificacaoNfse>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
            loteBuilder.Append("<CpfCnpj>");
            loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
                ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CodigoVerificacao>{retornoWebservice.CodigoVerificacao}</CodigoVerificacao>");
            loteBuilder.Append("</IdentificacaoNfse>");
            loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
            loteBuilder.Append($"<DescricaoCancelamento>{retornoWebservice.Motivo}</DescricaoCancelamento>");
            loteBuilder.Append("</InfPedidoCancelamento>");
            loteBuilder.Append("</Pedido>");
            loteBuilder.Append("</CancelarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Pedido", "", Certificado);
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlDocument = XElement.Parse(retornoWebservice.XmlRetorno);
            var reader = xmlDocument.ElementAnyNs("cancelarNfseResponse").CreateReader();
            reader.MoveToContent();
            var xml = reader.ReadInnerXml().Replace("ns2:", string.Empty);

            XmlDocument XmlRetorno = new XmlDocument();
            XmlRetorno.LoadXml(xml);

            XmlDocument xmlMensagem = new XmlDocument();
            var xmlRet = XDocument.Parse(XmlRetorno.LastChild.InnerText);
            MensagemErro(retornoWebservice, xmlRet, "CancelarNfseResposta");
            if (retornoWebservice.Erros.Any()) return;

            var confirmacaoCancelamento = xmlRet.ElementAnyNs("CancelarNfseResposta")?.ElementAnyNs("RetCancelamento")?.ElementAnyNs("NfseCancelamento")?.ElementAnyNs("Confirmacao");
            if (confirmacaoCancelamento == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
                return;
            }

            retornoWebservice.Data = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;

            var numeroNFSe = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?
                                 .ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);
            if (nota == null) return;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
            nota.Cancelamento.DataHora = retornoWebservice.Data;
            nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");
            loteBuilder.Append($"<credenciais>");
            loteBuilder.Append($"<usuario>{Configuracoes.WebServices.Usuario}</usuario>");
            loteBuilder.Append($"<senha>{Configuracoes.WebServices.Senha}</senha>");
            loteBuilder.Append($"<chavePrivada>{Configuracoes.WebServices.ChavePrivada}</chavePrivada>");
            loteBuilder.Append($"</credenciais>");
            loteBuilder.Append("<IdentificacaoRps>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
            loteBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}</Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append("<CpfCnpj>");
            loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
                ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfseRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
        {
            var mensagens = xmlRet?.ElementAnyNs(xmlTag);
            mensagens = mensagens?.ElementAnyNs("ListaMensagemRetorno") ?? mensagens?.ElementAnyNs("ListaMensagemRetornoLote");
            if (mensagens == null) return;

            foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
            {
                var codigoRetorno = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>();

                if (!string.IsNullOrEmpty(codigoRetorno) && codigoRetorno == "L000") //Emitido com Sucesso
                    return;

                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                    Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
                };

                retornoWs.Erros.Add(evento);
            }
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
            if (retornoWebservice.Erros.Any()) return;

            var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");

            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
                return;
            }

            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                .ElementAnyNs("IdentificacaoRps")?
                .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);

            if (nota == null)
            {
                nota = notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            }

            retornoWebservice.Nota = nota;
        }

        #endregion Services

        protected override IServiceClient GetClient(TipoUrl tipo) => new SigepServiceClient(this, tipo, Certificado);

        protected override string GetSchema(TipoUrl tipo) => "nfse-v2.xsd";

        #endregion Protected Methods

        #endregion Methods
    }
}
