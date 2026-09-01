using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers.Barueri;

namespace OpenAC.Net.NFSe.Providers
{

    internal sealed class ProviderBarueri : ProviderBase
    {
        #region Constants
        private const string VersaoLayout = "PMB004";
        #endregion

        #region Constructors

        public ProviderBarueri(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Barueri";
        }

        #endregion Constructors

        #region Methods

        #region LoadXml

        public override NotaServico LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");

            XElement rootDoc;
            XElement rootNFSe = xml.ElementAnyNs("RPS");

            if (rootNFSe != null)
            {
                rootDoc = rootNFSe;
            }
            else
            {
                rootNFSe = xml.ElementAnyNs("NFe");
                Guard.Against<XmlException>(rootNFSe == null, "Xml de RPS ou NFSe invalido.");
                rootDoc = rootNFSe;
            }

            var ret = new NotaServico(Configuracoes);

            // Carregar dados do RPS/NFe
            ret.IdentificacaoRps.Numero = rootDoc.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoRps.Serie = rootDoc.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoRps.Tipo = TipoRps.RPS;

            // Prestador
            var prestadorRoot = rootDoc.ElementAnyNs("Prestador");
            if (prestadorRoot != null)
            {
                ret.Prestador.CpfCnpj = prestadorRoot.ElementAnyNs("CPFCNPJPrestador")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.InscricaoMunicipal = prestadorRoot.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            // Tomador
            var tomadorRoot = rootDoc.ElementAnyNs("Tomador");
            if (tomadorRoot != null)
            {
                ret.Tomador.CpfCnpj = tomadorRoot.ElementAnyNs("CPFCNPJTomador")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.RazaoSocial = tomadorRoot.ElementAnyNs("RazaoSocialTomador")?.GetValue<string>() ?? string.Empty;

                var enderecoTomador = tomadorRoot.ElementAnyNs("Endereco");
                if (enderecoTomador != null)
                {
                    ret.Tomador.Endereco.Logradouro = enderecoTomador.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Numero = enderecoTomador.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Complemento = enderecoTomador.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Bairro = enderecoTomador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CodigoMunicipio = enderecoTomador.ElementAnyNs("Cidade")?.GetValue<int>() ?? 0;
                    ret.Tomador.Endereco.Uf = enderecoTomador.ElementAnyNs("UF")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Cep = enderecoTomador.ElementAnyNs("CEP")?.GetValue<string>() ?? string.Empty;
                }

                ret.Tomador.DadosContato.Email = tomadorRoot.ElementAnyNs("EmailTomador")?.GetValue<string>() ?? string.Empty;
            }

            // Serviço
            ret.Servico.Discriminacao = rootDoc.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
            ret.Servico.ItemListaServico = rootDoc.ElementAnyNs("CodigoServico")?.GetValue<string>() ?? string.Empty;

            ret.Servico.Valores.ValorServicos = rootDoc.ElementAnyNs("ValorServico")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorDeducoes = rootDoc.ElementAnyNs("ValorDeducao")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorPis = rootDoc.ElementAnyNs("ValorPIS")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorCofins = rootDoc.ElementAnyNs("ValorCOFINS")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorInss = rootDoc.ElementAnyNs("ValorINSS")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorIr = rootDoc.ElementAnyNs("ValorIR")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorCsll = rootDoc.ElementAnyNs("ValorCSLL")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.IssRetido = (rootDoc.ElementAnyNs("ISSRetido")?.GetValue<int>() ?? 0) == 1
                ? SituacaoTributaria.Retencao
                : SituacaoTributaria.Normal;
            ret.Servico.Valores.ValorIssRetido = rootDoc.ElementAnyNs("ValorISSRetido")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.Aliquota = rootDoc.ElementAnyNs("AliquotaServico")?.GetValue<decimal>() ?? 0;

            return ret;
        }

        public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            var rps = new XElement("RPS");

            var prestador = new XElement("Prestador");
            prestador.Add(new XElement("CPFCNPJPrestador", nota.Prestador.CpfCnpj));
            prestador.Add(new XElement("InscricaoMunicipal", nota.Prestador.InscricaoMunicipal));
            rps.Add(prestador);

