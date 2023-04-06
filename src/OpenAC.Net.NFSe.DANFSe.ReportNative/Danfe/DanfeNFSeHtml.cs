using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenAC.Net.NFSe.DANFSe.ReportNative.Properties;

namespace OpenAC.Net.NFSe.DANFSe.ReportNative.Danfe
{
    public sealed class DanfeNFSeHtml
    {
        private readonly DanfeInfo _danfeInfo;

        public DanfeNFSeHtml(DanfeInfo danfeInfo)
        {
            _danfeInfo = danfeInfo;
        }

        /// <summary>
        ///     Obter DANFE NFSe em formato html
        /// </summary>
        /// <returns></returns>
        public Documento ObterDocHtml()
        { 
            var html = Montar();
            return new Documento { Html = html };
        }

        /// <summary>
        /// Salvar DANFE NFSe em formato html
        /// </summary>
        /// <param name="caminho"></param>
        /// <param name="nomeArquivo"></param>
        public void SalvarDocHtml(string caminho,string nomeArquivo)
        {
            var conteudo = ObterDocHtml();
            Utils.EscreverArquivo(caminho, nomeArquivo, conteudo.Html);
        }

        /// <summary>
        ///     Montar cabeçalho da DANFE
        /// </summary> 
        /// <returns></returns>
        private string Montar()
        {
            var list = new Dictionary<string, string>();
            var nfse = _danfeInfo.NotaServico;
            var nomePrefeitura = _danfeInfo.NomePrefeitura;
            var titulo = _danfeInfo.Titulo;
            var linkImg = _danfeInfo.LinkImagemLogo;

            #region Cabecalho

            list.Add("#ds_image", linkImg);
            list.Add("#ds_title1#", nomePrefeitura);
            list.Add("#ds_title2#", titulo);
            list.Add("#nl_invoice#", nfse.IdentificacaoNFSe.Numero );
            list.Add("#dt_invoice_issue#", nfse.IdentificacaoNFSe.DataEmissao.ToString("G"));
            list.Add("#ds_protocol#", nfse.IdentificacaoNFSe.Chave);

            #endregion 

            #region Prestador


            list.Add("#nl_company_cnpj_cpf#", nfse.Prestador.CpfCnpj);
            list.Add("#ds_company_issuer_name#", nfse.Prestador.NomeFantasia);
            list.Add("#ds_company_address#", nfse.Prestador.Endereco.Logradouro);
            list.Add("#ds_company_neighborhood#", nfse.Prestador.Endereco.Complemento);
            list.Add("#nu_company_cep#", nfse.Prestador.Endereco.Cep);
            list.Add("#ds_company_city_name#", nfse.Prestador.Endereco.Municipio);
            list.Add("#ds_company_im#", nfse.Prestador.InscricaoMunicipal);
            list.Add("#ds_company_uf#", nfse.Prestador.Endereco.Uf);

            #endregion

            #region Tomador

            list.Add("#nl_client_cnpj_cpf#", nfse.Tomador.CpfCnpj);
            list.Add("#ds_client_receiver_name#", nfse.Tomador.NomeFantasia);
            list.Add("#ds_client_address#", nfse.Tomador.Endereco.Logradouro);
            list.Add("#ds_client_neighborhood#", nfse.Tomador.Endereco.Complemento);
            list.Add("#nu_client_cep#", nfse.Tomador.Endereco.Cep);
            list.Add("#ds_client_city_name#", nfse.Tomador.Endereco.Municipio);
            list.Add("#ds_client_uf#", nfse.Tomador.Endereco.Uf);
            list.Add("#nl_client_im#", nfse.Tomador.InscricaoMunicipal);
            list.Add("#ds_client_email#", nfse.Tomador.DadosContato.Email);

            #endregion


            #region Descricao Servicos

            var descricao = nfse.Servico.Descricao;
            var discriminacao = nfse.Servico.Discriminacao;
            var descricao2 = $"{descricao}<br>{discriminacao}";
            list.Add("#services#", descricao2);


            #endregion

            #region Valor Total

            list.Add("#orderTotal#", nfse.Servico.Valores.ValorServicos.ToString());


            #endregion

            #region Impostos

            list.Add("#cofins#", nfse.Servico.Valores.ValorCofins.ToString());
            list.Add("#csll#", nfse.Servico.Valores.ValorCsll.ToString());
            list.Add("#inss#", nfse.Servico.Valores.ValorInss.ToString());
            list.Add("#irpj#", nfse.Servico.Valores.ValorIr.ToString());
            list.Add("#pis#", nfse.Servico.Valores.ValorPis.ToString());
            list.Add("#totaldeducoes#", nfse.Servico.Valores.ValorDeducoes.ToString());
            list.Add("#basecalculo#", nfse.Servico.Valores.BaseCalculo.ToString());
            list.Add("#vl_issqn_aliq#", nfse.Servico.Valores.Aliquota.ToString());
            list.Add("#vl_issqn#", nfse.Servico.Valores.ValorIss.ToString());
            list.Add("#iisqnretido#", nfse.Servico.Valores.ValorIssRetido.ToString());
            list.Add("#serviceCode#", nfse.Servico.CodigoCnae.ToString());
            list.Add("#orderTotallIq#", nfse.Servico.Valores.ValorLiquidoNfse.ToString());
            list.Add("#outrasretencoes#", nfse.Servico.Valores.ValorOutrasRetencoes.ToString());



            #endregion


            #region Outras Informacoes

            list.Add("#ds_additional_information#", nfse.OutrasInformacoes);

            #endregion


            return LerTemplateEsubstituirTags(Resources.nfse_template, list);
        }
         
        /// <summary>
        ///     Substituir chave pelo valor
        /// </summary>
        /// <param name="html"></param>
        /// <param name="replacesList"></param>
        /// <returns></returns>
        private string Substituir(string html, Dictionary<string, string> replacesList)
        {
            foreach (var replaces in replacesList)
                html = Regex.Replace(html, replaces.Key, replaces.Value ?? string.Empty, RegexOptions.IgnoreCase, new TimeSpan(0, 0, 5));
            return html;
        }

        /// <summary>
        ///     Realizar a leitura de arquivo e substitui tags
        /// </summary>
        /// <param name="nomeTemplate"></param>
        /// <param name="dictionaryReplace"></param>
        /// <returns></returns>
        private string LerTemplateEsubstituirTags(string nomeTemplate, Dictionary<string, string> dictionaryReplace)
        {
            if (dictionaryReplace == null) return string.Empty;
            return !dictionaryReplace.Any() ? string.Empty : Substituir(nomeTemplate, dictionaryReplace);
        }

    }
}
