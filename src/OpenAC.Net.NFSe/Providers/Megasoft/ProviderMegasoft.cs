using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers.Megasoft
{
    internal class ProviderMegasoft : ProviderABRASF202
    {
        public ProviderMegasoft(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Megasoft";
        }

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new MegasoftServiceCliente(this, tipo);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://megasoftarrecadanet.com.br/xsd/nfse_v01.xsd\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_v01.xsd";
        }

        
        protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (notas.Count > 3) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas 3 RPS podem ser enviados em modo Sincrono." });
            if (retornoWebservice.Erros.Count > 0) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append($"<GerarNfseEnvio {GetNamespace()}>");
            xmlLote.Append(xmlLoteRps);

            xmlLote.Append("</GerarNfseEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlConsulta = new StringBuilder();
            xmlConsulta.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlConsulta.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");
            xmlConsulta.Append("<IdentificacaoRps>");
            xmlConsulta.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");            
            xmlConsulta.Append("</IdentificacaoRps>");
            xmlConsulta.Append("<Prestador>");
            xmlConsulta.Append("<CpfCnpj>");
            switch (Configuracoes.PrestadorPadrao.CpfCnpj.Length)
            {
                case 11:
                    {
                        xmlConsulta.Append($"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cpf>");
                        break;
                    }
                case 14:
                    {
                        xmlConsulta.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cnpj>");
                        break;
                    }
            }

            xmlConsulta.Append("</CpfCnpj>");
            xmlConsulta.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlConsulta.Append("</Prestador>");
            xmlConsulta.Append("</ConsultarNfseRpsEnvio>");
            retornoWebservice.XmlEnvio = xmlConsulta.ToString();
        }

        protected override XElement WriteIdentificacaoRps(NotaServico nota)
        {
            var indRps = new XElement("IdentificacaoRps");

            indRps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));

            return indRps;
        }

        protected override XElement WriteRpsRps(NotaServico nota)
        {
            var rps = new XElement("Rps");

            rps.Add(WriteIdentificacaoRps(nota));

            rps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            
            return rps;
        }

        protected override XElement WriteRps(NotaServico nota)
        {
            var rootRps = new XElement("Rps");

            var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
            rootRps.Add(infServico);

            infServico.Add(WriteRpsRps(nota));

            infServico.AddChild(WriteServicosRps(nota));
            infServico.AddChild(WritePrestadorRps(nota));
            infServico.AddChild(WriteTomadorRps(nota));
            infServico.AddChild(WriteIntermediarioRps(nota));
            infServico.AddChild(WriteConstrucaoCivilRps(nota));

            return rootRps;
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);        
        }

        protected override XElement WriteTomadorRps(NotaServico nota)
        {
            var tomador = new XElement("Tomador");

            if (!nota.Tomador.CpfCnpj.IsEmpty())
            {
                var ideTomador = new XElement("IdentificacaoTomador");
                tomador.Add(ideTomador);

                var cpfCnpjTomador = new XElement("CpfCnpj");
                ideTomador.Add(cpfCnpjTomador);

                cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

                ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
                    Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
            }

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

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));                                
            }

            return tomador;
        }

        protected override XElement WriteServicosRps(NotaServico nota)
        {
            var servico = new XElement("Servico");

            servico.Add(WriteValoresRps(nota));

            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));

            return servico;
        }

        protected override string GerarCabecalho()
        {
            var cabecalho = new StringBuilder();            
            cabecalho.Append("<cabecalho versao=\"1.00\" xmlns=\"http://megasoftarrecadanet.com.br/xsd/nfse_v01.xsd\">");
            cabecalho.Append("<versaoDados>1.00</versaoDados>");
            cabecalho.Append("</cabecalho>");
            return cabecalho.ToString();
        }

        protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
        {
            var mensagens = xmlRet?.ElementAnyNs("ListaMensagemRetorno");
            if (mensagens != null)
            {
                foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
                {
                    var evento = new Evento
                    {
                        Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                        Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                        Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
                    };

                    retornoWs.Erros.Add(evento);
                }
            }
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno)?.ElementAnyNs("ConsultarNfseRpsResposta");
            MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
            if (retornoWebservice.Erros.Any()) return;

            var compNfse = xmlRet?.ElementAnyNs("CompNfse");

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
                .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                .ElementAnyNs("Rps")?
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
                nota.XmlOriginal = compNfse.ToString();

                var nfseCancelamento = compNfse.ElementAnyNs("NfseCancelamento");

                if (nfseCancelamento != null)
                {
                    nota.Situacao = SituacaoNFSeRps.Cancelado;

                    var confirmacaoCancelamento = nfseCancelamento
                        .ElementAnyNs("Confirmacao");

                    if (confirmacaoCancelamento != null)
                    {
                        var pedido = confirmacaoCancelamento.ElementAnyNs("Pedido");

                        if (pedido != null)
                        {
                            var codigoCancelamento = pedido
                                .ElementAnyNs("InfPedidoCancelamento")
                                .ElementAnyNs("CodigoCancelamento")
                                .GetValue<string>();

                            nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;

                            nota.Cancelamento.Signature = DFeSignature.Load(pedido.ElementAnyNs("Signature").ToString());
                        }
                    }

                    nota.Cancelamento.DataHora = confirmacaoCancelamento
                        .ElementAnyNs("DataHora")
                        .GetValue<DateTime>();
                }
            }

            retornoWebservice.Nota = nota;
            retornoWebservice.Sucesso = true;
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root, "GerarNfseResposta");
            if (retornoWebservice.Erros.Any()) return;

            var infNfse = xmlRet.ElementAnyNs("GerarNfseResposta")?.ElementAnyNs("ListaNfse")?.ElementAnyNs("CompNfse")?.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
            var numeroNFSe = infNfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = infNfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = infNfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = infNfse?.ElementAnyNs("DeclaracaoPrestacaoServico")?.ElementAnyNs("InfDeclaracaoPrestacaoServico")?.ElementAnyNs("Rps")?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(infNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota != null)
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            }

            retornoWebservice.Sucesso = true;
        }
    }
}