            rps.Add(new XElement("TipoRPS", "RPS"));
            rps.Add(new XElement("Serie", nota.IdentificacaoRps.Serie));
            rps.Add(new XElement("Numero", nota.IdentificacaoRps.Numero));
            rps.Add(new XElement("DataEmissao", nota.IdentificacaoRps.DataEmissao.ToString("yyyy-MM-dd")));
            rps.Add(new XElement("SituacaoRPS", "N")); // N = Normal
            rps.Add(new XElement("SeriePrestacao", nota.IdentificacaoRps.Serie));
            rps.Add(new XElement("ValorServico", nota.Servico.Valores.ValorServicos.ToString("0.00", CultureInfo.InvariantCulture)));
            rps.Add(new XElement("ValorDeducao", nota.Servico.Valores.ValorDeducoes.ToString("0.00", CultureInfo.InvariantCulture)));
            rps.Add(new XElement("CodigoServico", nota.Servico.ItemListaServico));
            rps.Add(new XElement("AliquotaServico", nota.Servico.Valores.Aliquota.ToString("0.0000", CultureInfo.InvariantCulture)));

            rps.Add(new XElement("ISSRetido", nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "1" : "2"));
            if (nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
            {
                rps.Add(new XElement("ValorISSRetido", nota.Servico.Valores.ValorIssRetido.ToString("0.00", CultureInfo.InvariantCulture)));
            }

            rps.Add(new XElement("ValorPIS", nota.Servico.Valores.ValorPis.ToString("0.00", CultureInfo.InvariantCulture)));
            rps.Add(new XElement("ValorCOFINS", nota.Servico.Valores.ValorCofins.ToString("0.00", CultureInfo.InvariantCulture)));
            rps.Add(new XElement("ValorINSS", nota.Servico.Valores.ValorInss.ToString("0.00", CultureInfo.InvariantCulture)));
            rps.Add(new XElement("ValorIR", nota.Servico.Valores.ValorIr.ToString("0.00", CultureInfo.InvariantCulture)));
            rps.Add(new XElement("ValorCSLL", nota.Servico.Valores.ValorCsll.ToString("0.00", CultureInfo.InvariantCulture)));

            rps.Add(new XElement("Discriminacao", nota.Servico.Discriminacao));

            // Tomador
            var tomador = new XElement("Tomador");
            tomador.Add(new XElement("CPFCNPJTomador", nota.Tomador.CpfCnpj));

            if (!string.IsNullOrWhiteSpace(nota.Tomador.InscricaoMunicipal))
                tomador.Add(new XElement("InscricaoMunicipalTomador", nota.Tomador.InscricaoMunicipal));

            tomador.Add(new XElement("RazaoSocialTomador", nota.Tomador.RazaoSocial));

            var endereco = new XElement("Endereco");
            endereco.Add(new XElement("Logradouro", nota.Tomador.Endereco.Logradouro));
            endereco.Add(new XElement("Numero", nota.Tomador.Endereco.Numero));
            if (!string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Complemento))
                endereco.Add(new XElement("Complemento", nota.Tomador.Endereco.Complemento));
            endereco.Add(new XElement("Bairro", nota.Tomador.Endereco.Bairro));
            endereco.Add(new XElement("Cidade", nota.Tomador.Endereco.CodigoMunicipio));
            endereco.Add(new XElement("UF", nota.Tomador.Endereco.Uf));
            endereco.Add(new XElement("CEP", nota.Tomador.Endereco.Cep));
            tomador.Add(endereco);

            if (!string.IsNullOrWhiteSpace(nota.Tomador.DadosContato.Email))
                tomador.Add(new XElement("EmailTomador", nota.Tomador.DadosContato.Email));

            rps.Add(tomador);

            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xmlDoc.Add(rps);

            return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            throw new NotImplementedException("Barueri não implementa geração de XML de NFSe.");
        }

        #endregion LoadXml

