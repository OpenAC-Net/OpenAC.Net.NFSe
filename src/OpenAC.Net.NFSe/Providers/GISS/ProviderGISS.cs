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

namespace OpenAC.Net.NFSe.Providers.GISS
{
    internal class ProviderGISS : ProviderABRASF203
    {
        #region Constructors

        public ProviderGISS(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "GISS";
            Versao = VersaoNFSe.ve204;
            UsaPrestadorEnvio = true;
        }

        #endregion Constructors

        #region Methods

        #region Protected Methods

        #region RPS
        
        protected override XElement WriteTomadorRps(NotaServico nota)
        {
            if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

            var tomador = new XElement("TomadorServico");

            var idTomador = new XElement("IdentificacaoTomador");
            tomador.Add(idTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            idTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            idTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 150, Ocorrencia.NaoObrigatoria,
                nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "NifTomador", 1, 150, Ocorrencia.NaoObrigatoria,
                nota.Tomador.DocTomadorEstrangeiro));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria,
                nota.Tomador.RazaoSocial));

            if (nota.Tomador.EnderecoExterior.CodigoPais > 0)
            {
                var enderecoExt = new XElement("EnderecoExterior");
                tomador.Add(enderecoExt);

                enderecoExt.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 8, 8, Ocorrencia.Obrigatoria,
                    nota.Tomador.EnderecoExterior.CodigoPais));
                enderecoExt.AddChild(AdicionarTag(TipoCampo.Str, "", "EnderecoCompletoExterior", 8, 8,
                    Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto));
            }
            else if (nota.Tomador.Endereco.CodigoMunicipio > 0)
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.Obrigatoria,
                    nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.Obrigatoria,
                    nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.Obrigatoria,
                    nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria,
                    nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria,
                    nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.Obrigatoria,
                    nota.Tomador.Endereco.Cep));
            }

            if (nota.Tomador.DadosContato.Email.IsEmpty() && nota.Tomador.DadosContato.Telefone.IsEmpty())
                return tomador;

            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Telefone", 8, 8, Ocorrencia.NaoObrigatoria,
                nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 8, 8, Ocorrencia.NaoObrigatoria,
                nota.Tomador.DadosContato.Email));

            return tomador;
        }

        #endregion RPS

        #region Services

        /// <inheritdoc />
        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice,
            NotaServicoCollection notas)
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

            retornoWebservice.Data =
                xmlRet.Root.ElementAnyNs("ListaNfse").ElementAnyNs("CompNfse").ElementAnyNs("Nfse")
                    .ElementAnyNs("InfNfse").ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet.Root.ElementAnyNs("ListaNfse").ElementAnyNs("CompNfse")
                                              .ElementAnyNs("Nfse").ElementAnyNs("InfNfse")
                                              .ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ??
                                          "";

            var listaNfse = xmlRet.Root.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento
                    { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
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
        
        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
            NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento
                    { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
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
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty())
                loteBuilder.Append(
                    $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfseRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
        {
            var mensagens = xmlRet?.ElementAnyNs(xmlTag);
            mensagens = mensagens?.ElementAnyNs("ListaMensagemRetorno") ??
                        mensagens?.ElementAnyNs("ListaMensagemRetornoLote");
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

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
            NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
            if (retornoWebservice.Erros.Any()) return;

            var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");

            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento
                    { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
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

        protected override IServiceClient GetClient(TipoUrl tipo) => new GISSServiceClient(this, tipo, Certificado);

        protected override string GetSchema(TipoUrl tipo) => "nfse_v2-04.xsd";
        
        #endregion Protected Methods

        #endregion Methods
    }
}