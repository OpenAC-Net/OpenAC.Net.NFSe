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
    internal class ProviderGISS : ProviderABRASF204
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
        
        protected override XElement WriteValoresRps(NotaServico nota)
        {
            var valores = new XElement("Valores");

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValTotTributos", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValTotTributos));

            var valorISS = nota.Servico.Valores.ValorIss;

            if (valorISS <= 0 && nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao && nota.Servico.Valores.ValorIssRetido > 0)
                valorISS = nota.Servico.Valores.ValorIssRetido;

            if (nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
                valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, valorISS));

            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional || nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
                valores.AddChild(AdicionarTag(TipoCampo.De2, "", "Aliquota", 1, 5, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            return valores;
        }

        #endregion RPS

        #region Services
        
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
            xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
            xmlLote.Append($"<LoteRps Id=\"lote{retornoWebservice.Lote}\" {GetVersao()}>");
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
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            
            var xmlstring = xmlLote.ToString().Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>", "");
            XDocument doc = XDocument.Parse(xmlstring);
            XNamespace nsRoot = "http://www.giss.com.br/enviar-lote-rps-envio-v2_04.xsd";
            XNamespace nsChild = "http://www.giss.com.br/tipos-v2_04.xsd";
            
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;
            
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "ds", "http://www.w3.org/2000/09/xmldsig#");
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "ns4", nsRoot);
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "ns2", nsChild);
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance");
            
            foreach (var element in doc.Descendants().ToList())
                element.Name = nsChild + element.Name.LocalName;
            
            var loteRps = doc.Descendants().First(x=>x.Name.LocalName == "LoteRps");
            loteRps.Name = nsRoot + loteRps.Name.LocalName;
            loteRps.SetAttributeValue("versao", "1.00");
            
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;

            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + doc;

            retornoWebservice.XmlEnvio = xml;
        }
        
        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns2:Rps", "ns2:InfDeclaracaoPrestacaoServico", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns4:EnviarLoteRpsEnvio", "ns4:LoteRps", Certificado);
        }
        
        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append($"<ConsultarLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append("<CpfCnpj>");
            loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
                ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");
            
            var xmlstring = loteBuilder.ToString().Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>", "");
            XDocument doc = XDocument.Parse(xmlstring);
            XNamespace nsRoot = "http://www.giss.com.br/consultar-lote-rps-envio-v2_04.xsd";
            XNamespace nsChild = "http://www.giss.com.br/tipos-v2_04.xsd";
            
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;
            
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "con", nsRoot);
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "tip", nsChild);
            
            foreach (var element in doc.Descendants().ToList())
                element.Name = nsChild + element.Name.LocalName;
            
            var loteRps = doc.Descendants().First(x=>x.Name.LocalName == "Prestador");
            loteRps.Name = nsRoot + loteRps.Name.LocalName;
            var protocolo = doc.Descendants().First(x=>x.Name.LocalName == "Protocolo");
            protocolo.Name = nsRoot + protocolo.Name.LocalName;
            
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;

            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + doc;
            
            retornoWebservice.XmlEnvio = xml;
        }

        /// <inheritdoc />
        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "con:ConsultarLoteRpsEnvio", "", Certificado);
        }
        
        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            retornoWebservice.Lote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResposta");
            var situacao = retornoLote?.ElementAnyNs("Situacao");
            if (situacao != null)
                retornoWebservice.Situacao = situacao.GetValue<string>();
            
            MensagemErro(retornoWebservice, xmlRet, "ConsultarLoteRpsResposta");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = true;

            if (notas == null) return;

            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return;
            }

            var notasFiscais = new List<NotaServico>();

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
                    nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                    nota.XmlOriginal = compNfse.ToString();
                }

                nota.Protocolo = retornoWebservice.Protocolo;
                notasFiscais.Add(nota);
            }

            retornoWebservice.Notas = notasFiscais.ToArray();
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
            loteBuilder.Append("<Pedido>");
            loteBuilder.Append($"<InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
            loteBuilder.Append("<IdentificacaoNfse>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
            loteBuilder.Append("<CpfCnpj>");
            loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
                ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
            loteBuilder.Append("</IdentificacaoNfse>");
            loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
            loteBuilder.Append("</InfPedidoCancelamento>");
            loteBuilder.Append("</Pedido>");
            loteBuilder.Append("</CancelarNfseEnvio>");
            var xmlstring = loteBuilder.ToString().Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>", "");
            XDocument doc = XDocument.Parse(xmlstring);
            
            XNamespace nsRoot = "http://www.giss.com.br/cancelar-nfse-envio-v2_04.xsd";
            XNamespace nsChild = "http://www.giss.com.br/tipos-v2_04.xsd";
                
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;
                
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "can", nsRoot);
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "tip", nsChild);
                
            foreach (var element in doc.Descendants().ToList())
                element.Name = nsChild + element.Name.LocalName;
                
            var pedido = doc.Descendants().First(x=>x.Name.LocalName == "Pedido");
            pedido.Name = nsRoot + pedido.Name.LocalName;
                
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;

            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + doc;
                
            retornoWebservice.XmlEnvio = xml;
        }

        /// <inheritdoc />
        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "can:Pedido", "tip:InfPedidoCancelamento", Certificado);
        }
        
        /// <inheritdoc />
        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<IdentificacaoRps>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
            loteBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
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
            
            var xmlstring = loteBuilder.ToString().Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>", "");
            XDocument doc = XDocument.Parse(xmlstring);
            
            XNamespace nsRoot = "http://www.giss.com.br/consultar-nfse-rps-envio-v2_04.xsd";
            XNamespace nsChild = "http://www.giss.com.br/tipos-v2_04.xsd";
                
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;
                
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "con", nsRoot);
            doc.Root.SetAttributeValue(XNamespace.Xmlns + "tip", nsChild);
                
            foreach (var element in doc.Descendants().ToList())
                element.Name = nsChild + element.Name.LocalName;
                
            var identificador = doc.Descendants().First(x=>x.Name.LocalName == "IdentificacaoRps");
            identificador.Name = nsRoot + identificador.Name.LocalName;  
            
            var prestador = doc.Descendants().First(x=>x.Name.LocalName == "Prestador");
            prestador.Name = nsRoot + prestador.Name.LocalName;
                
            doc.Root.Name = nsRoot + doc.Root.Name.LocalName;

            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + doc;
            retornoWebservice.XmlEnvio = xml;
        }

        /// <inheritdoc />
        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "con:ConsultarNfseRpsEnvio", "", Certificado);
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
            retornoWebservice.Sucesso = true;
            MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
            if (retornoWebservice.Erros.Any())
            {
                retornoWebservice.Sucesso = false;
                return;
            }

            var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");

            if (compNfse == null)
            {
                retornoWebservice.Sucesso = false;
                retornoWebservice.Erros.Add(new Evento
                    { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
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
            }

            retornoWebservice.Nota = nota;
        }

        #endregion Services

        protected override IServiceClient GetClient(TipoUrl tipo) => new GISSServiceClient(this, tipo, Certificado);

        protected override string GetSchema(TipoUrl tipo) => "nfse_v2-04.xsd";
        protected override string GetNamespace() => "";
        
        #endregion Protected Methods

        #endregion Methods
    }
}