        #region Services

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Count > 0) return;

            // Gera o XML do arquivo RPS conforme especificação de Barueri
            var xmlArquivo = GenerateArquivoXml(notas);

            // O layout de Barueri é posicional; remover acentuação evita variação de bytes
            xmlArquivo = xmlArquivo.RemoveAccent();

            // Converte para Base64
            var arquivoBytes = Encoding.UTF8.GetBytes(xmlArquivo);
            var arquivoBase64 = Convert.ToBase64String(arquivoBytes);

            // MensagemXML deve conter o XML conforme schema NFeLoteEnviarArquivo.v1.xsd
            XNamespace ns = "http://www.barueri.sp.gov.br/nfe";
            var envelope = new XElement(ns + "NFeLoteEnviarArquivo",
                new XElement(ns + "InscricaoMunicipal", Configuracoes.PrestadorPadrao.InscricaoMunicipal),
                new XElement(ns + "CPFCNPJContrib", Configuracoes.PrestadorPadrao.CpfCnpj),
                new XElement(ns + "NomeArquivoRPS", $"lote_{notas[0].IdentificacaoRps.DataEmissao.ToString("yyyy-MM-dd")}-{notas[0].IdentificacaoRps.Numero}.txt"),
                new XElement(ns + "ApenasValidaArq", "false"),
                new XElement(ns + "ArquivoRPSBase64", arquivoBase64)
            );

            // O BarueriServiceClient.Enviar() irá envolver isso no SOAP com VersaoSchema e MensagemXML/CDATA
            retornoWebservice.XmlEnvio = envelope.ToString(SaveOptions.DisableFormatting);

        }

        private string GenerateArquivoXml(NotaServicoCollection notas)
        {
            // Barueri espera ARQUIVO TEXTO (layout fixo) conforme RPS_Layout.pdf
            // O conteúdo será enviado em Base64 no campo ArquivoRPSBase64.

            var sb = new StringBuilder();

            var totalLinhas = 0;

            // Registro Tipo 1 (Cabeçalho)
            var loteNumero = notas.First().IdentificacaoRps.Numero;
            sb.AppendLine(BuildHeaderRegistroTipo1(Configuracoes.PrestadorPadrao.InscricaoMunicipal, loteNumero));
            totalLinhas += 1;

            // Registros Tipo 2, 4 e 5 (Detalhe) - um por RPS
            foreach (var nota in notas)
            {
                sb.AppendLine(BuildDetalheRegistroTipo2(nota));
                sb.AppendLine(BuildDetalheRegistroTipo4(nota));
                sb.AppendLine(BuildDetalheRegistroTipo5(nota));
                totalLinhas += 3;
            }

            // Registro Tipo 9 (Rodapé)
            var totalServicos = notas.Sum(n => n.Servico.Valores.ValorServicos);
            var totalRetencoes = 0m; // registro 3 não é gerado quando não há retenções
            totalLinhas += 1;
            sb.AppendLine(BuildRodapeRegistroTipo9(totalLinhas, totalServicos, totalRetencoes));

            return sb.ToString();
        }

        private static string BuildHeaderRegistroTipo1(string inscricaoMunicipal, string retornoWebserviceLote)
        {
            var buffer = new char[25];
            Fill(buffer, ' ');

            SetField(buffer, 1, 1, "1", padLeft: true, padChar: '0');
            SetField(buffer, 2, 8, inscricaoMunicipal, padLeft: true, padChar: ' ');
            SetField(buffer, 9, 14, VersaoLayout, padLeft: true, padChar: ' ');

            const int RemessaLength = 11;
            var lote = retornoWebserviceLote ?? string.Empty;
            var guidLength = Math.Max(0, RemessaLength - lote.Length);

            // Gera um número aleatório baseado em GUID (apenas dígitos)
            var guidNumeric = (uint)Guid.NewGuid().GetHashCode();
            var guidPart = (guidNumeric % (long)Math.Pow(10, guidLength)).ToString($"D{guidLength}");

            var remessa = $"{guidPart}{lote}";
            SetField(buffer, 15, 25, remessa, padLeft: true, padChar: '0');

            return new string(buffer);
        }

        /// <summary>
        /// Builds the detalhe registro tipo 2 conforme Layout v4.2.
        /// </summary>
        /// <param name="nota">Dados da nota</param>
        /// <param name="localPrestacaoServico">1 para serviço prestado no Município / 2 para serviço prestado fora do Município. * Obrigatório para as atividades relacionadas nas exceções, conforme artigo 39 da LC 118/02.</param>
        /// <param name="servicoPrestaViasPublicas">1 para serviço prestado em vias públicas / 2 para serviço não prestado em vias públicas. * Obrigatório para as atividades da lista de serviços conforme artigo 39 da LC 118/02, não prestados em vias públicas</param>
        /// <returns></returns>
        private static string BuildDetalheRegistroTipo2(
            NotaServico nota,
            string localPrestacaoServico = "1",
            string servicoPrestaViasPublicas = "2")
        {
            var buffer = new char[1970];
            Fill(buffer, ' ');

            SetField(buffer, 1, 1, "2", padLeft: true, padChar: '0');
            SetField(buffer, 2, 6, "RPS", padLeft: false, padChar: ' ');
            SetField(buffer, 7, 10, nota.IdentificacaoRps.Serie, padLeft: false, padChar: ' ');
            SetField(buffer, 11, 15, "", padLeft: false, padChar: ' ');

            if (nota.Situacao == SituacaoNFSeRps.Cancelado)
            {
                SetField(buffer, 16, 25, ToNumeric("0", 10), padLeft: true, padChar: '0');
            }
            else
            {
                SetField(buffer, 16, 25, ToNumeric(nota.IdentificacaoRps.Numero, 10), padLeft: true, padChar: '0');
            }
            SetField(buffer, 26, 33, nota.IdentificacaoRps.DataEmissao.ToString("yyyyMMdd"), padLeft: true, padChar: '0');
            SetField(buffer, 34, 39, nota.IdentificacaoRps.DataEmissao.ToString("HHmmss"), padLeft: true, padChar: '0');

            var situacaoRps = nota.Situacao == SituacaoNFSeRps.Normal ? "E" : nota.Situacao == SituacaoNFSeRps.Cancelado ? "C" : "E";

            SetField(buffer, 40, 40, situacaoRps, padLeft: false, padChar: ' ');

            if(nota.Situacao == SituacaoNFSeRps.Cancelado)
            {
                SetField(buffer, 41, 42, nota.Cancelamento.Pedido.CodigoCancelamento, padLeft: false, padChar: ' ');
                SetField(buffer, 43, 49, nota.Cancelamento.Pedido.IdentificacaoNFSe.Numero, padLeft: true, padChar: '0');
                SetField(buffer, 50, 54, nota.Cancelamento.Pedido.IdentificacaoNFSe.Serie, padLeft: false, padChar: ' ');
                SetField(buffer, 55, 62, nota.Cancelamento.Pedido.IdentificacaoNFSe.DataEmissao.ToString("yyyyMMdd"), padLeft: true, padChar: '0');
                SetField(buffer, 63, 242, nota.Cancelamento.MotivoCancelamento, padLeft: false, padChar: ' ');
            }
            else
            {
                SetField(buffer, 41, 42, "", padLeft: false, padChar: ' ');
                SetField(buffer, 43, 49, "", padLeft: true, padChar: '0');
                SetField(buffer, 50, 54, "", padLeft: false, padChar: ' ');
                SetField(buffer, 55, 62, "", padLeft: true, padChar: '0');
                SetField(buffer, 63, 242, "", padLeft: false, padChar: ' ');
            }

            SetField(buffer, 243, 251, ToNumeric(nota.Servico.ItemListaServico, 9), padLeft: true, padChar: '0');

            SetField(buffer, 252, 252, localPrestacaoServico, padLeft: false, padChar: ' ');
            SetField(buffer, 253, 253, servicoPrestaViasPublicas, padLeft: false, padChar: ' ');

            SetField(buffer, 254, 328, "", padLeft: false, padChar: ' ');
            SetField(buffer, 329, 337, "", padLeft: false, padChar: ' ');
            SetField(buffer, 338, 367, "", padLeft: false, padChar: ' ');
            SetField(buffer, 368, 407, "", padLeft: false, padChar: ' ');
            SetField(buffer, 408, 447, "", padLeft: false, padChar: ' ');
            SetField(buffer, 448, 449, "", padLeft: false, padChar: ' ');
            SetField(buffer, 450, 457, "", padLeft: true, padChar: '0');

            var quantidade = 1;
            SetField(buffer, 458, 463, quantidade.ToString().PadLeft(6, '0'), padLeft: true, padChar: '0');

            var valorUnitario = nota.Servico.Valores.ValorServicos;
            SetField(buffer, 464, 478, ToMoney(valorUnitario, 15), padLeft: true, padChar: '0');
            SetField(buffer, 479, 483, "", padLeft: false, padChar: ' ');
            SetField(buffer, 484, 498, ToMoney(0, 15), padLeft: true, padChar: '0');

            SetField(buffer, 499, 499, "2", padLeft: false, padChar: ' ');
            SetField(buffer, 500, 502, "", padLeft: false, padChar: ' ');
            SetField(buffer, 503, 503, "2", padLeft: false, padChar: ' ');

            var cpfCnpj = nota.Tomador?.CpfCnpj ?? string.Empty;
            var indicadorCpfCnpj = cpfCnpj.Length > 11 ? "2" : "1";
            SetField(buffer, 504, 504, indicadorCpfCnpj, padLeft: false, padChar: ' ');
            SetField(buffer, 505, 518, ToNumeric(cpfCnpj, 14), padLeft: true, padChar: '0');

            SetField(buffer, 519, 578, nota.Tomador?.RazaoSocial ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 579, 653, nota.Tomador?.Endereco?.Logradouro ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 654, 662, nota.Tomador?.Endereco?.Numero ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 663, 692, nota.Tomador?.Endereco?.Complemento ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 693, 732, nota.Tomador?.Endereco?.Bairro ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 733, 772, nota.Tomador?.Endereco?.Municipio ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 773, 774, nota.Tomador?.Endereco?.Uf ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 775, 782, ToNumeric(nota.Tomador?.Endereco?.Cep ?? string.Empty, 8), padLeft: true, padChar: '0');
            SetField(buffer, 783, 934, nota.Tomador?.DadosContato?.Email ?? string.Empty, padLeft: false, padChar: ' ');

            SetField(buffer, 935, 940, "", padLeft: true, padChar: '0');
            SetField(buffer, 941, 955, ToMoney(0, 15), padLeft: true, padChar: '0');
            SetField(buffer, 956, 970, "", padLeft: false, padChar: ' ');

            SetField(buffer, 971, 1970, FormatDiscriminacao(nota.Servico.Discriminacao), padLeft: false, padChar: ' ');

            return new string(buffer);
        }

        private static string BuildDetalheRegistroTipo4(NotaServico nota)
        {
            var buffer = new char[531];
            Fill(buffer, ' ');

            var ibs = nota.Servico?.Valores?.IBSCBS;
            var sitClass = ibs?.Valores?.Tributos?.SituacaoClassificacao;

            var enquadramento = "1";
            if (nota.OptanteMEISimei == NFSeSimNao.Sim) enquadramento = "2";
            else if (nota.OptanteSimplesNacional == NFSeSimNao.Sim) enquadramento = "3";

            var regimeApuracao = enquadramento == "3" ? "1" : string.Empty;

            var codigoCidadePrestacao = nota.EnderecoPrestacao?.CodigoMunicipio > 0
                ? nota.EnderecoPrestacao.CodigoMunicipio
                : (nota.Servico?.CodigoMunicipio > 0 ? nota.Servico.CodigoMunicipio : nota.Tomador?.Endereco?.CodigoMunicipio ?? 0);

            var codigoCidadeTomador = nota.Tomador?.Endereco?.CodigoMunicipio ?? 0;

            SetField(buffer, 1, 1, "4", padLeft: true, padChar: '0');
            SetField(buffer, 2, 2, enquadramento, padLeft: false, padChar: ' ');
            SetField(buffer, 3, 3, regimeApuracao, padLeft: false, padChar: ' ');
            SetField(buffer, 4, 6, string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 7, 13, codigoCidadePrestacao.ToString(), padLeft: true, padChar: '0');
            SetField(buffer, 14, 20, codigoCidadeTomador.ToString(), padLeft: true, padChar: '0');
            SetField(buffer, 21, 60, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 61, 69, ToNumeric(nota.Servico?.CodigoNbs ?? string.Empty, 9), padLeft: true, padChar: '0');
            SetField(buffer, 70, 80, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 81, 140, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 141, 141, "0", padLeft: false, padChar: ' ');
            SetField(buffer, 142, 171, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 172, 182, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 183, 242, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 243, 497, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 498, 505, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 506, 513, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 514, 514, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 515, 520, nota.Servico?.CodigoIndicadorOperacao ?? string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 521, 526, nota.Servico?.CodigoClassificacaoTributaria ?? string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 527, 529, sitClass?.CodigoSituacaoTributaria ?? string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 530, 530, ibs?.IndicadorFinal ?? "0", padLeft: false, padChar: ' ');
            SetField(buffer, 531, 531, ibs?.IndicadorDestinatario ?? "0", padLeft: false, padChar: ' ');

            return new string(buffer);
        }

        private static string BuildDetalheRegistroTipo5(NotaServico nota)
        {
            var buffer = new char[697];
            Fill(buffer, ' ');

            var ibs = nota.Servico?.Valores?.IBSCBS;
            var destinatario = nota.DestinatarioCBSIBS;

            SetField(buffer, 1, 1, "5", padLeft: true, padChar: '0');
            SetField(buffer, 2, 3, string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 4, 4, ibs?.TipoEnteGov ?? string.Empty, padLeft: false, padChar: '0');
            SetField(buffer, 5, 5, ibs?.TipoOperacao ?? string.Empty, padLeft: false, padChar: '0');
            SetField(buffer, 6, 55, ibs?.ReferenciasNFSe?.FirstOrDefault() ?? string.Empty, padLeft: false, padChar: '0');
            SetField(buffer, 56, 63, string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 64, 213, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 214, 216, string.Empty, padLeft: true, padChar: '0');
            SetField(buffer, 217, 217, "0", padLeft: false, padChar: '0');
            SetField(buffer, 218, 218, "2", padLeft: false, padChar: '0');

            SetField(buffer, 219, 232, ToNumeric(destinatario?.CpfCnpj ?? string.Empty, 14), padLeft: true, padChar: '0');
            SetField(buffer, 233, 292, destinatario?.RazaoSocial ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 293, 367, destinatario?.Endereco?.Logradouro ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 368, 376, destinatario?.Endereco?.Numero ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 377, 406, destinatario?.Endereco?.Complemento ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 407, 446, destinatario?.Endereco?.Bairro ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 447, 486, destinatario?.Endereco?.Municipio ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 487, 493, (destinatario?.Endereco?.CodigoMunicipio ?? 0).ToString(), padLeft: true, padChar: '0');
            SetField(buffer, 494, 495, destinatario?.Endereco?.Uf ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 496, 498, (destinatario?.Endereco?.CodigoPais ?? 0).ToString(), padLeft: true, padChar: ' ');
            SetField(buffer, 499, 506, ToNumeric(destinatario?.Endereco?.Cep ?? string.Empty, 8), padLeft: true, padChar: '0');
            SetField(buffer, 507, 586, destinatario?.DadosContato?.Email ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 587, 626, destinatario?.DocEstrangeiro ?? string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 627, 637, string.Empty, padLeft: false, padChar: ' ');
            SetField(buffer, 638, 697, string.Empty, padLeft: false, padChar: ' ');

            return new string(buffer);
        }

        private static string BuildRodapeRegistroTipo9(int totalLinhas, decimal totalServicos, decimal totalRetencoes)
        {
            var buffer = new char[38];
            Fill(buffer, ' ');

            SetField(buffer, 1, 1, "9", padLeft: true, padChar: '0');
            SetField(buffer, 2, 8, totalLinhas.ToString().PadLeft(7, '0'), padLeft: true, padChar: '0');

            SetField(buffer, 9, 23, ToMoney(totalServicos, 15), padLeft: true, padChar: '0');
            SetField(buffer, 24, 38, ToMoney(totalRetencoes, 15), padLeft: true, padChar: '0');

            return new string(buffer);
        }

        private static void Fill(char[] buffer, char value)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = value;
        }

        private static void SetField(char[] buffer, int start, int end, string value, bool padLeft, char padChar)
        {
            var length = end - start + 1;
            var safe = value ?? string.Empty;
            if (safe.Length > length) safe = safe.Substring(0, length);

            safe = padLeft ? safe.PadLeft(length, padChar) : safe.PadRight(length, padChar);
            safe.CopyTo(0, buffer, start - 1, length);
        }

        private static string ToNumeric(string value, int length)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length > length) digits = digits.Substring(digits.Length - length);
            return digits.PadLeft(length, '0');
        }

        private static string ToMoney(decimal value, int length)
        {
            var scaled = (long)Math.Round(value * 100m, 0);
            return scaled.ToString().PadLeft(length, '0');
        }

        private static string FormatDiscriminacao(string value)
        {
            var clean = (value ?? string.Empty).Replace("\r", string.Empty).Replace("\n", "|");
            var chunks = Enumerable.Range(0, (int)Math.Ceiling(clean.Length / 100d))
                .Select(i => clean.Substring(i * 100, Math.Min(100, clean.Length - i * 100)))
                .Take(13)
                .ToList();

            var joined = string.Join("|", chunks);
            if (joined.Length > 1000) joined = joined.Substring(0, 1000);
            return joined;
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            // Barueri não requer assinatura digital no envio
        }

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisar o retorno conforme WSDL
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var resposta = xmlRet
                            .Descendants()
                            .FirstOrDefault(x => x.Name.LocalName == "NFeLoteEnviarArquivoResult");

            if (resposta != null)
            {
                var listaMensagem = resposta
                                        .Descendants()
                                        .FirstOrDefault (x => x.Name.LocalName == "ListaMensagemRetorno");

                if (listaMensagem != null)
                {
                    var codigo = listaMensagem.ElementAnyNs("Codigo")?.GetValue<string>();
                    var mensagem = listaMensagem.ElementAnyNs("Mensagem")?.GetValue<string>();
                    var correcao = listaMensagem.ElementAnyNs("Correcao")?.GetValue<string>();

                    if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(mensagem))
                    {
                        if (!codigo.Equals("OK200", StringComparison.OrdinalIgnoreCase))
                        {
                            retornoWebservice.Erros.Add(new EventoRetorno
                            {
                                Codigo = codigo,
                                Descricao = mensagem + (string.IsNullOrEmpty(correcao) ? "" : " - " + correcao)
                            });
                        }
                    }
                }

                var protocolo = resposta
                                .Descendants()
                                .FirstOrDefault(x => x.Name.LocalName == "ProtocoloRemessa")?.GetValue<string>();
                if (!string.IsNullOrEmpty(protocolo))
                {
                    retornoWebservice.Protocolo = protocolo;
                    retornoWebservice.Sucesso = true;
                }
            }
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa envio síncrono.");
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            throw new NotImplementedException("Barueri não implementa envio síncrono.");
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa envio síncrono.");
        }

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            if (string.IsNullOrWhiteSpace(retornoWebservice.Protocolo))
            {
                retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Protocolo não informado." });
                return;
            }

            // MensagemXML deve conter o XML conforme schema NFeLoteStatusArquivo.v1.xsd
            XNamespace ns = "http://www.barueri.sp.gov.br/nfe";
            var envelope = new XElement(ns + "NFeLoteStatusArquivo",
                new XElement(ns + "InscricaoMunicipal", Configuracoes.PrestadorPadrao.InscricaoMunicipal),
                new XElement(ns + "CPFCNPJContrib", Configuracoes.PrestadorPadrao.CpfCnpj),
                new XElement(ns + "ProtocoloRemessa", retornoWebservice.Protocolo)
            );

            retornoWebservice.XmlEnvio = envelope.ToString(SaveOptions.DisableFormatting);
        }

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            // Barueri não requer assinatura
        }

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var resposta = xmlRet
                            .Descendants()
                            .FirstOrDefault(x => x.Name.LocalName == "NFeLoteStatusArquivoResult");

            if (resposta == null) return;

            var listaMensagem = resposta
                                    .Descendants()
                                    .FirstOrDefault(x => x.Name.LocalName == "ListaMensagemRetorno");
            if (listaMensagem != null)
            {
                var codigo = listaMensagem.ElementAnyNs("Codigo")?.GetValue<string>();
                var mensagem = listaMensagem.ElementAnyNs("Mensagem")?.GetValue<string>();
                var correcao = listaMensagem.ElementAnyNs("Correcao")?.GetValue<string>();

                if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(mensagem))
                {
                    if (!codigo.Equals("OK200", StringComparison.OrdinalIgnoreCase))
                    {
                        retornoWebservice.Erros.Add(new EventoRetorno
                        {
                            Codigo = codigo,
                            Descricao = mensagem + (string.IsNullOrEmpty(correcao) ? "" : " - " + correcao)
                        });
                    }
                    else
                    {
                        retornoWebservice.Sucesso = true;
                    }
                }
            }

            var info = resposta.Descendants().FirstOrDefault(x => x.Name.LocalName == "ListaNfeArquivosRPS");
            if (info != null)
            {
                retornoWebservice.Situacao = info.Descendants().FirstOrDefault(x => x.Name.LocalName == "SituacaoArq")?.GetValue<string>() ?? string.Empty;
            }
        }

        public RetornoBaixarArquivo BaixarArquivoRps(string nomeArquivoRetorno)
        {
            var retornoWebservice = new RetornoBaixarArquivo
            {
                NomeArquivoRetorno = nomeArquivoRetorno
            };

            try
            {
                if (string.IsNullOrWhiteSpace(nomeArquivoRetorno))
                {
                    retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nome do arquivo de retorno não informado." });
                    return retornoWebservice;
                }

                XNamespace ns = "http://www.barueri.sp.gov.br/nfe";
                var envelope = new XElement(ns + "NFeLoteBaixarArquivo",
                    new XElement(ns + "InscricaoMunicipal", Configuracoes.PrestadorPadrao.InscricaoMunicipal),
                    new XElement(ns + "CPFCNPJContrib", Configuracoes.PrestadorPadrao.CpfCnpj),
                    new XElement(ns + "NomeArqRetorno", nomeArquivoRetorno)
                );

                retornoWebservice.XmlEnvio = envelope.ToString(SaveOptions.DisableFormatting);

                if (Configuracoes.Geral.RetirarAcentos)
                    retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

                if (PrecisaValidarSchema(TipoUrl.ConsultarNFSeRps))
                {
                    ValidarSchema(retornoWebservice, "NFeLoteBaixarArquivo.v1.xsd");
                    if (retornoWebservice.Erros.Any()) return retornoWebservice;
                }

                using (var cliente = new BarueriServiceClient(this, TipoUrl.ConsultarNFSeRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSeRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                    retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                    retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
                }

                var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
                var resposta = xmlRet.Descendants().FirstOrDefault(x => x.Name.LocalName == "NFeLoteBaixarArquivoResult");
                if (resposta == null) return retornoWebservice;

                var listaMensagem = resposta.Descendants().FirstOrDefault(x => x.Name.LocalName == "ListaMensagemRetorno");
                if (listaMensagem != null)
                {
                    var codigo = listaMensagem.ElementAnyNs("Codigo")?.GetValue<string>();
                    var mensagem = listaMensagem.ElementAnyNs("Mensagem")?.GetValue<string>();
                    var correcao = listaMensagem.ElementAnyNs("Correcao")?.GetValue<string>();

                    if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(mensagem))
                    {
                        if (!codigo.Equals("OK200", StringComparison.OrdinalIgnoreCase))
                        {
                            retornoWebservice.Erros.Add(new EventoRetorno
                            {
                                Codigo = codigo,
                                Descricao = mensagem + (string.IsNullOrEmpty(correcao) ? "" : " - " + correcao)
                            });
                        }
                        else
                        {
                            retornoWebservice.Sucesso = true;
                        }
                    }
                }

                var arquivoBase64 = resposta.Descendants().FirstOrDefault(x => x.Name.LocalName == "ArquivoRPSBase64")?.Value ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(arquivoBase64))
                {
                    retornoWebservice.ConteudoBase64 = arquivoBase64;
                    try
                    {
                        retornoWebservice.Conteudo = Convert.FromBase64String(arquivoBase64);
                    }
                    catch (FormatException ex)
                    {
                        retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
                    }
                }

                return retornoWebservice;
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultaLote xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CPFCNPJPrestador>{Configuracoes.PrestadorPadrao.CpfCnpj}</CPFCNPJPrestador>");
            loteBuilder.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            loteBuilder.Append("</ConsultaLote>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            // Barueri não requer assinatura
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var nfseNodes = xmlRet.ElementsAnyNs("NFe");

            foreach (var nfseNode in nfseNodes)
            {
                var nota = LoadXml(new XDocument(nfseNode));
                notas.Add(nota);
            }
        }

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Barueri não possui consulta de sequencial RPS.");
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Barueri não possui consulta de sequencial RPS.");
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Barueri não possui consulta de sequencial RPS.");
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultaNFe xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CPFCNPJPrestador>{Configuracoes.PrestadorPadrao.CpfCnpj}</CPFCNPJPrestador>");
            loteBuilder.Append($"<NumeroRPS>{notas[0].IdentificacaoRps.Numero}</NumeroRPS>");
            loteBuilder.Append($"<SerieRPS>{notas[0].IdentificacaoRps.Serie}</SerieRPS>");
            loteBuilder.Append("</ConsultaNFe>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            // Barueri não requer assinatura
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var nfseNode = xmlRet.ElementAnyNs("NFe");
            if (nfseNode != null)
            {
                var nota = LoadXml(new XDocument(nfseNode));
                notas.Add(nota);
            }
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException("Barueri não implementa consulta de NFSe por período.");
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException("Barueri não implementa consulta de NFSe por período.");
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa consulta de NFSe por período.");
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<CancelaNFe xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CPFCNPJPrestador>{Configuracoes.PrestadorPadrao.CpfCnpj}</CPFCNPJPrestador>");
            loteBuilder.Append($"<NumeroNFe>{retornoWebservice.NumeroNFSe}</NumeroNFe>");
            loteBuilder.Append($"<Motivo>{retornoWebservice.Motivo}</Motivo>");
            loteBuilder.Append("</CancelaNFe>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            // Barueri não requer assinatura
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var sucesso = xmlRet.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            if (!sucesso)
            {
                var erro = xmlRet.ElementAnyNs("Erro");
                if (erro != null)
                {
                    retornoWebservice.Erros.Add(new EventoRetorno
                    {
                        Codigo = erro.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                        Descricao = erro.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty
                    });
                }
            }
        }

        protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa cancelamento em lote.");
        }

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
        {
            throw new NotImplementedException("Barueri não implementa cancelamento em lote.");
        }

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa cancelamento em lote.");
        }

        protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa substituição de NFSe.");
        }

        protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
        {
            throw new NotImplementedException("Barueri não implementa substituição de NFSe.");
        }

        protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Barueri não implementa substituição de NFSe.");
        }

        protected override string GerarCabecalho()
        {
            return string.Empty;
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return tipo switch
            {
                TipoUrl.Enviar => "NFeLoteEnviarArquivo.v1.xsd",
                TipoUrl.ConsultarSituacao => "NFeLoteStatusArquivo.v1.xsd",
                TipoUrl.ConsultarLoteRps => "NFeLoteStatusArquivo.v1.xsd",
                TipoUrl.ConsultarNFSeRps => "ConsultaNFeRecebidaNumero.v1.xsd",
                TipoUrl.ConsultarNFSe => "ConsultaNFeRecebidaPeriodo.v1.xsd",
                _ => string.Empty
            };
        }

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new BarueriServiceClient(this, tipo);
        }

        #endregion Services

        #endregion Methods
    }
}
