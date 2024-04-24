using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers.ISSRecife;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers.Agili
{
    internal class ProviderAgili : ProviderABRASF
    {
        #region Constructors

        public ProviderAgili(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Agili";
        }

        #endregion Constructors

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new AgiliServiceClient(this, tipo);
        }

        protected virtual XElement WriteIdentificacaoPrestador(NotaServico nota)
        {
            var identificacaoPrestador = new XElement("IdentificacaoPrestador");

            identificacaoPrestador.AddChild(AdicionarTag(TipoCampo.Str, "", "ChaveDigital", 1, 32, Ocorrencia.Obrigatoria, Configuracoes.WebServices.ChavePrivada));
            //identificacaoPrestador.AddChild(AdicionarTag(TipoCampo.Str, "", "Signature", 1, 32, Ocorrencia.Obrigatoria, "123"));

            var cpfCnpj = new XElement("CpfCnpj");

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", Configuracoes.PrestadorPadrao.CpfCnpj));

            identificacaoPrestador.Add(cpfCnpj);
            identificacaoPrestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 32, Ocorrencia.Obrigatoria, Configuracoes.PrestadorPadrao.InscricaoMunicipal));

            return identificacaoPrestador;
        }

        protected override XElement WriteIdentificacao(NotaServico nota)
        {
            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "-2";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "-4";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "-5";
                    break;

                default:
                    tipoRps = "0";
                    break;
            }

            var ideRps = new XElement("IdentificacaoRps");
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

            return ideRps;
        }

        protected override XElement WriteTomadorRps(NotaServico nota)
        {
            var tomador = new XElement("DadosTomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));
            tomador.AddChild(AdicionarTag(TipoCampo.Int, "", "LocalEndereco", 1, 1, Ocorrencia.Obrigatoria, 1));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
                !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
                !nota.Tomador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouro", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.TipoLogradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Logradouro", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));

                var municipio = new XElement("Municipio");
                municipio.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipioIBGE", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
                municipio.AddChild(AdicionarTag(TipoCampo.Str, "", "Descricao", 1, 300, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Municipio));
                municipio.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));

                endereco.AddChild(municipio);

                var pais = new XElement("Pais");
                pais.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPaisBacen", 1, 4, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoPais));
                pais.AddChild(AdicionarTag(TipoCampo.Str, "", "Descricao", 1, 4, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.Pais));

                endereco.AddChild(pais);

                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                !nota.Tomador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("Contato");
                tomador.Add(contato);

                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
            }

            return tomador;
        }

        protected override XElement WriteIntermediarioRps(NotaServico nota)
        {
            if (nota.Intermediario.RazaoSocial.IsEmpty()) return null;

            var intermediario = new XElement("DadosIntermediario");

            var ideIntermediario = new XElement("IdentificacaoIntermediario");
            intermediario.Add(ideIntermediario);

            var cpfCnpj = new XElement("CpfCnpj");
            ideIntermediario.Add(cpfCnpj);

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

            ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.InscricaoMunicipal));

            intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.RazaoSocial));

            return intermediario;
        }

        protected XElement WriteListaServico(NotaServico nota)
        {
            var listaServico = new XElement("ListaServico");
            var dadosServico = new XElement("DadosServico");

            dadosServico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            dadosServico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 140, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
            dadosServico.AddChild(AdicionarTag(TipoCampo.Int, "", "Quantidade", 1, 13, Ocorrencia.NaoObrigatoria, 1));
            dadosServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServico", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

            listaServico.AddChild(dadosServico);

            return listaServico;
        }

        protected override XElement WriteRps(NotaServico nota)
        {
            var declaracaoPrestacaoServico = new XElement("DeclaracaoPrestacaoServico");
            declaracaoPrestacaoServico.Add(WriteIdentificacaoPrestador(nota));

            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "NfseSubstituida", 1, 1, Ocorrencia.MaiorQueZero, 0));

            var rps = new XElement("Rps");
            rps.AddChild(WriteIdentificacao(nota));
            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 1, 1, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            declaracaoPrestacaoServico.AddChild(rps);

            declaracaoPrestacaoServico.AddChild(WriteTomadorRps(nota));
            declaracaoPrestacaoServico.AddChild(WriteIntermediarioRps(nota));

            if (nota.RegimeEspecialTributacao.IsIn(
                RegimeEspecialTributacao.Estimativa,
                RegimeEspecialTributacao.SociedadeProfissionais,
                RegimeEspecialTributacao.Cooperativa,
                RegimeEspecialTributacao.MicroEmpresarioIndividual,
                RegimeEspecialTributacao.MicroEmpresarioEmpresaPP))
            {
                var regimeEspecialTributacao = new XElement("RegimeEspecialTributacao");

                switch (nota.RegimeEspecialTributacao)
                {
                    case RegimeEspecialTributacao.Estimativa:
                        {
                            regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-2"));
                            break;
                        }
                    case RegimeEspecialTributacao.SociedadeProfissionais:
                        {
                            regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-3"));
                            break;
                        }
                    case RegimeEspecialTributacao.Cooperativa:
                        {
                            regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-4"));
                            break;
                        }
                    case RegimeEspecialTributacao.MicroEmpresarioIndividual:
                        {
                            regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-5"));
                            break;
                        }
                    case RegimeEspecialTributacao.MicroEmpresarioEmpresaPP:
                        {
                            regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-6"));
                            break;
                        }
                }

                declaracaoPrestacaoServico.AddChild(regimeEspecialTributacao);
            }

            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, nota.OptanteSimplesNacional == NFSeSimNao.Sim ? 1 : 0));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteMEISimei", 1, 1, Ocorrencia.Obrigatoria, nota.OptanteMEISimei == NFSeSimNao.Sim ? 1 : 0));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "ISSQNRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 0));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemLei116AtividadeEconomica", 1, 140, Ocorrencia.NaoObrigatoria, nota.Servico.ItemListaServico));

            var exigibilidadeIss = new XElement("ExigibilidadeISSQN");

            switch (nota.Servico.ExigibilidadeIss)
            {
                case ExigibilidadeIss.Exigivel:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-1"));
                        break;
                    }
                case ExigibilidadeIss.NaoIncidencia:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-2"));
                        break;
                    }
                case ExigibilidadeIss.Isencao:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-3"));
                        break;
                    }
                case ExigibilidadeIss.Exportacao:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-4"));
                        break;
                    }
                case ExigibilidadeIss.Imunidade:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-5"));
                        break;
                    }
                case ExigibilidadeIss.SuspensaDecisaoJudicial:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-6"));
                        break;
                    }
                case ExigibilidadeIss.SuspensaProcessoAdministrativo:
                    {
                        exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria, "-7"));
                        break;
                    }
            }

            declaracaoPrestacaoServico.AddChild(exigibilidadeIss);

            var municipioIncidencia = new XElement("MunicipioIncidencia");

            municipioIncidencia.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipioIBGE", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.MunicipioIncidencia));
            declaracaoPrestacaoServico.AddChild(municipioIncidencia);

            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIrrf", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorOutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorOutrasRetencoes));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorBaseCalculoISSQN", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "AliquotaISSQN", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorISSQNCalculado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));

            declaracaoPrestacaoServico.AddChild(WriteListaServico(nota));

            declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Str, "", "Versao", 1, 1, Ocorrencia.Obrigatoria, "1.00"));

            return declaracaoPrestacaoServico;
        }

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
            xmlLote.Append($"<UnidadeGestora>{Municipio.CnpjPrefeitura}</UnidadeGestora>");
            xmlLote.Append(xmlLoteRps);

            xmlLote.Append("</GerarNfseEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Any()) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                nota.Servico.ItemListaServico = nota.Servico.ItemListaServico.OnlyNumbers();

                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
            xmlLote.Append($"<UnidadeGestora>{Municipio.CnpjPrefeitura}</UnidadeGestora>");
            xmlLote.Append($"<LoteRps>");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            xmlLote.Append($"<IdentificacaoPrestador>");
            xmlLote.Append($"<ChaveDigital>{Configuracoes.WebServices.ChavePrivada}</ChaveDigital>");
            xmlLote.Append($"<CpfCnpj>");

            switch (Configuracoes.PrestadorPadrao.CpfCnpj.Length)
            {
                case 11:
                    {
                        xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cnpj>");
                        break;
                    }
                case 14:
                    {
                        xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
                        break;
                    }
            }

            xmlLote.Append($"</CpfCnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"</IdentificacaoPrestador>");

            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");

            xmlLote.Append(xmlLoteRps);

            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append($"<Versao>1.00</Versao>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            return;
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            return;
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            return;
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append($"<UnidadeGestora>{Municipio.CnpjPrefeitura}</UnidadeGestora>");
            loteBuilder.Append("<PedidoCancelamento>");
            loteBuilder.Append("<IdentificacaoNfse>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
            loteBuilder.Append("<IdentificacaoPrestador>");
            loteBuilder.Append($"<ChaveDigital>{Configuracoes.WebServices.ChavePrivada}</ChaveDigital>");
            loteBuilder.Append("<CpfCnpj>");

            switch (Configuracoes.PrestadorPadrao.CpfCnpj.Length)
            {
                case 11:
                    {
                        loteBuilder.Append($"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cpf>");
                        break;
                    }
                case 14:
                    {
                        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cnpj>");
                        break;
                    }
            }
            
            loteBuilder.Append("</CpfCnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</IdentificacaoPrestador>");
            loteBuilder.Append("</IdentificacaoNfse>");
            loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
            loteBuilder.Append($"<JustificativaCancelamento>{retornoWebservice.Motivo}</JustificativaCancelamento>");
            loteBuilder.Append($"<Versao>1.00</Versao>");
            loteBuilder.Append("</PedidoCancelamento>");
            loteBuilder.Append("</CancelarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarSituacaoLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo xmlns=\"\">{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append($"<ConsultarLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarNfsePorRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<IdentificacaoRps xmlns=\"\">");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
            loteBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
            loteBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}</Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfsePorRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");

            if (retornoWebservice.NumeroNFse > 0)
                loteBuilder.Append($"<NumeroNfse xmlns=\"\">{retornoWebservice}</NumeroNfse>");

            if (retornoWebservice.Inicio.HasValue && retornoWebservice.Fim.HasValue)
            {
                loteBuilder.Append("<PeriodoEmissao xmlns=\"\">");
                loteBuilder.Append($"<DataInicial>{retornoWebservice.Inicio:yyyy-MM-dd}</DataInicial>");
                loteBuilder.Append($"<DataFinal>{retornoWebservice.Fim:yyyy-MM-dd}</DataFinal>");
                loteBuilder.Append("</PeriodoEmissao>");
            }

            if (!retornoWebservice.CPFCNPJTomador.IsEmpty())
            {
                loteBuilder.Append("<Tomador xmlns=\"\">");
                loteBuilder.Append("<CpfCnpj>");
                loteBuilder.Append(retornoWebservice.CPFCNPJTomador.IsCNPJ()
                    ? $"<Cnpj>{retornoWebservice.CPFCNPJTomador.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{retornoWebservice.CPFCNPJTomador.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!retornoWebservice.IMTomador.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMTomador}</InscricaoMunicipal>");
                loteBuilder.Append("</Tomador>");
            }

            if (!retornoWebservice.NomeIntermediario.IsEmpty() && !retornoWebservice.CPFCNPJIntermediario.IsEmpty())
            {
                loteBuilder.Append("<IntermediarioServico xmlns=\"\">");
                loteBuilder.Append($"<RazaoSocial>{retornoWebservice.NomeIntermediario}</RazaoSocial>");
                loteBuilder.Append(retornoWebservice.CPFCNPJIntermediario.IsCNPJ()
                    ? $"<Cnpj>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!retornoWebservice.IMIntermediario.IsEmpty())
                    loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMIntermediario}</InscricaoMunicipal>");
                loteBuilder.Append("</IntermediarioServico>");
            }

            loteBuilder.Append("</ConsultarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.agili.com.br/nfse_v_1.00.xsd\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse-v-100.xsd";
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root);
            if (retornoWebservice.Erros.Any()) return;

            var protocoloCancelamento = xmlRet.Root.ElementAnyNs("ProtocoloRequerimentoCancelamento");
            if (protocoloCancelamento == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
                return;
            }

            retornoWebservice.Data = xmlRet.Root.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = true;
            retornoWebservice.CodigoCancelamento = protocoloCancelamento.GetValue<string>();

            var numeroNFSe = xmlRet.Root.ElementAnyNs("PedidoCancelamento").ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
            if (nota == null) return;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
            nota.Cancelamento.DataHora = retornoWebservice.Data;
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root);
            if (retornoWebservice.Erros.Any()) return;

            var nfse = xmlRet.ElementAnyNs("GerarNfseResposta")?.ElementAnyNs("Nfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoAutenticidade")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse?.ElementAnyNs("DeclaracaoPrestacaoServico")?.ElementAnyNs("Rps")?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(nfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Add(LoadXml(retornoWebservice.XmlRetorno));
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            }
                        
            retornoWebservice.Sucesso = true;
        }

        public override NotaServico LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");
            
            var rootGrupo = xml.ElementAnyNs("GerarNfseResposta");
            var rootDoc = rootGrupo.ElementAnyNs("Nfse");

            Guard.Against<XmlException>(rootDoc == null, "Xml de RPS ou NFSe invalido.");

            var ret = new NotaServico(Configuracoes)
            {
                XmlOriginal = xml.AsString()
            };

            ret.IdentificacaoNFSe.Numero = rootDoc.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoNFSe.Chave = rootDoc.ElementAnyNs("CodigoAutenticidade")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoNFSe.DataEmissao = rootDoc.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // RPS
            var rootRps = rootDoc.ElementAnyNs("IdentificacaoRps");
            if (rootRps != null)
            {
                ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Serie = rootRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Tipo = rootRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
            }

            ret.IdentificacaoRps.DataEmissao = rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("Rps").ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Simples Nacional
            if (rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("OptanteSimplesNacional")?.GetValue<int>() == 1)
            {
                ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            }
            else
            {
                // Regime Especial de Tributação
                switch (rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("RegimeEspecialTributacao").ElementAnyNs("Codigo")?.GetValue<int>())
                {
                    case -2:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Estimativa;
                        break;

                    case -3:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SociedadeProfissionais;
                        break;

                    case -4:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Cooperativa;
                        break;

                    case -5:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioIndividual;
                        break;

                    case -6:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioEmpresaPP;
                        break;
                }
            }

            ret.RpsSubstituido.NumeroNfse = rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
            ret.InformacoesComplementares = rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
            ret.OutrasInformacoes = rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("Observacao")?.GetValue<string>() ?? string.Empty;

            // Serviços e Valores
            LoadServicosValoresRps(ret, rootDoc);

            // Prestador (Nota Fiscal)
            var rootPrestador = rootDoc.ElementAnyNs("DadosPrestador");
            if (rootPrestador != null)
            {
                var rootPretadorIdentificacao = rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("IdentificacaoPrestador");
                if (rootPretadorIdentificacao != null)
                {
                    ret.Prestador.CpfCnpj = rootPretadorIdentificacao.ElementAnyNs("CpfCnpj").ElementAnyNs("Cpf")?.GetValue<string>() ?? rootPretadorIdentificacao.ElementAnyNs("CpfCnpj").ElementAnyNs("Cnpj")?.GetValue<string>();                    
                    ret.Prestador.InscricaoMunicipal = rootPretadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }

                ret.Prestador.RazaoSocial = rootPrestador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.NomeFantasia = rootPrestador.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;
                var rootPrestadorEndereco = rootPrestador.ElementAnyNs("Endereco");
                if (rootPrestadorEndereco != null)
                {
                    ret.Prestador.Endereco.TipoLogradouro = rootPrestadorEndereco.ElementAnyNs("TipoLogradouro")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Logradouro = rootPrestadorEndereco.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Numero = rootPrestadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Complemento = rootPrestadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Bairro = rootPrestadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.CodigoMunicipio = rootPrestadorEndereco.ElementAnyNs("Municipio").ElementAnyNs("CodigoMunicipioIBGE")?.GetValue<int>() ?? 0;
                    ret.Prestador.Endereco.Uf = rootPrestadorEndereco.ElementAnyNs("Municipio").ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Cep = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootPrestadorContato = rootPrestador.ElementAnyNs("Contato");
                if (rootPrestadorContato != null)
                {
                    ret.Prestador.DadosContato.DDD = "";
                    ret.Prestador.DadosContato.Telefone = rootPrestadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.DadosContato.Email = rootPrestadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Tomador
            var rootTomador = rootDoc.ElementAnyNs("DeclaracaoPrestacaoServico").ElementAnyNs("DadosTomador");
            if (rootTomador != null)
            {
                var rootTomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
                if (rootTomadorIdentificacao != null)
                {
                    var rootTomadorIdentificacaoCpfCnpj = rootTomadorIdentificacao.ElementAnyNs("CpfCnpj");
                    if (rootTomadorIdentificacaoCpfCnpj != null)
                    {
                        ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                        if (ret.Tomador.CpfCnpj.IsEmpty())
                        {
                            ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                        }
                    }
                    ret.Tomador.InscricaoMunicipal = rootTomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
                ret.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootTomadorEndereco = rootTomador.ElementAnyNs("Endereco");
                if (rootTomadorEndereco != null)
                {
                    ret.Tomador.Endereco.TipoLogradouro = rootTomadorEndereco.ElementAnyNs("TipoLogradouro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Logradouro = rootTomadorEndereco.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Numero = rootTomadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Complemento = rootTomadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Bairro = rootTomadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CodigoMunicipio = rootTomadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                    ret.Tomador.Endereco.Uf = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Cep = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
                if (rootTomadorContato != null)
                {
                    ret.Tomador.DadosContato.DDD = "";
                    ret.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Intermediario
            var rootIntermediarioIdentificacao = rootDoc.ElementAnyNs("DadosIntermediario");
            if (rootIntermediarioIdentificacao != null)
            {
                ret.Intermediario.RazaoSocial = rootIntermediarioIdentificacao.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootIntermediarioIdentificacaoCpfCnpj = rootIntermediarioIdentificacao.ElementAnyNs("CpfCnpj");
                if (rootIntermediarioIdentificacaoCpfCnpj != null)
                {
                    ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                    if (ret.Intermediario.CpfCnpj.IsEmpty())
                        ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                }
                ret.Intermediario.InscricaoMunicipal = rootIntermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            var rootOrgaoGerador = rootDoc.ElementAnyNs("IdentificacaoOrgaoGerador");
            if (rootOrgaoGerador != null)
            {
                ret.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("Municipio").ElementAnyNs("CodigoMunicipioIBGE")?.GetValue<int>() ?? 0;
                ret.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Municipio").ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
            }

            return ret;
        }

        protected override void LoadServicosValoresRps(NotaServico nota, XElement rootNFSe)
        {
            var rootServico = rootNFSe.ElementAnyNs("DeclaracaoPrestacaoServico");
            if (rootServico == null) return;

            nota.Servico.Valores.ValorServicos = rootServico.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorDeducoes = rootServico.ElementAnyNs("ValorDeducaoConstCivil")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorPis = rootServico.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorCofins = rootServico.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorInss = rootServico.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorIr = rootServico.ElementAnyNs("ValorIr")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorCsll = rootServico.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.IssRetido = (rootServico.ElementAnyNs("ISSQNRetido")?.GetValue<int>() ?? 0) == 1 ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;
            nota.Servico.Valores.ValorIss = rootServico.ElementAnyNs("ValorISSQNCalculado")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorOutrasRetencoes = rootServico.ElementAnyNs("ValorOutrasRetencoes")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.BaseCalculo = rootServico.ElementAnyNs("ValorBaseCalculoISSQN")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.Aliquota = rootServico.ElementAnyNs("AliquotaISSQN")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorLiquidoNfse = rootServico.ElementAnyNs("ValorLiquido")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorIssRetido = rootServico.ElementAnyNs("ValorISSQNRecolher")?.GetValue<decimal>() ?? 0;

            nota.Servico.ItemListaServico = rootServico.ElementAnyNs("ItemLei116AtividadeEconomica")?.GetValue<string>() ?? string.Empty;
            nota.Servico.CodigoCnae = rootServico.ElementAnyNs("CodigoCnaeAtividadeEconomica")?.GetValue<string>() ?? string.Empty;

            nota.Servico.Discriminacao = rootServico.ElementAnyNs("ListaServico").ElementAnyNs("DadosServico").ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;

        }
    }
}
